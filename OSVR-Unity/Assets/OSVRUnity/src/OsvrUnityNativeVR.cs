/// OsvrUnityNativeVR.cs
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2017 Sensics, Inc.
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
/// 

/* This script connects Unity's VR support to OSVR-Unity and OSVR-RenderManager, so that OSVR-Unity applications can benefit from
 * single-pass rendering and otherr optimiztions. To use it, attach this script to a gameobject with a camera. In Player Settings,
 * check the box for "Virtual Realty Supported" and choose the "Split Stereo Display (non-head-mounted)" option. Choose single-pass
 * or multi-pass for the stereo rendering method.
 * This is not intended to be used in the same scene with  DisplayController/VRViewer/VREye/VRSurface. It only depends on ClientKit.cs
 * to be in the scene.
*/
using OSVR.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using System;

namespace OSVR
{
    namespace Unity
    {
        [RequireComponent(typeof(Camera))]
        public class OsvrUnityNativeVR : MonoBehaviour
        {
            private const int NUM_RENDERBUFFERS = 2;
            private const int LEFT_EYE = 0;
            private const int RIGHT_EYE = 1;
            private const int LEFT_EYE_BUFFER_2 = 2;
            private const int RIGHT_EYE_BUFFER_2 = 3;
            private const int FIRST_BUFFER = 0;
            private const int SECOND_BUFFER = 1;

            public enum StereoRigSetup
            {
                OneCameraBothEyes,
                TwoCameras
            };
            public StereoRigSetup stereoRigSetup = StereoRigSetup.OneCameraBothEyes;
             public Camera _camera0; //the main camera in a one-camera setup, or the left eye in a two-camera setup
             public Camera _camera1; //null in a one-camera setup, or the right eye in a two-camera setup

             private Transform _camera0CachedTransform;
             private Transform _camera1CachedTransform;

            private RenderTexture StereoTargetRenderTextureLeft;
            private RenderTexture StereoTargetRenderTextureRight;
            private RenderTexture StereoTargetRenderTextureLeft_buffer2;
            private RenderTexture StereoTargetRenderTextureRight_buffer2;


            private ClientKit _clientKit;
            private OSVR.ClientKit.DisplayConfig _displayConfig;

            private bool _displayConfigInitialized = false;
            private uint _totalDisplayWidth;
            private uint _totalSurfaceHeight;
            private bool _osvrClientKitError = false;

            //variables for controlling use of osvrUnityRenderingPlugin.dll which enables DirectMode
            private OsvrRenderManager _renderManager;
            private bool _renderManagerConfigFound = false;

            public OSVR.ClientKit.DisplayConfig DisplayConfig
            {
                get { return _displayConfig; }
                set { _displayConfig = value; }
            }
            public OsvrRenderManager RenderManager { get { return _renderManager; } }

            public uint TotalDisplayWidth
            {
                get { return _totalDisplayWidth; }
                set { _totalDisplayWidth = value; }
            }

            public uint TotalDisplayHeight
            {
                get { return _totalSurfaceHeight; }
                set { _totalSurfaceHeight = value; }
            }

            private int frameCount = 0;

            void Awake()
            {
                _camera0 = GetComponent<Camera>();
                _camera0CachedTransform = this.transform;
                if (stereoRigSetup == StereoRigSetup.TwoCameras)
                {
                    Camera[] cameras = transform.parent.GetComponentsInChildren<Camera>();
                    foreach (Camera c in cameras)
                    {
                        if (c != _camera0)
                        {
                            _camera1 = c; //set the right eye camera to the main camera's sibling.
                            _camera1CachedTransform = _camera1.transform;
                        }
                    }
                    if (cameras.Length < 2)
                    {
                        Debug.LogError("[OSVR-Unity] Two-camera VR setup cannot find 2nd camera. Add a 2nd camera as a sibling of the main camera.");
                    }
                }
            }
            void Start()
            {
                _clientKit = ClientKit.instance;
                if (_clientKit == null)
                {
                    Debug.LogError("[OSVR-Unity] OsvrUnityNativeVR requires a ClientKit object in the scene.");
                }

            }

