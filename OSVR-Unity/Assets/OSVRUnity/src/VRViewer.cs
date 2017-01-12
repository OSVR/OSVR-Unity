/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
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

using UnityEngine;
using System.Collections;

namespace OSVR
{
    namespace Unity
    {
        [RequireComponent(typeof(Camera))]  
        public class VRViewer : MonoBehaviour
        {   
            #region Public Variables         
            public DisplayController DisplayController { get { return _displayController; } set { _displayController = value; } }
            public VREye[] Eyes { get { return _eyes; } }
            public uint EyeCount { get { return _eyeCount; } }
            public uint ViewerIndex { get { return _viewerIndex; } set { _viewerIndex = value; } }
            [HideInInspector]
            public Transform cachedTransform;
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
            #endregion

            #region Private Variables
            private DisplayController _displayController;
            private VREye[] _eyes;
            private uint _eyeCount;
            private uint _viewerIndex;
            private Camera _camera;
            private bool _disabledCamera = true;
            private bool _hmdConnectionError = false;
            private Rect _emptyViewport = new Rect(0, 0, 0, 0);
			private IEnumerator _endOfFrameCoroutine;

            #endregion

            void Awake()
            {
                Init();
            }

            void Init()
            {
				if (_camera == null)
				{
					_camera = GetComponent<Camera>();
					//cache:
					cachedTransform = transform;
					if (DisplayController == null)
					{
						DisplayController = FindObjectOfType<DisplayController>();
					}
					
					_endOfFrameCoroutine = EndOfFrame();
				}
            }

            void OnEnable()
            {
                Init();
				
				if (DisplayController != null)
					StartCoroutine(_endOfFrameCoroutine);
            }

            void OnDisable()
            {
				if (DisplayController != null)
				{
					StopCoroutine(_endOfFrameCoroutine);
                    //@todo any cleanup of RenderTextures necessary here?
				}
            }

            //Creates the Eyes of this Viewer
            public void CreateEyes(uint eyeCount)
            {
                _eyeCount = eyeCount; //cache the number of eyes this viewer controls
                _eyes = new VREye[_eyeCount];

                uint eyeIndex = 0;
                uint foundEyes = 0;

                //Check if there are already VREyes in the scene.
                //If so, use them instead of creating a new 
                VREye[] eyesInScene = FindObjectsOfType<VREye>();
                foundEyes = (uint)eyesInScene.Length;
                if (eyesInScene != null && foundEyes > 0)
                {
                    for (eyeIndex = 0; eyeIndex < eyesInScene.Length; eyeIndex++)
                    {
                        VREye eye = eyesInScene[eyeIndex];
                        // get the VREye gameobject
                        GameObject eyeGameObject = eye.gameObject;
                        eyeGameObject.name = "VREye" + eyeIndex;
                        eye.Viewer = this; 
                        eye.EyeIndex = eyeIndex; //set the eye's index
                        eyeGameObject.transform.parent = DisplayController.transform; //child of DisplayController
                        eyeGameObject.transform.localPosition = Vector3.zero;
                        eyeGameObject.transform.rotation = this.transform.rotation;
                        _eyes[eyeIndex] = eye;
                        uint eyeSurfaceCount = DisplayController.DisplayConfig.GetNumSurfacesForViewerEye(ViewerIndex, (byte)eyeIndex);
                        eye.CreateSurfaces(eyeSurfaceCount);
                    }
                }

                for (; eyeIndex < _eyeCount; eyeIndex++)
                {
                    if(foundEyes == 0)
                    {
                        GameObject eyeGameObject = new GameObject("Eye" + eyeIndex); //add an eye gameobject to the scene
                        VREye eye = eyeGameObject.AddComponent<VREye>(); //add the VReye component
                        eye.Viewer = this; //ASSUME THERE IS ONLY ONE VIEWER
                        eye.EyeIndex = eyeIndex; //set the eye's index
                        eyeGameObject.transform.parent = DisplayController.transform; //child of DisplayController
                        eyeGameObject.transform.localPosition = Vector3.zero;
                        eyeGameObject.transform.rotation = this.transform.rotation;
                        _eyes[eyeIndex] = eye;
                        //create the eye's rendering surface
                        uint eyeSurfaceCount = DisplayController.DisplayConfig.GetNumSurfacesForViewerEye(ViewerIndex, (byte)eyeIndex);
                        eye.CreateSurfaces(eyeSurfaceCount);
                    }
                    else
                    {
                        //if we need to create a new VREye, and there is already one in the scene (eyeIndex > 0), 
                        //duplicate the last eye found instead of creating new gameobjects
                        GameObject eyeGameObject = (GameObject)Instantiate(_eyes[eyeIndex - 1].gameObject);
                        VREye eye = eyeGameObject.GetComponent<VREye>(); //add the VReye component
                        eye.Viewer = this; //ASSUME THERE IS ONLY ONE VIEWER
                        eye.EyeIndex = eyeIndex; //set the eye's index
                        eyeGameObject.name = "VREye" + eyeIndex;
                        eyeGameObject.transform.parent = DisplayController.transform; //child of DisplayController
                        eyeGameObject.transform.localPosition = Vector3.zero;
                        eyeGameObject.transform.rotation = this.transform.rotation;
                        _eyes[eyeIndex] = eye;
                        uint eyeSurfaceCount = DisplayController.DisplayConfig.GetNumSurfacesForViewerEye(ViewerIndex, (byte)eyeIndex);
                        eye.CreateSurfaces(eyeSurfaceCount);
                    }
                    
                }
            }

