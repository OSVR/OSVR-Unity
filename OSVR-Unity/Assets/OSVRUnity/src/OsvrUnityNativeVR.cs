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
using UnityEngine;
using UnityEngine.VR;

[RequireComponent(typeof(Camera))]
public class OsvrUnityNativeVR : MonoBehaviour {

    private Camera _camera;

    [HideInInspector]
    public RenderTexture StereoTargetRenderTexture;
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


    void OnDisable()
    {
        ExitRenderManager();
    }
    void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    void Start()
    {
        _clientKit = ClientKit.instance;
        if (_clientKit == null)
        {
            Debug.LogError("[OSVR-Unity] OsvrUnityNativeVR requires a ClientKit object in the scene.");
        }

        SetVRAppSettings();
    }

    //setup and VR, Application, Player, Quality, and Screen settings
    void SetVRAppSettings()
    {
        //Disable autovr camera tracking since the camera's transform is set by RenderManager poses. 
        VRDevice.DisableAutoVRCameraTracking(_camera, true);
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

        _displayConfig = _clientKit.context.GetDisplayConfig();
        if (_displayConfig == null)
        {
            return;
        }
        _displayConfigInitialized = true;

        InitRenderManager();
        SetResolution();
        CreateRenderTextures();

        //create RenderBuffers in RenderManager
        if (_renderManagerConfigFound && RenderManager != null)
        {
            RenderManager.ConstructBuffers();
        }
        SetRenderParams();
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

    //Set RenderManager rendering parameters: near and far clip plane distance and IPD
    private void SetRenderParams()
    {
        RenderManager.SetNearClippingPlaneDistance(_camera.nearClipPlane);
        RenderManager.SetFarClippingPlaneDistance(_camera.farClipPlane);
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

        //create a RenderTexture for this eye's camera to render into
        RenderTexture renderTexture = new RenderTexture((int)TotalDisplayWidth, (int)TotalDisplayHeight, 24, RenderTextureFormat.Default);
        if (QualitySettings.antiAliasing > 0)
        {
            renderTexture.antiAliasing = QualitySettings.antiAliasing;
        }
        StereoTargetRenderTexture = renderTexture;
        _camera.targetTexture = StereoTargetRenderTexture;
        RenderTexture.active = StereoTargetRenderTexture;

        //Set the native texture pointer so we can access this texture from the plugin
        RenderManager.SetEyeColorBuffer(renderTexture.GetNativeTexturePtr(), 0);
        RenderManager.SetEyeColorBuffer(renderTexture.GetNativeTexturePtr(), 1);

    }


    void LateUpdate()
    {
        // sometimes it takes a few frames to get a DisplayConfig from ClientKit
        // keep trying until we have initialized
        if (!_displayConfigInitialized)
        {
            Init();
        }
        else if(RenderManager != null && _renderManagerConfigFound)
        {
            GL.IssuePluginEvent(RenderManager.GetRenderEventFunction(), OsvrRenderManager.UPDATE_RENDERINFO_EVENT);
            UpdateHeadPose();
            GL.IssuePluginEvent(RenderManager.GetRenderEventFunction(), OsvrRenderManager.RENDER_EVENT);
        }        
    }

    private void SetProjectionMatrix()
    { 
        Matrix4x4 matrix0 = RenderManager.GetEyeProjectionMatrix(0);
       // Matrix4x4 matrix1 = RenderManager.GetEyeProjectionMatrix(1);

       // _camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matrix0);
       // _camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, matrix1);
        _camera.projectionMatrix = matrix0;

    }

    // Updates the position and rotation of the eye
    // Optionally, update the viewer associated with this eye
    public void UpdateHeadPose()
    {
        OSVR.ClientKit.Pose3 eyePose0 = RenderManager.GetRenderManagerEyePose((byte)0);
        OSVR.ClientKit.Pose3 eyePose1 = RenderManager.GetRenderManagerEyePose((byte)1);


        // Convert from OSVR space into Unity space.
        Vector3 pos0 = OSVR.Unity.Math.ConvertPosition(eyePose0.translation);
        Quaternion rot0 = OSVR.Unity.Math.ConvertOrientation(eyePose0.rotation);

        Vector3 pos1 = OSVR.Unity.Math.ConvertPosition(eyePose1.translation);
        Quaternion rot1 = OSVR.Unity.Math.ConvertOrientation(eyePose1.rotation);

        Quaternion slerpy = Quaternion.Slerp(rot0, rot1, 0.5f);
        Vector3 pos = new Vector3((pos0.x + pos1.x) * 0.5f, (pos0.y + pos1.y) * 0.5f, (pos0.z + pos1.z) * 0.5f);

        // Invert the transformation
        transform.localRotation = Quaternion.Inverse(slerpy);
        Vector3 invPos = -pos;
        transform.localPosition = Quaternion.Inverse(slerpy) * invPos;
    }

    public bool CheckDisplayStartup()
    {
        return DisplayConfig != null && _displayConfigInitialized && DisplayConfig.CheckDisplayStartup();
    }

    public void ExitRenderManager()
    {
        if (_renderManagerConfigFound && RenderManager != null)
        {
            RenderManager.ExitRenderManager();
        }
    }

    /*
    private void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
        {

            //Retrieves the number of dropped frames reported by the VR SDK.
            int droppedFrames;
            if (VRStats.TryGetDroppedFrameCount(out droppedFrames))
            {
                GUI.Label(new Rect(0, 0, 200, 200), "Dropped frames: " + droppedFrames);
            }

            //Retrieves the number of times the current frame has been drawn to the device as reported by the VR SDK.
            int framePresentCount;
            if (VRStats.TryGetFramePresentCount(out framePresentCount))
            {
                GUI.Label(new Rect(0, 200, 200, 200), "Frame Present Count: " + framePresentCount);
            }

            //Retrieves the time spent by the GPU last frame, in seconds, as reported by the VR SDK.
            float gpuTimeSpentLastFrame;
            if (VRStats.TryGetGPUTimeLastFrame(out gpuTimeSpentLastFrame))
            {
                GUI.Label(new Rect(0, 400, 200, 200), "GPU Time spent last frame: " + gpuTimeSpentLastFrame);
            }
        }
    }
    */
}