            //setup and VR, Application, Player, Quality, and Screen settings
            void SetVRAppSettings()
            {
                //Disable autovr camera tracking since the camera's transform is set by RenderManager poses. 
#if UNITY_2017
                if(_clientKit.context.CheckStatus())
                {
                    VRDevice.DisableAutoVRCameraTracking(_camera0, true);
                }

#endif
                VRSettings.showDeviceView = false;

                //Application.targetFrameRate = 90;
                Application.targetFrameRate = -1;
                Application.runInBackground = true;
                QualitySettings.vSyncCount = 0;
                QualitySettings.maxQueuedFrames = -1; //limit the number of frames queued up to be rendered, reducing latency
                Screen.sleepTimeout = SleepTimeout.NeverSleep;  //VR should never timeout the screen:
            }

            // Setup RenderManager for DirectMode or non-DirectMode rendering.
            // Checks to make sure Unity version and Graphics API are supported, 
            // and that a RenderManager config file is being used.
            void InitRenderManager()
            {
                //check if we are configured to use RenderManager or not
                string renderManagerPath = _clientKit.context.getStringParameter("/renderManagerConfig");
                _renderManagerConfigFound = !(renderManagerPath == null || renderManagerPath.Equals(""));
                if (_renderManagerConfigFound)
                {
                    //found a RenderManager config
                    _renderManager = GameObject.FindObjectOfType<OsvrRenderManager>();
                    if (_renderManager == null)
                    {
                        GameObject renderManagerGameObject = new GameObject("RenderManager");
                        //add a RenderManager component
                        _renderManager = renderManagerGameObject.AddComponent<OsvrRenderManager>();
                    }

                    //check to make sure Unity version and Graphics API are supported
                    bool supportsRenderManager = _renderManager.IsRenderManagerSupported();
                    _renderManagerConfigFound = supportsRenderManager;
                    if (!_renderManagerConfigFound)
                    {
                        Debug.LogError("[OSVR-Unity] RenderManager config found but RenderManager is not supported.");
                        Destroy(_renderManager);
                    }
                    else
                    {
                        // attempt to create a RenderManager in the plugin                                              
                        int result = _renderManager.InitRenderManager();
                        if (result != 0)
                        {
                            Debug.LogError("[OSVR-Unity] Failed to create RenderManager.");
                            _renderManagerConfigFound = false;
                            VRSettings.enabled = false; //disable VR mode
                        }
                    }
                }
                else
                {
                    Debug.Log("[OSVR-Unity] RenderManager config not detected. Using normal Unity rendering path.");
                }
            }

            // Get a DisplayConfig object from the server via ClientKit.
            // Setup stereo rendering with DisplayConfig data.
            void Init()
            {
                //get the DisplayConfig object from ClientKit
                if (_clientKit == null || _clientKit.context == null)
                {
                    if (!_osvrClientKitError)
                    {
                        Debug.LogError("[OSVR-Unity] ClientContext is null. Can't setup display.");
                        _osvrClientKitError = true;
                    }
                    return;
                }
                SetVRAppSettings();

                _displayConfig = _clientKit.context.GetDisplayConfig();
                if (_displayConfig == null)
                {
                    return;
                }
                _displayConfigInitialized = true;

                InitRenderManager();
                if (!_renderManagerConfigFound || RenderManager == null)
                {
                    return;
                }
                SetupStereoCamerarig();
                SetResolution();
                CreateRenderTextures();

                //create RenderBuffers in RenderManager
                if (_renderManagerConfigFound && RenderManager != null)
                {
                    RenderManager.ConstructBuffers();
                }
                SetRenderParams();

            }

