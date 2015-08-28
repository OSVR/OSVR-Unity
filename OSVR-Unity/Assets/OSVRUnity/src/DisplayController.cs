/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2015 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>
/// <summary>
/// Author: Greg Aring
/// Email: greg@sensics.com
/// </summary>
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;


namespace OSVR
{
    namespace Unity
    {
        //This class is responsible for creating the head, eyes, and surfaces in our scene.
        //Rendering parameters are obtained from ClientKit.
        //DisplayController creates VRViewer and VREyes as children. Each eye has a VRSurface child with a camera.
        //In this implementation, we are assuming that there is exactly one viewer and one surface per eye.
        public class DisplayController : MonoBehaviour
        {
            public const uint DEFAULT_VIEWER = 0; //assume exactly one viewer in this Unity implementation
            public const uint DEFAULT_SURFACE = 0; //assume exactly one viewer in this Unity implementation
            public const uint NUM_VIEWERS = 1;
            public const uint NUM_SURFACE_PER_EYE = 1; 

            private ClientKit _clientKit;
            private OsvrRenderManager _renderManager;
            private OSVR.ClientKit.DisplayConfig _displayConfig;
            private VRViewer[] viewers;
            private VREye[] eyes;
            private uint _eyeCount;
            private uint _viewerCount;
            private bool renderedStereo = false;
            private bool displayConfigInitialized = false;
            public bool IsInitialized
            {
                get { return displayConfigInitialized; }
            }
            private bool rtSet = false;
            public bool RtSet
            {
                get { return rtSet; }
            }

            public OSVR.ClientKit.DisplayConfig DisplayConfig
            {
                get { return _displayConfig; }
                set { _displayConfig = value; }
            } 
            public OsvrRenderManager RenderManager
            {
                get { return _renderManager; }
            }
            public VRViewer[] Viewers { get { return viewers; } }           
            public VREye[] Eyes { get { return eyes; } }
            public uint EyeCount { get { return _eyeCount; } }
            public uint ViewerCount { get { return _viewerCount; } }
            public float nearClippingPlane = 0.01f;
            public float farClippingPlane = 1000f;

            void Awake()
            {
                _clientKit = FindObjectOfType<ClientKit>();
                if(_clientKit == null)
                {
                    Debug.LogError("DisplayController requires a ClientKit object in the scene.");
                    return;
                }

                if(SupportsRenderManager() && _renderManager == null)
                {
                    _renderManager = GameObject.FindObjectOfType<OsvrRenderManager>();
                    if(_renderManager == null)
                    {
                        //Add a rendermanager to this gameobject if there isn't one in the scene
                        _renderManager = gameObject.AddComponent<OsvrRenderManager>();
                        _renderManager.InitRenderManager(_clientKit.context);
                    }
                    
                }
                SetupApplicationSettings();
            }
            void Start()
            {
                SetupDisplay();
            }
           
            public bool SupportsRenderManager()
            {
                bool support = true;
#if UNITY_ANDROID
                Debug.Log("RenderManager not yet supported on Android.");
                support = false;
#endif
                if(!SystemInfo.graphicsDeviceVersion.Contains("Direct3D 11.0"))
                {
                    Debug.Log("RenderManager not yet supported on " + 
                        SystemInfo.graphicsDeviceVersion + ". Only D3D11 is currently supported.");
                    support = false;
                }

                if(!SystemInfo.supportsRenderTextures)
                {
                    Debug.Log("RenderManager not supported. RenderTexture (Unity 4 Pro feature) is unavailable.");
                    support = false;
                }

                if(!SupportsUnityRenderEvent())
                {
                    Debug.Log("RenderManager not supported. Unity 4.5+ is needed for UnityRenderEvent.");
                    support = false;
                }
                return support;
            }

            //Unity 4.5+ is needed for UnityRenderEvent.
            private bool SupportsUnityRenderEvent()
            {
                bool support = true;
                try
                {
                    string version = new Regex(@"(\d+\.\d+)\..*").Replace(Application.unityVersion, "$1");
                    if (new Version(version) < new Version("4.5"))
                    {
                        support = false;
                    }
                }
                catch
                {
                    Debug.LogWarning("Unable to determine Unity version from: " + Application.unityVersion);
                }
                return support;
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
                Debug.Log("SetupDisplay");
                //get the DisplayConfig object from ClientKit
                if(_clientKit.context == null)
                {
                    Debug.LogError("ClientContext is null. Can't setup display.");
                    return;
                }
                _displayConfig = _clientKit.context.GetDisplayConfig();
                if (_displayConfig == null)
                {
                    Debug.LogError("Unable to setup display. No DisplayConfig object found.");
                    return;
                }
                Debug.Log("DisplayConfig initialized on frame " + Time.frameCount);
                displayConfigInitialized = true;
                Debug.Log("Let's get the viewer count");
                //get the number of viewers, bail if there isn't exactly one viewer for now
                _viewerCount = _displayConfig.GetNumViewers();
                Debug.Log("Viewer count is " + _viewerCount);
                if(_viewerCount != 1)
                {
                    Debug.LogError(_viewerCount + " viewers found, but this implementation requires exactly one viewer.");
                    return;
                }
                //create scene objects 
                CreateHeadAndEyes();
                
            }

