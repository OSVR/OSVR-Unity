﻿/// OSVR-Unity Connection
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
using UnityEngine.Rendering;
using System.Collections;
using System.Text.RegularExpressions;
using System;

namespace OSVR
{
    namespace Unity
    {
        //*This class is responsible for creating stereo rendering in a scene, and updating viewing parameters
        // throughout a scene's lifecycle. 
        // The number of viewers, eyes, and surfaces, as well as viewports, projection matrices,and distortion 
        // paramerters are obtained from OSVR via ClientKit.
        // 
        // DisplayController creates VRViewers and VREyes as children. Although VRViewers and VREyes are siblings
        // in the scene hierarchy, conceptually VREyes are indeed children of VRViewers. The reason they are siblings
        // in the Unity scene is because GetViewerEyePose(...) returns a pose relative to world space, not head space.
        //
        // In this implementation, we are assuming that there is exactly one viewer and one surface per eye.
        //*/
        [RequireComponent(typeof(Camera))] //requires a "dummy" camera
        public class DisplayController : MonoBehaviour
        {
            
            public const uint NUM_VIEWERS = 1;
            private const int TARGET_FRAME_RATE = 60; //@todo get from OSVR

            private ClientKit _clientKit;
            private OSVR.ClientKit.DisplayConfig _displayConfig;
            private VRViewer[] _viewers; 
            private uint _viewerCount;
            private bool _displayConfigInitialized = false;
            private bool _checkDisplayStartup = false;
            private Camera _camera;
            private bool _disabledCamera = true;
            private OsvrRenderManager _renderManager;

            //for testing
            public bool _useRenderManager = true;

            public Camera Camera
            {
                get
                {
                    if (_camera == null)
                    {
                        _camera = GetComponent<Camera>();
                    }
                    return _camera;
                }
                set { _camera = value; }
            }
            public OSVR.ClientKit.DisplayConfig DisplayConfig
            {
                get { return _displayConfig; }
                set { _displayConfig = value; }
            }          
            public VRViewer[] Viewers { get { return _viewers; } }           
            public uint ViewerCount { get { return _viewerCount; } }
            public OsvrRenderManager RenderManager { get { return _renderManager; } }

            private bool shutdown = false;

            void Awake()
            {
                _clientKit = FindObjectOfType<ClientKit>();
                if(_clientKit == null)
                {
                    Debug.LogError("DisplayController requires a ClientKit object in the scene.");
                }
                _camera = GetComponent<Camera>(); //get the "dummy" camera
                SetupApplicationSettings();
                SetupRenderManager();
            }
            void Start()
            {
                //attempt to setup the display here, but it might take a few frames before we have data
               // SetupDisplay();
            }

            void OnEnable()
            {
                shutdown = false;
                StartCoroutine("EndOfFrame");
            }

    
            void OnDisable()
            {
                ClearViewers();
                shutdown = true;
                StopCoroutine("EndOfFrame");
                RenderManager.ShutdownRenderManager();
            }

            void ClearViewers()
            {
                //for each viewer, update each eye, which will update each surface
                for (uint viewerIndex = 0; viewerIndex < _viewerCount; viewerIndex++)
                {
                    VRViewer viewer = Viewers[viewerIndex];
                    viewer.ClearEyes();
                }
            }

            void SetupApplicationSettings()
            {
                //VR should never timeout the screen:
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

                //Set the framerate
                //@todo get this value from OSVR, not a const value
                //Performance note: Developers should try setting Time.fixedTimestep to 1/Application.targetFrameRate
               // Application.targetFrameRate = TARGET_FRAME_RATE;
            }

            void SetupRenderManager()
            {
                if (_useRenderManager && SupportsRenderManager())
                {
                    //Initialize the RenderManager
                    _renderManager = GameObject.FindObjectOfType<OsvrRenderManager>();
                    if (_renderManager == null)
                    {
                        //add a RenderManager component if we're not on mobile
                        _renderManager = gameObject.AddComponent<OsvrRenderManager>();
                    }

                    _renderManager.InitRenderManager(FindObjectOfType<ClientKit>().context);
                }
                else
                {
                    Debug.LogError("RenderManager not supported.");
                }
            }

            //Get a DisplayConfig object from the server via ClientKit.
            //Setup stereo rendering with DisplayConfig data.
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
                _displayConfigInitialized = true;               