            /* 
             * We currently are only able to set the projection matrix of each eye with a two camera setup, via camera.projectionMatrix.
            * For configurations where the projection matrix for each eye is identical, we use the one-camera setup.
            * For configs where the proj matrices are different per eye, we use the two camera setup
            * This may be due to camera.SetStereoProjectionMatrix(...) not working as expected.
            * 
            * This function switches from a one-camera to a two-camera setup when the one-camera setup won't 
            * produce a correct stereo pair work with the display.
            */
            private void SetupStereoCamerarig()
            {
                if (stereoRigSetup == StereoRigSetup.OneCameraBothEyes)
                {
                    //check if the projection matrices are the same
                    //@todo is this the best way to tell if the COP are off-axis?
                    Matrix4x4 matrix0 = RenderManager.GetEyeProjectionMatrix(0);
                    Matrix4x4 matrix1 = RenderManager.GetEyeProjectionMatrix(1);

                    if (matrix0 != matrix1)
                    {
                        Debug.Log("[OSVR-Unity] Stereo projection matrices not identical, switching to two-camera stereo setup.");
                        stereoRigSetup = StereoRigSetup.TwoCameras;
                        //rename this gameobject and call it the left camera
                        this.transform.name = "OsvrStereoCameraLeft";
                        //create a new gameobject for the right eye camera
                        GameObject rightEyeGO = new GameObject("OsvrStereoCameraRight");
                        rightEyeGO.transform.parent = this.transform.parent;
                        rightEyeGO.transform.localPosition = Vector3.zero;
                        Camera rightEyeCamera = rightEyeGO.AddComponent<Camera>();
                        rightEyeCamera.CopyFrom(_camera0);
                        rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
                        _camera1 = rightEyeCamera;
                        _camera1CachedTransform = _camera1.transform;
                    }
                }

            }

            private void SetRenderScale()
            {
                //@todo
                //VRSettings.renderScale = overfill_factor;
            }

            private void SetRenderViewportScale()
            {
                //@todo
                //VRSettings.renderViewportScale = overfill_factor;
            }

            //Set RenderManager rendering parameters: near and far clip plane distance, projection matrices
            private void SetRenderParams()
            {
                RenderManager.SetNearClippingPlaneDistance(_camera0.nearClipPlane);
                RenderManager.SetFarClippingPlaneDistance(_camera0.farClipPlane);
                //@todo set renderscale to overfillfactor
                SetRenderScale();
                SetRenderViewportScale();
                SetProjectionMatrix();

            }

            //Set Resolution of the Unity game window based on total surface width
            private void SetResolution()
            {
                TotalDisplayWidth = 0; //add up the width of each eye
                TotalDisplayHeight = 0; //don't add up heights

                int numDisplayInputs = DisplayConfig.GetNumDisplayInputs();
                //for each display
                for (int i = 0; i < numDisplayInputs; i++)
                {
                    OSVR.ClientKit.DisplayDimensions surfaceDisplayDimensions = DisplayConfig.GetDisplayDimensions((byte)i);

                    TotalDisplayWidth += (uint)surfaceDisplayDimensions.Width; //add up the width of each eye
                    TotalDisplayHeight = (uint)surfaceDisplayDimensions.Height; //store the height -- this shouldn't change
                }

                //Set the resolution. Don't force fullscreen if we have multiple display inputs
                //We only need to do this if we aren't using RenderManager, because it adjusts the window size for us
                //@todo figure out why this causes problems with direct mode, perhaps overfill factor?
                if (numDisplayInputs > 1 && !_renderManagerConfigFound)
                {
                    Screen.SetResolution((int)TotalDisplayWidth, (int)TotalDisplayHeight, false);
                }

            }

