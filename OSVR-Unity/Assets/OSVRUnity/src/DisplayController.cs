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
            private OSVR.ClientKit.DisplayConfig _displayConfig;
            private VRViewer[] viewers;
            private VREye[] eyes;
            private uint _eyeCount;
            private uint _viewerCount;
            private bool renderedStereo = false;
            private bool displayConfigInitialized = false;

            public OSVR.ClientKit.DisplayConfig DisplayConfig
            {
                get { return _displayConfig; }
                set { _displayConfig = value; }
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
                }
                SetupApplicationSettings();
            }
            void Start()
            {
                SetupDisplay();
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
                if(_clientKit.context == null)
                {
                    Debug.LogError("ClientContext is null. Can't setup display.");
                    return;
                }
                _displayConfig = _clientKit.context.GetDisplayConfig();
                if (_displayConfig == null)
                {
                    return;
                }
                displayConfigInitialized = true;

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
            //Viewers and Eyes are siblings, children of DisplayController
            //Each eye has one child Surface which has a camera
            private void CreateHeadAndEyes()
            {
                /* ASSUME ONE VIEWER */
                //Create VRViewers, only one in this implementation
                _viewerCount = (uint)_displayConfig.GetNumViewers();
                if(_viewerCount != NUM_VIEWERS)
                {
                    Debug.LogError(_viewerCount + " viewers detected. This implementation supports exactly one viewer.");
                    return;
                }               
                viewers = new VRViewer[_viewerCount];
                for (uint viewerIndex = 0; viewerIndex < _viewerCount; viewerIndex++)
                {
                    //create a VRViewer
                    GameObject vrViewer = new GameObject("VRViewer" + viewerIndex);
                    vrViewer.AddComponent<AudioListener>(); //add an audio listener
                    VRViewer vrViewerComponent = vrViewer.AddComponent<VRViewer>();
                    vrViewerComponent.Camera = vrViewer.GetComponent<Camera>(); //add a dummy camera, VRViewer requires that it has a camera already
                    vrViewerComponent.Camera.nearClipPlane = nearClippingPlane;
                    vrViewerComponent.Camera.farClipPlane = farClippingPlane;
                    vrViewerComponent.DisplayController = this; //pass DisplayController to Viewers  
                    if(viewerIndex == 0)
                    {
                        vrViewer.tag = "MainCamera"; //tag a VRViewer as the MainCamera so other gameobjects can reference it 
                    }                                             
                    vrViewer.transform.parent = this.transform; //child of DisplayController
                    vrViewer.transform.localPosition = Vector3.zero;
                    viewers[viewerIndex] = vrViewerComponent;

                    //create Viewer's VREyes
                    _eyeCount = (uint)_displayConfig.GetNumEyesForViewer(viewerIndex); //get the number of eyes
                    eyes = new VREye[_eyeCount];
                    for (uint eyeIndex = 0; eyeIndex < _eyeCount; eyeIndex++)
                    {
                        GameObject eyeGameObject = new GameObject("Eye" + eyeIndex); //add an eye gameobject to the scene
                        VREye eye = eyeGameObject.AddComponent<VREye>(); //add the VReye component
                        eye.Viewer = viewers[viewerIndex]; //ASSUME THERE IS ONLY ONE VIEWER
                        eye.EyeIndex = eyeIndex; //set the eye's index
                        eyeGameObject.transform.parent = this.transform; //child of DisplayController
                        eyeGameObject.transform.localPosition = Vector3.zero;
                        eyes[eyeIndex] = eye;
                        CreateEyeSurface(eyeIndex);
                        SetDistortion(eyeIndex);
                    }
                }            
            }

            //Creates a Surface for a given Eye
            //bail if there isn't exactly one surface per eye
            private void CreateEyeSurface(uint eyeIndex)
            {
                uint surfaceCount = _displayConfig.GetNumSurfacesForViewerEye(DEFAULT_VIEWER, (byte)eyeIndex);
                if(surfaceCount != 1)
                {
                    Debug.LogError("Eye" + eyeIndex + " has " + surfaceCount + " surfaces, but "+
                        "this implementation requires exactly one surface per eye.");
                    return;
                }
                GameObject surfaceGameObject = new GameObject("Surface");
                VRSurface surface = surfaceGameObject.AddComponent<VRSurface>();
                surface.Eye = eyes[eyeIndex];
                surface.Camera = surfaceGameObject.AddComponent<Camera>();
                surface.Camera.nearClipPlane = nearClippingPlane;
                surface.Camera.farClipPlane = farClippingPlane;
                surface.Camera.enabled = false;
                surfaceGameObject.transform.parent = eyes[eyeIndex].transform; //surface is child of Eye
                surfaceGameObject.transform.localPosition = Vector3.zero;
                eyes[eyeIndex].Surface = surface;
            }

            //determines if distortion will be used, and what type of distortion will be used
            //set distortion parameters accordingly for the given eye.
            private void SetDistortion(uint eyeIndex)
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
            private void SetK1RadialDistortion(uint eyeIndex, float k1Red, float k1Green, float k1Blue, Vector2 center)
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

