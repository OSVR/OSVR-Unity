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
        public class VRViewer : MonoBehaviour
        {   
            #region Public Variables         
            public DisplayController DisplayController { get { return _displayController; } set { _displayController = value; } }
            public VREye[] Eyes { get { return _eyes; } }
            public uint EyeCount { get { return _eyeCount; } }
            public uint ViewerIndex { get { return _viewerIndex; } set { _viewerIndex = value; } }
            [HideInInspector]
            public Transform cachedTransform;
            #endregion

            #region Private Variables
            private DisplayController _displayController;
            private VREye[] _eyes;
            private uint _eyeCount;
            private uint _viewerIndex;

            #endregion

            void Awake()
            {
                Init();
            }

            void Init()
            {
                //cache:
                cachedTransform = transform;
            }

            //Creates the Eyes of this Viewer
            public void CreateEyes(uint eyeCount)
            {
                _eyeCount = eyeCount; //cache the number of eyes this viewer controls
                _eyes = new VREye[_eyeCount];
                for (uint eyeIndex = 0; eyeIndex < _eyeCount; eyeIndex++)
                {
                    GameObject eyeGameObject = new GameObject("Eye" + eyeIndex); //add an eye gameobject to the scene
                    VREye eye = eyeGameObject.AddComponent<VREye>(); //add the VReye component
                    eye.Viewer = this; //ASSUME THERE IS ONLY ONE VIEWER
                    eye.EyeIndex = eyeIndex; //set the eye's index
                    eyeGameObject.transform.parent = DisplayController.transform; //child of DisplayController
                    eyeGameObject.transform.localPosition = Vector3.zero;
                    _eyes[eyeIndex] = eye;
                    uint eyeSurfaceCount = DisplayController.DisplayConfig.GetNumSurfacesForViewerEye(ViewerIndex, (byte)eyeIndex);
                    eye.CreateSurfaces(eyeSurfaceCount);
                }
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
#if UNITY_5_2 || UNITY_5_3
                    GL.IssuePluginEvent(DisplayController.RenderManager.GetRenderEventFunction(), OsvrRenderManager.UPDATE_RENDERINFO_EVENT);
#else
                    Debug.LogError("GL.IssuePluginEvent failed. This version of Unity is not supported by RenderManager.");
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
        }
    }
}