            private void CreateRenderTextures()
            {
                if (stereoRigSetup == StereoRigSetup.OneCameraBothEyes)
                {
                    //create a RenderTexture for this eye's camera to render into
                    RenderTexture renderTexture = new RenderTexture((int)TotalDisplayWidth, (int)TotalDisplayHeight, 24, RenderTextureFormat.Default);
                    if (QualitySettings.antiAliasing > 0)
                    {
                        renderTexture.antiAliasing = QualitySettings.antiAliasing;
                    }
                    StereoTargetRenderTextureLeft = renderTexture;
                    StereoTargetRenderTextureLeft.Create();
                    _camera0.targetTexture = StereoTargetRenderTextureLeft;
                    //  RenderTexture.active = StereoTargetRenderTexture0;

                    //Set the native texture pointer so we can access this texture from the plugin
                    RenderManager.SetEyeColorBuffer(renderTexture.GetNativeTexturePtr(), 0, 0);
                    RenderManager.SetEyeColorBuffer(renderTexture.GetNativeTexturePtr(), 1, 0);

                    if (NUM_RENDERBUFFERS == 2)
                    {
                        //create a RenderTexture for this eye's camera to render into
                        RenderTexture rt2 = new RenderTexture((int)TotalDisplayWidth, (int)TotalDisplayHeight, 24, RenderTextureFormat.Default);
                        if (QualitySettings.antiAliasing > 0)
                        {
                            rt2.antiAliasing = QualitySettings.antiAliasing;
                        }
                        StereoTargetRenderTextureLeft_buffer2 = rt2;
                        StereoTargetRenderTextureLeft_buffer2.Create();
                        // RenderTexture.active = StereoTargetRenderTexture0_buffer2;

                        RenderManager.SetEyeColorBuffer(rt2.GetNativeTexturePtr(), 0, 1);
                        RenderManager.SetEyeColorBuffer(rt2.GetNativeTexturePtr(), 1, 1);
                        //make the first target active

                        RenderTexture.active = StereoTargetRenderTextureLeft;


                    }


                }
                else //two-camera setup
                {
                    //left eye
                    OSVR.ClientKit.Viewport leftEyeViewport = RenderManager.GetEyeViewport(0);

                    //create a RenderTexture for this eye's camera to render into
                    RenderTexture renderTexture0 = new RenderTexture(leftEyeViewport.Width, leftEyeViewport.Height, 24, RenderTextureFormat.Default);
                    if (QualitySettings.antiAliasing > 0)
                    {
                        renderTexture0.antiAliasing = QualitySettings.antiAliasing;
                    }
                    StereoTargetRenderTextureLeft = renderTexture0;
                    StereoTargetRenderTextureLeft.Create();
                    _camera0.targetTexture = StereoTargetRenderTextureLeft;

                   

                    //right eye
                    OSVR.ClientKit.Viewport rightEyeViewport = RenderManager.GetEyeViewport(1);

                    //create a RenderTexture for this eye's camera to render into
                    RenderTexture renderTexture1 = new RenderTexture(rightEyeViewport.Width, rightEyeViewport.Height, 24, RenderTextureFormat.Default);
                    if (QualitySettings.antiAliasing > 0)
                    {
                        renderTexture1.antiAliasing = QualitySettings.antiAliasing;
                    }
                    StereoTargetRenderTextureRight = renderTexture1;
                    StereoTargetRenderTextureRight.Create();
                    _camera1.targetTexture = StereoTargetRenderTextureRight;

                    //set rendermanager color buffers
                    //Set the native texture pointer so we can access this texture from the plugin
                    //set rendermanager color buffers
                    //Set the native texture pointer so we can access this texture from the plugin
                    RenderManager.SetEyeColorBuffer(renderTexture0.GetNativeTexturePtr(), 0, 0);
                    RenderManager.SetEyeColorBuffer(renderTexture1.GetNativeTexturePtr(), 1, 0);

                    if(NUM_RENDERBUFFERS == 2)
                    {
                        //create a RenderTexture for this eye's camera to render into
                        RenderTexture renderTexture0_b2 = new RenderTexture(leftEyeViewport.Width, leftEyeViewport.Height, 24, RenderTextureFormat.Default);
                        if (QualitySettings.antiAliasing > 0)
                        {
                            renderTexture0_b2.antiAliasing = QualitySettings.antiAliasing;
                        }
                        StereoTargetRenderTextureLeft_buffer2 = renderTexture0_b2;
                        StereoTargetRenderTextureLeft_buffer2.Create();



                        //create a RenderTexture for this eye's camera to render into
                        RenderTexture renderTexture1_b2 = new RenderTexture(leftEyeViewport.Width, rightEyeViewport.Height, 24, RenderTextureFormat.Default);
                        if (QualitySettings.antiAliasing > 0)
                        {
                            renderTexture1_b2.antiAliasing = QualitySettings.antiAliasing;
                        }
                        StereoTargetRenderTextureRight_buffer2 = renderTexture1_b2;
                        StereoTargetRenderTextureRight_buffer2.Create();

                        //set rendermanager color buffers
                        //Set the native texture pointer so we can access this texture from the plugin
                        RenderManager.SetEyeColorBuffer(renderTexture0_b2.GetNativeTexturePtr(), 0, 1);
                        //set rendermanager color buffers
                        //Set the native texture pointer so we can access this texture from the plugin
                        RenderManager.SetEyeColorBuffer(renderTexture1_b2.GetNativeTexturePtr(), 1, 1);

                        RenderTexture.active = StereoTargetRenderTextureLeft;

                    }


                }

            }

