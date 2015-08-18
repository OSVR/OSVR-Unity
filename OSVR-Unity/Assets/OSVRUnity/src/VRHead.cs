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
/// <summary>
/// Author: Bob Berkebile
/// Email: bob@bullyentertainment.com || bobb@pixelplacement.com
/// </summary>

using UnityEngine;
using System.Collections;

namespace OSVR
{
    namespace Unity
    {
        [RequireComponent(typeof(Camera))]
        public class VRHead : MonoBehaviour
        {
            #region Public Variables
            //public Camera Camera { get { return _camera; } set { _camera = value; } }
            public DisplayController DisplayController { get { return _displayController; } set { _displayController = value; } }
            #endregion

            #region Private Variables
            private DisplayController _displayController;
            //private Camera _camera;
            private bool renderedStereo = true;
            private bool updated = false; //whether the headpose has been updated this frame
            private bool updateEarly = false; //if false, update in LateUpdate
            #endregion

            // Update is called once per frame.
            void Update()
            {
                updated = false;  // OK to recompute head pose.
                if (updateEarly)
                {
                    UpdateHeadPose();
                }
            }
            // LateUpdate is called once per frame, after Update has finished. 
            void LateUpdate()
            {
                UpdateHeadPose();
            }
            //Updates the position and rotation of the head
            private void UpdateHeadPose()
            {
                if (updated)
                {  // Only one update per frame.
                    return;
                }
                updated = true;

                _displayController.UpdateClient();
                
                OSVR.ClientKit.Pose3 headPose = _displayController.DisplayConfig.GetViewerPose(DisplayController.DEFAULT_VIEWER);
                transform.localPosition = Math.ConvertPosition(headPose.translation);
                transform.localRotation = Math.ConvertOrientation(headPose.rotation);
            }         
        }
    }
}
