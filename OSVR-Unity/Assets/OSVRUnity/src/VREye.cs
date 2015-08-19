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
using System.Reflection;

namespace OSVR
{
    namespace Unity
    {
        public class VREye : MonoBehaviour
        {
            #region Private Variables
            private DisplayController _displayController;
            private VRSurface _surface; //the surface associated with this eye
            private int _eyeIndex;
            private bool updated = false; //whether the headpose has been updated this frame
            private bool updateEarly = false; //if false, update in LateUpdate
            
            #endregion
            #region Public Variables  
            public int EyeIndex
            {
                get { return _eyeIndex; }
                set { _eyeIndex = value; }
            }
            public DisplayController DisplayController 
            { 
                get { return _displayController; } 
                set { _displayController = value; } 
            }
            public VRSurface Surface
            {
                get { return _surface; }
                set { _surface = value; }
            }
            [HideInInspector]
            public Transform cachedTransform;
            #endregion

            #region Init
            void Awake()
            {
                Init();
            }
            #endregion

            #region Public Methods
            #endregion

            #region Private Methods
            void Init()
            {
                //cache:
                cachedTransform = transform;
            }         
            #endregion

            #region Loop
            // Update is called once per frame.
            void Update()
            {
                updated = false;  // OK to recompute head pose.
                if (updateEarly)
                {
                    UpdateEyePose();
                }
            }
            // LateUpdate is called once per frame, after Update has finished. 
            void LateUpdate()
            {
                UpdateEyePose();
            }
            //Updates the position and rotation of the head
            private void UpdateEyePose()
            {
                if (updated)
                {  // Only one update per frame.
                    return;
                }
                updated = true;

                _displayController.UpdateClient();

                OSVR.ClientKit.Pose3 eyePose = _displayController.DisplayConfig.GetViewerEyePose(DisplayController.DEFAULT_VIEWER, (byte)_eyeIndex);
                cachedTransform.localPosition = Math.ConvertPosition(eyePose.translation);
                cachedTransform.localRotation = Math.ConvertOrientation(eyePose.rotation);
            }
            #endregion


        }
    }
}