            void SwapRenderTextures()
            {
                int frame = frameCount % 2;
                if (stereoRigSetup == StereoRigSetup.OneCameraBothEyes)
                {
                    RenderTexture buff;
                    if (frame == 0)
                    {
                        buff = StereoTargetRenderTextureLeft;
                        //  RenderTexture.active = StereoTargetRenderTexture0;
                        _camera0.targetTexture = StereoTargetRenderTextureLeft;


                    }
                    else
                    {
                        buff = StereoTargetRenderTextureLeft_buffer2;
                        //  RenderTexture.active = StereoTargetRenderTexture0_buffer2;
                        _camera0.targetTexture = StereoTargetRenderTextureLeft_buffer2;

                    }
                }
                else
                {
                    RenderTexture buff0;
                    RenderTexture buff1;
                    if (frame == 0)
                    {
                        buff0 = StereoTargetRenderTextureLeft;
                        _camera0.targetTexture = StereoTargetRenderTextureLeft;

                        buff1 = StereoTargetRenderTextureRight;
                        _camera1.targetTexture = StereoTargetRenderTextureRight;


                    }
                    else
                    {
                        buff0 = StereoTargetRenderTextureLeft_buffer2;
                        _camera0.targetTexture = StereoTargetRenderTextureLeft_buffer2;

                        buff1 = StereoTargetRenderTextureRight_buffer2;
                        _camera1.targetTexture = StereoTargetRenderTextureRight_buffer2;

                    }
                }

                frameCount++;
            }

            void OnPreCull()
            {
                if (_displayConfigInitialized && RenderManager != null && _renderManagerConfigFound)
                {

                    SwapRenderTextures();
                }
            }


            void LateUpdate()
            {
                // sometimes it takes a few frames to get a DisplayConfig from ClientKit
                // keep trying until we have initialized
                if (!_displayConfigInitialized)
                {
                    Init();
                }
                else if (_displayConfigInitialized && RenderManager != null && _renderManagerConfigFound)
                {
                    GL.IssuePluginEvent(RenderManager.GetRenderEventFunction(), OsvrRenderManager.UPDATE_RENDERINFO_EVENT);
                    if (stereoRigSetup == StereoRigSetup.OneCameraBothEyes)
                    {
                        UpdateHeadPose();

                    }
                    else
                    {
                        UpdateEyePoses();
                    }
                    GL.IssuePluginEvent(RenderManager.GetRenderEventFunction(), OsvrRenderManager.RENDER_EVENT);
                    // SwapRenderTextures();
                }
            }

            //Set each Unity camera's projectionMatrix
            //Projetion Matrices come from RenderManager
            private void SetProjectionMatrix()
            {
                //use near and far clipping plane values from the Unity inspector
                float nearPlane = _camera0.nearClipPlane;
                float farPlane = _camera0.farClipPlane;

                //modify the matrix with near and far clipping planes
                float c = (farPlane + nearPlane) / (farPlane - nearPlane);
                float d = -nearPlane * (1.0f + c);

                if (stereoRigSetup == StereoRigSetup.OneCameraBothEyes)
                {
                    Matrix4x4 matrix0 = RenderManager.GetEyeProjectionMatrix(0);

                    matrix0[2, 2] = -c;
                    matrix0[2, 3] = d;
                    _camera0.projectionMatrix = matrix0;

                    //Debug.Log("Proj Matrix0 is " + matrix0);

                    //@todo I thought this should achieve the same result as the line above, but it doesn't work that way
                    // _camera0.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matrix0);
                    //_camera0.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, matrix0);

                    //this doesn't work as expected, either
                    //Matrix4x4 matrix1 = RenderManager.GetEyeProjectionMatrix(1);
                    //Debug.Log("Proj Matrix1 is " + matrix1);

                    // _camera0.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matrix0);
                    //_camera0.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, matrix1);
                }
                else
                {
                    Matrix4x4 matrix0 = RenderManager.GetEyeProjectionMatrix(0);
                    matrix0[2, 2] = -c;
                    matrix0[2, 3] = d;
                    Matrix4x4 matrix1 = RenderManager.GetEyeProjectionMatrix(1);
                    matrix1[2, 2] = -c;
                    matrix1[2, 3] = d;
                    _camera0.projectionMatrix = matrix0;
                    _camera1.projectionMatrix = matrix1;

                    //this doesnt work either
                    // _camera0.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matrix0);
                    //_camera1.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, matrix1);
                }
            }


