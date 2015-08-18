using UnityEngine;
using System.Collections;

namespace OSVR
{
    namespace Unity
    {
        public class DisplayController : MonoBehaviour
        {
            public const uint DEFAULT_VIEWER = 1; //assume exactly one viewer in this Unity implementation
            public const uint DEFAULT_SURFACE = 1; //assume exactly one viewer in this Unity implementation

            private ClientKit _clientKit;
            private OSVR.ClientKit.DisplayConfig _displayConfig;
            private VRHead _head;
            private VREye[] eyes;
            private uint _eyeCount;
            private uint _viewerCount;
            private Camera _camera;
            private bool renderedStereo = false;

            public OSVR.ClientKit.DisplayConfig DisplayConfig
            {
                get { return _displayConfig; }
                set { _displayConfig = value; }
            }          
            public VRHead Head { get { return _head; } }           
            public VREye[] Eyes { get { return eyes; } }
            
              
            void Awake()
            {
                _clientKit = FindObjectOfType<ClientKit>();
                if(_clientKit == null)
                {
                    Debug.LogError("DisplayController requires a ClientKit object in the scene.");
                }
                if((_camera == GetComponent<Camera>() == null))
                {
                    _camera = gameObject.AddComponent<Camera>();
                }
            }
            void OnEnable()
            {
                SetupApplicationSettings();
                SetupDisplay();
                StartCoroutine("EndOfFrame");
            }

            void OnDisable()
            {
                StopCoroutine("EndOfFrame");
            }

            void SetupApplicationSettings()
            {
                //VR should never timeout the screen:
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

                //60 FPS whenever possible:
                Application.targetFrameRate = 60;
            }

            void SetupDisplay()
            {
                //get the DisplayConfig object from ClientKit
                _displayConfig = _clientKit.context.GetDisplayConfig();
                if (_displayConfig == null)
                {
                    Debug.LogError("Unable to setup display. No DisplayConfig object found.");
                    return;
                }
                //get the number of viewers, bail if there isn't exactly one viewer for now
                _viewerCount = _displayConfig.GetNumViewers();
                if(_viewerCount != 1)
                {
                    Debug.LogError(_viewerCount + " viewers found, but this implementation requires exactly one viewer.");
                    return;
                }
                //create scene objects 
                CreateHeadAndEyes();
                
            }

            //Creates a head and eyes as configured in clientKit
            //Head and Eyes are siblings, children of DisplayController
            //Each eye has one child Surface which has a camera
            private void CreateHeadAndEyes()
            {
                /* ASSUME ONE VIEWER */
                //create a VRHead
                GameObject vrHead = new GameObject("VRHead");
                vrHead.AddComponent<Camera>(); //add a dummy camera
                vrHead.AddComponent<AudioListener>(); //add an audio listener
                _head = vrHead.AddComponent<VRHead>();
                _head.DisplayController = this; //pass DisplayController to Head           
                vrHead.transform.parent = this.transform; //child of DisplayController
                vrHead.transform.localPosition = Vector3.zero;

                //create VREyes
                _eyeCount = (uint)_displayConfig.GetNumEyesForViewer(DEFAULT_VIEWER); //get the number of eyes
                eyes = new VREye[_eyeCount];
                for (int i = 0; i < _eyeCount; i++)
                {
                    GameObject eyeGameObject = new GameObject("Eye" + i); //add an eye gameobject to the scene
                    VREye eye = eyeGameObject.AddComponent<VREye>(); //add the VReye component
                    eye.DisplayController = this; //pass DisplayController to Eye
                    eye.EyeIndex = i; //set the eye's index
                    eyeGameObject.transform.parent = this.transform; //child of DisplayController
                    eyeGameObject.transform.localPosition = Vector3.zero;
                    //@todo add more eyes to enum other than left and right and center?
                    eyes[i] = eye;
                    CreateEyeSurface(i);
                    SetDistortion(i);
                }
            }