            //Creates a head and eyes as configured in clientKit
            //Viewers and Eyes are siblings, children of DisplayController
            //Each eye has one child Surface which has a camera
            private void CreateHeadAndEyes()
            {
                Debug.Log("CreateHeadAndEyes");
                /* ASSUME ONE VIEWER */
                //Create VRViewers, only one in this implementation
                _viewerCount = (uint)_displayConfig.GetNumViewers();
                if(_viewerCount != NUM_VIEWERS)
                {
                    Debug.LogError(_viewerCount + " viewers detected. This implementation supports exactly one viewer.");
                    return;
                }               
                viewers = new VRViewer[_viewerCount];
                for (int i = 0; i < _viewerCount; i++)
                {
                    //create a VRViewer
                    GameObject vrViewer = new GameObject("VRViewer" + i);
                    vrViewer.AddComponent<AudioListener>(); //add an audio listener
                    VRViewer vrViewerComponent = vrViewer.AddComponent<VRViewer>();
                    vrViewerComponent.Camera = vrViewer.GetComponent<Camera>(); //add a dummy camera, VRViewer requires that it has a camera already
                    vrViewerComponent.Camera.nearClipPlane = nearClippingPlane;
                    vrViewerComponent.Camera.farClipPlane = farClippingPlane;
                    vrViewerComponent.DisplayController = this; //pass DisplayController to Viewers  
                    if(i == 0)
                    {
                        vrViewer.tag = "MainCamera"; //tag a VRViewer as the MainCamera so other gameobjects can reference it 
                    }                                             
                    vrViewer.transform.parent = this.transform; //child of DisplayController
                    vrViewer.transform.localPosition = Vector3.zero;
                }
                

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
                    eyes[i] = eye;
                    CreateEyeSurface(i);
                    if (SupportsRenderManager())
                    {
                        //@todo do something here
                    }
                    else
                    {
                        SetDistortion(i);
                    }
                    
                }
            }

            //Creates a Surface for a given Eye
            //bail if there isn't exactly one surface per eye
            private void CreateEyeSurface(int eyeIndex)
            {
                Debug.Log("Create Eye Surface");
                uint surfaceCount = _displayConfig.GetNumSurfacesForViewerEye(DEFAULT_VIEWER, (byte)eyeIndex);
                if(surfaceCount != 1)
                {
                    Debug.LogError("Eye" + eyeIndex + " has " + surfaceCount + " surfaces, but "+
                        "this implementation requires exactly one surface per eye.");
                    return;
                }
                GameObject surfaceGameObject = new GameObject("Surface");
                VRSurface surface = surfaceGameObject.AddComponent<VRSurface>();
                surface.Camera = surfaceGameObject.AddComponent<Camera>();
                surface.Camera.nearClipPlane = nearClippingPlane;
                surface.Camera.farClipPlane = farClippingPlane;
                if(SupportsRenderManager())
                {                   
                    Debug.Log("About to create render textures");
                    surface.Camera.enabled = false; //only enabled if not rendering to texture                   
                    int width = _renderManager.GetRenderTextureWidth();
                    int height = _renderManager.GetRenderTextureHeight();
                    //create a RenderTexture for this surface
                    Debug.Log("Creating surface rendertexture with dimensions (" + width + ", " + height + ")");
                    surface.SetRenderTexture(new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32));
                    _renderManager.SetEyeColorBuffer(surface.Camera.targetTexture.GetNativeTexturePtr(), eyeIndex);
                    if (eyeIndex == 1) rtSet = true;
                }
                else
                {
                    surface.Camera.enabled = true;
                }
                
                surfaceGameObject.transform.parent = eyes[eyeIndex].transform; //child of Eye
                surfaceGameObject.transform.localPosition = Vector3.zero;
                eyes[eyeIndex].Surface = surface;
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
                    if (eye.Surface.DistortionEffect)
                    {
                        eye.Surface.DistortionEffect.enabled = false;
                    }
                    return;
                }
                // Otherwise try to create distortion and set its parameters
                var distortionFactory = new K1RadialDistortionFactory();
                var effect = distortionFactory.GetOrCreateDistortion(eye.Surface);
                if (effect)
                {
                    effect.k1Red = k1Red;
                    effect.k1Green = k1Green;
                    effect.k1Blue = k1Blue;
                    effect.center = center;
                }
            }

            void Update()
            {
                if(!displayConfigInitialized)
                {
                    SetupDisplay();
                }
            }

            //helper method for updating the client context
            public void UpdateClient()
            {
                _clientKit.context.update(); //update the client
            }
        }
    }
}