            // Updates the position and rotation of the eye
            // Optionally, update the viewer associated with this eye
            public void UpdateEyePoses()
            {
                OSVR.ClientKit.Pose3 eyePose0 = RenderManager.GetRenderManagerEyePose((byte)0);
                OSVR.ClientKit.Pose3 eyePose1 = RenderManager.GetRenderManagerEyePose((byte)1);


                // Convert from OSVR space into Unity space.
                Vector3 pos0 = OSVR.Unity.Math.ConvertPosition(eyePose0.translation);
                Quaternion rot0 = OSVR.Unity.Math.ConvertOrientation(eyePose0.rotation);

                Vector3 pos1 = OSVR.Unity.Math.ConvertPosition(eyePose1.translation);
                Quaternion rot1 = OSVR.Unity.Math.ConvertOrientation(eyePose1.rotation);

                if (stereoRigSetup == StereoRigSetup.OneCameraBothEyes && _camera0CachedTransform != null)
                {
                    Quaternion slerpedRot = Quaternion.Slerp(rot0, rot1, 0.5f);
                    Vector3 pos = new Vector3((pos0.x + pos1.x) * 0.5f, (pos0.y + pos1.y) * 0.5f, (pos0.z + pos1.z) * 0.5f);

                    // Invert the transformation
                    _camera0CachedTransform.localRotation = Quaternion.Inverse(slerpedRot);
                    Vector3 invPos = -pos;
                    _camera0CachedTransform.localPosition = Quaternion.Inverse(slerpedRot) * invPos;
                }
                else if (_camera0CachedTransform != null && _camera1CachedTransform != null)//two-camera setup
                {
                    //this script is attached to the left eye, with a right-eye sibling gameobject
                    _camera0CachedTransform.localRotation = Quaternion.Inverse(rot0);
                    Vector3 invPos = -pos0;
                    _camera0CachedTransform.localPosition = Quaternion.Inverse(rot0) * invPos;

                    _camera1CachedTransform.localRotation = Quaternion.Inverse(rot1);
                    invPos = -pos1;
                    _camera1CachedTransform.localPosition = Quaternion.Inverse(rot1) * invPos;
                }

            }

            // Updates the position and rotation of the head
            // Optionally, update the viewer associated with this head
            public void UpdateHeadPose()
            {
                OSVR.ClientKit.Pose3 eyePose0 = RenderManager.GetRenderManagerEyePose((byte)0);
                OSVR.ClientKit.Pose3 eyePose1 = RenderManager.GetRenderManagerEyePose((byte)1);


                // Convert from OSVR space into Unity space.
                Vector3 pos0 = OSVR.Unity.Math.ConvertPosition(eyePose0.translation);
                Quaternion rot0 = OSVR.Unity.Math.ConvertOrientation(eyePose0.rotation);

                Vector3 pos1 = OSVR.Unity.Math.ConvertPosition(eyePose1.translation);
                Quaternion rot1 = OSVR.Unity.Math.ConvertOrientation(eyePose1.rotation);

                Quaternion slerpedRot = Quaternion.Slerp(rot0, rot1, 0.5f);
                Vector3 pos = new Vector3((pos0.x + pos1.x) * 0.5f, (pos0.y + pos1.y) * 0.5f, (pos0.z + pos1.z) * 0.5f);

                // Invert the transformation
                _camera0CachedTransform.localRotation = Quaternion.Inverse(slerpedRot);
                Vector3 invPos = -pos;
                _camera0CachedTransform.localPosition = Quaternion.Inverse(slerpedRot) * invPos;
            }

            public bool CheckDisplayStartup()
            {
                return DisplayConfig != null && _displayConfigInitialized && DisplayConfig.CheckDisplayStartup();
            }

        }
    }
}