                //get the number of viewers, bail if there isn't exactly one viewer for now
                _viewerCount = _displayConfig.GetNumViewers();
                if(_viewerCount != 1)
                {
                    Debug.LogError(_viewerCount + " viewers found, but this implementation requires exactly one viewer.");
                    return;
                }
                //create scene objects 
                CreateHeadAndEyes();
                Camera.cullingMask = 0;              
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
                _viewers = new VRViewer[_viewerCount];
                //loop through viewers because at some point we could support multiple viewers
                //but this implementation currently supports exactly one
                for (uint viewerIndex = 0; viewerIndex < _viewerCount; viewerIndex++)
                {
                    //create a VRViewer
                    GameObject vrViewer = new GameObject("VRViewer" + viewerIndex);
                    vrViewer.AddComponent<AudioListener>(); //add an audio listener
                    VRViewer vrViewerComponent = vrViewer.AddComponent<VRViewer>();
                    vrViewerComponent.DisplayController = this; //pass DisplayController to Viewers  
                    vrViewerComponent.ViewerIndex = viewerIndex; //set the viewer's index                         
                    vrViewer.transform.parent = this.transform; //child of DisplayController
                    vrViewer.transform.localPosition = Vector3.zero;
                    _viewers[viewerIndex] = vrViewerComponent;

                    //create Viewer's VREyes
                    uint eyeCount = (uint)_displayConfig.GetNumEyesForViewer(viewerIndex); //get the number of eyes for this viewer
                    vrViewerComponent.CreateEyes(eyeCount);
                }            
            }
            void Update()
            {
                //sometimes it takes a few frames to get a DisplayConfig from ClientKit
                //keep trying until we have initialized
                if(!_displayConfigInitialized)
                {
                    SetupDisplay();
                }
            }

            //helper method for updating the client context
            public void UpdateClient()
            {
                _clientKit.context.update();
            }

            //Culling determines which objects are visible to the camera. OnPreCull is called just before this process.
            //This gets called because we have a camera component, but we disable the camera here so it doesn't render.
            //We have the "dummy" camera so existing Unity game code can refer to a MainCamera object.
            //We update our viewer and eye transforms here because it is as late as possible before rendering happens.
            //OnPreRender is not called because we disable the camera here.
            void OnPreCull()
            {
                if (!shutdown)
                {
                    // Disable dummy camera during rendering
                    // Enable after frame ends
                    _camera.enabled = false;

                    DoRendering();
                    if (_useRenderManager && _checkDisplayStartup)
                    {

                        Camera.depth = 10;

                    }
                    else if (!_checkDisplayStartup)
                    {
                        _checkDisplayStartup = DisplayConfig.CheckDisplayStartup();
                    }

                    // Flag that we disabled the camera
                    _disabledCamera = true;
                }
            }

            void DoRendering()
            {
                //for each viewer, update each eye, which will update each surface
                for (uint viewerIndex = 0; viewerIndex < _viewerCount; viewerIndex++)
                {
                    VRViewer viewer = Viewers[viewerIndex];

                    //update the client
                    UpdateClient();

                    //update poses once DisplayConfig is ready
                    if (_checkDisplayStartup)
                    {
                        //update the viewer's head pose
                        viewer.UpdateViewerHeadPose(DisplayConfig.GetViewerPose(viewerIndex));

                        //each viewer update its eyes
                        viewer.UpdateEyes();
                    }
                    else
                    {
                        //Debug.Log("No Display " + Time.time);
                        _checkDisplayStartup = DisplayConfig.CheckDisplayStartup();
                    }
                }
            }
            //This couroutine is called every frame.
            IEnumerator EndOfFrame()
            {
                while (true)
                {
                    //if we disabled the dummy camera, enable it here
                    if (_disabledCamera)
                    {
                        Camera.enabled = true;
                        _disabledCamera = false;
                    }
                    yield return new WaitForEndOfFrame();
                    if(_useRenderManager && _checkDisplayStartup && !shutdown)
                    {
                        GL.IssuePluginEvent(_renderManager.GetRenderEventFunction(), 0);
                    }
               
                }
            }

            //Is the RenderManager supported? Requires D3D11 or OpenGL, currently.
            private bool SupportsRenderManager()
            {
                bool support = true;
#if UNITY_ANDROID
                Debug.Log("RenderManager not yet supported on Android.");
                support = false;
#endif
                if (!SystemInfo.graphicsDeviceVersion.Contains("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("Direct3D 11"))
                {
                    Debug.Log("RenderManager not supported on " +
                        SystemInfo.graphicsDeviceVersion + ". Only OpenGL and Direct3D 11 are currently supported.");
                    support = false;
                }

                if (!SystemInfo.supportsRenderTextures)
                {
                    Debug.Log("RenderManager not supported. RenderTexture (Unity Pro feature) is unavailable.");
                    support = false;
                }
                if (!SupportsUnityRenderEvent())
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
        }
    }
}