            //Get an updated tracker position + orientation
            public OSVR.ClientKit.Pose3 GetViewerPose(uint viewerIndex)
            {
                return DisplayController.DisplayConfig.GetViewerPose(viewerIndex);
            }

            //Updates the position and rotation of the head
            public void UpdateViewerHeadPose(OSVR.ClientKit.Pose3 headPose)
            {
                cachedTransform.localPosition = Math.ConvertPosition(headPose.translation);
                cachedTransform.localRotation = Math.ConvertOrientation(headPose.rotation);
            }

            //Update the pose of each eye, then update and render each eye's surfaces
            public void UpdateEyes()
            {
                if (DisplayController.UseRenderManager)
                {
                    //Update RenderInfo
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
                    GL.IssuePluginEvent(DisplayController.RenderManager.GetRenderEventFunction(), OsvrRenderManager.UPDATE_RENDERINFO_EVENT);
#else
                    Debug.LogError("[OSVR-Unity] GL.IssuePluginEvent failed. This version of Unity cannot support RenderManager.");
                    DisplayController.UseRenderManager = false;
#endif
                }
                else
                {
                    DisplayController.UpdateClient();
                }
                    
                for (uint eyeIndex = 0; eyeIndex < EyeCount; eyeIndex++)
                {                   
                    //update the eye pose
                    VREye eye = Eyes[eyeIndex];

                    if (DisplayController.UseRenderManager)
                    { 
                        //get eye pose from RenderManager                     
                        eye.UpdateEyePose(DisplayController.RenderManager.GetRenderManagerEyePose((byte)eyeIndex));
                    }
                    else
                    {
                        //get eye pose from DisplayConfig
                        eye.UpdateEyePose(_displayController.DisplayConfig.GetViewerEyePose(ViewerIndex, (byte)eyeIndex));
                    }
                        

                    // update the eye's surfaces, includes call to Render
                    eye.UpdateSurfaces();                   
                }
            }

            //helper method for updating the client context
            public void UpdateClient()
            {
                DisplayController.UpdateClient();
            }

            // Culling determines which objects are visible to the camera. OnPreCull is called just before this process.
            // This gets called because we have a camera component, but we disable the camera here so it doesn't render.
            // We have the "dummy" camera so existing Unity game code can refer to a MainCamera object.
            // We update our viewer and eye transforms here because it is as late as possible before rendering happens.
            // OnPreRender is not called because we disable the camera here.
            void OnPreCull()
            {
                //leave the preview camera enabled if there is no display config
                _camera.enabled = !DisplayController.CheckDisplayStartup();

                DoRendering();

                //Sends queued-up commands in the driver's command buffer to the GPU.
                //only accessible in Unity 5.4+ API
#if !(UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_7 || UNITY_4_6)
                GL.Flush();
#endif

                // Flag that we disabled the camera
                _disabledCamera = true;
            }

            // The main rendering loop, should be called late in the pipeline, i.e. from OnPreCull
            // Set our viewer and eye poses and render to each surface.
            void DoRendering()
            {
                // update poses once DisplayConfig is ready
                if (DisplayController.CheckDisplayStartup())
                {
                    if(_hmdConnectionError)
                    {
                        _hmdConnectionError = false;
                        Debug.Log("[OSVR-Unity] HMD connection established. You can ignore previous error messages indicating Display Startup failure.");
                    }

                    // update the viewer's head pose
                    // currently getting viewer pose from DisplayConfig always
                    UpdateViewerHeadPose(GetViewerPose(ViewerIndex));

                    // each viewer updates its eye poses, viewports, projection matrices
                    UpdateEyes();

                }
                else
                {
                    if(!_hmdConnectionError)
                    {
                        //report an error message once if the HMD is not connected
                        //it can take a few frames to connect under normal operation, so inidcate when this error has been resolved
                        _hmdConnectionError = true;
                        Debug.LogError("[OSVR-Unity] Display Startup failed. Check HMD connection.");
                    }
                    
                }
            }

            // This couroutine is called every frame.
            IEnumerator EndOfFrame()
            {
                while (true)
                {                  
                    yield return new WaitForEndOfFrame();
                    if (DisplayController.UseRenderManager && DisplayController.CheckDisplayStartup())
                    {
                        // Issue a RenderEvent, which copies Unity RenderTextures to RenderManager buffers
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
                        GL.Viewport(_emptyViewport);
                        GL.Clear(false, true, Camera.backgroundColor);                      
                        GL.IssuePluginEvent(DisplayController.RenderManager.GetRenderEventFunction(), OsvrRenderManager.RENDER_EVENT); 
                        if(DisplayController.showDirectModePreview)
                        {
                            Camera.Render();
                        } 
                                             
#else
                        Debug.LogError("[OSVR-Unity] GL.IssuePluginEvent failed. This version of Unity cannot support RenderManager.");
                        DisplayController.UseRenderManager = false;
#endif
                    }
                    //if we disabled the dummy camera, enable it here
                    if (_disabledCamera)
                    {
                        Camera.enabled = true;
                        _disabledCamera = false;
                    }
                    //Sends queued-up commands in the driver's command buffer to the GPU.
                    //only accessible in Unity 5.4+ API
#if !(UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_7 || UNITY_4_6)
                GL.Flush();
#endif
                }
            }             
        }
    }
}
