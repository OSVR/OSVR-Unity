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
            private VRSurface _surface; //the surface associated with this eye
            private VRViewer viewer; //the viewer associated with this eye
            private uint _eyeIndex;
            
            #endregion
            #region Public Variables  
            public uint EyeIndex
            {
                get { return _eyeIndex; }
                set { _eyeIndex = value; }
            }
            public VRSurface Surface
            {
                get { return _surface; }
                set { _surface = value; }
            }
            public VRViewer Viewer
            {
                get { return viewer; }
                set { viewer = value; }
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

            // Updates the position and rotation of the eye
            // Optionally, update the viewer associated with this eye
            public void UpdateEyePose(OSVR.ClientKit.Pose3 eyePose)
            { 
                cachedTransform.localPosition = Math.ConvertPosition(eyePose.translation);
                cachedTransform.localRotation = Math.ConvertOrientation(eyePose.rotation);
            }
        }
    }
}