            //Creates a Surface for a given Eye
            //bail if there isn't exactly one surface per eye
            private void CreateEyeSurface(int eyeIndex)
            {
                uint surfaceCount = _displayConfig.GetNumSurfacesForViewerEye(DEFAULT_VIEWER, (byte)eyeIndex);
                if(surfaceCount != 1)
                {
                    Debug.LogError("Eye" + eyeIndex + " has " + surfaceCount + " surfaces, but "+
                        "this implementation requires exactly one surface per eye.");
                    return;
                }
                GameObject surface = new GameObject("Surface");
                surface.AddComponent<Camera>();
                surface.AddComponent<VRSurface>();
                surface.transform.parent = eyes[eyeIndex].transform; //child of Eye
                surface.transform.localPosition = Vector3.zero;
            }

            //determines if distortion will be used, and what type of distortion will be used
            //set distortion parameters accordingly for the given eye.
            private void SetDistortion(int eyeIndex)
            {
                bool useDistortion = _displayConfig.DoesViewerEyeSurfaceWantDistortion(DEFAULT_VIEWER, (byte)eyeIndex, DEFAULT_SURFACE);
                if (!useDistortion)
                { //return if we do not want distortion
                    return;
                }
                
                //@todo figure out which type of distortion to use
                //right now, there is only one option
                OSVR.ClientKit.RadialDistortionParameters distortionParameters = 
                    _displayConfig.GetViewerEyeSurfaceRadialDistortion(DEFAULT_VIEWER, (byte)eyeIndex, DEFAULT_SURFACE);
                float k1Red = (float)distortionParameters.k1.x;
                float k1Green = (float)distortionParameters.k1.y;
                float k1Blue = (float)distortionParameters.k1.z;
                Vector2 center = new Vector2((float)distortionParameters.centerOfProjection.x, 
                    (float)distortionParameters.centerOfProjection.y);
                SetK1RadialDistortion(eyeIndex, k1Red, k1Green, k1Blue, center);
            }

            //set distortion parameters for K1 Radial Distortion method
            private void SetK1RadialDistortion(int eyeIndex, float k1Red, float k1Green, float k1Blue, Vector2 center)
            {
                VREye eye = eyes[eyeIndex];
                // disable distortion if there is no distortion for this HMD
                if (k1Red == 0 && k1Green == 0 && k1Blue == 0)
                {
                    if (eye.DistortionEffect)
                    {
                        eye.DistortionEffect.enabled = false;
                    }
                    return;
                }
                // Otherwise try to create distortion and set its parameters
                var distortionFactory = new K1RadialDistortionFactory();
                var effect = distortionFactory.GetOrCreateDistortion(eye);
                if (effect)
                {
                    effect.k1Red = k1Red;
                    effect.k1Green = k1Green;
                    effect.k1Blue = k1Blue;
                    effect.center = center;
                }
            }

            void OnPreCull()
            {
                UpdateClient();

                // Turn off the mono camera so it doesn't waste time rendering.
                // @note mono camera is left on from beginning of frame till now
                // in order that other game logic (e.g. Camera.main) continues
                // to work as expected.
                _camera.enabled = false;

                float near = 0.1f;
                float far = 1000f;

                //render each eye camera (each surface)
                //assumes one surface per eye
                for (int i = 0; i < _eyeCount; i++)
                {
                    VRSurface surface = eyes[i].Surface;
                    OSVR.ClientKit.Viewport viewport = _displayConfig.GetRelativeViewportForViewerEyeSurface(DEFAULT_VIEWER, (byte)i, DEFAULT_SURFACE);
                    surface.SetViewport(Math.ConvertViewport(viewport));
                    OSVR.ClientKit.Matrix44f projMatrix = _displayConfig.GetProjectionMatrixForViewerEyeSurfacef(DEFAULT_VIEWER, (byte)i, DEFAULT_SURFACE,
                        near, far, OSVR.ClientKit.MatrixConventionsFlags.ColMajor);
                    surface.SetProjectionMatrix(Math.ConvertMatrix(projMatrix));
                    surface.Render();
                }

                // Remember to reenable.
                renderedStereo = true;
            }

            IEnumerator EndOfFrame()
            {
                while (true)
                {
                    // If *we* turned off the mono cam, turn it back on for next frame.
                    if (renderedStereo)
                    {
                        _camera.enabled = true;
                        renderedStereo = false;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }

            public void UpdateClient()
            {
                _clientKit.context.update(); //update the client
            }
        }
    }
}

