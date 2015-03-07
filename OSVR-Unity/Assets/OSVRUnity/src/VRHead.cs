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

        public enum ViewMode { stereo, mono };

        [RequireComponent(typeof(Camera))]
        [RequireComponent(typeof(DisplayInterface))]
        public class VRHead : MonoBehaviour
        {
            #region Public Variables
            public ViewMode viewMode;

            [Range(0, 1)]
            public float stereoAmount;

            public float maxStereo = .03f;
            #endregion

            #region Private Variables
            VREye _leftEye;
            VREye _rightEye;
            float _previousStereoAmount;
            ViewMode _previousViewMode;
            #endregion

            #region Init
            void Start()
            {
                Init();
                GetDeviceDescription();
                CatalogEyes();
            }

            #endregion

            #region Loop
            void Update()
            {
                UpdateStereoAmount();
                UpdateViewMode();
            }
            #endregion

            #region Public Methods
            #endregion

            #region Private Methods
            void UpdateViewMode()
            {
                if (Time.realtimeSinceStartup < 100 || _previousViewMode != viewMode)
                {
                    switch (viewMode)
                    {
                        case ViewMode.mono:
                            camera.enabled = true;
                            _leftEye.camera.enabled = false;
                            _rightEye.camera.enabled = false;
                            break;

                        case ViewMode.stereo:
                            camera.enabled = false;
                            _leftEye.camera.enabled = true;
                            _rightEye.camera.enabled = true;
                            break;
                    }
                }

                _previousViewMode = viewMode;
            }

            void UpdateStereoAmount()
            {
                if (stereoAmount != _previousStereoAmount)
                {
                    stereoAmount = Mathf.Clamp(stereoAmount, 0, 1);
                    _rightEye.cachedTransform.localPosition = Vector3.right * (maxStereo * stereoAmount);
                    _leftEye.cachedTransform.localPosition = Vector3.left * (maxStereo * stereoAmount);
                    _previousStereoAmount = stereoAmount;
                }
            }

            void CatalogEyes()
            {
                foreach (VREye currentEye in GetComponentsInChildren<VREye>())
                {
                    //match:
                    currentEye.MatchCamera(camera);

                    //catalog:
                    switch (currentEye.eye)
                    {
                        case Eye.left:
                            _leftEye = currentEye;
                            break;

                        case Eye.right:
                            _rightEye = currentEye;
                            break;
                    }
                }
            }

            void Init()
            {
                //VR should never timeout the screen:
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

                //60 FPS whenever possible:
                Application.targetFrameRate = 60;
            }

            private void GetDeviceDescription()
            {
                DeviceDescriptor deviceDescriptor = GetComponent<DisplayInterface>().GetDeviceDescription();
                switch (deviceDescriptor.DisplayMode)
                {
                    case "full_screen":
                        viewMode = ViewMode.mono;
                        break;
                    case "horz_side_by_side":
                    case "vert_side_by_side":
                    default:
                        viewMode = ViewMode.stereo;
                        break;
                }
                stereoAmount = Mathf.Clamp(deviceDescriptor.OverlapPercent, 0, 100);
                camera.fieldOfView = Mathf.Clamp(deviceDescriptor.MonocularVertical, 0, 180); //unity camera FOV is vertical
                SetResolution(deviceDescriptor.Width, deviceDescriptor.Height);

                //if the view needs to be rotated 180 degrees, create a parent game object that is flipped 180 degrees on the z axis.
                if(deviceDescriptor.Rotate180 > 0)
                {
                    GameObject vrHeadParent = new GameObject();
                    vrHeadParent.name = this.transform.name + "_parent";
                    vrHeadParent.transform.position = this.transform.position;
                    vrHeadParent.transform.rotation = this.transform.rotation;
                    if(this.transform.parent != null)
                    {
                        vrHeadParent.transform.parent = this.transform.parent;
                    }
                    this.transform.parent = vrHeadParent.transform;
                    vrHeadParent.transform.Rotate(0, 0, 180, Space.Self);
                }
            }

            private void SetResolution(int width, int height)
            {
                //set the resolution
                Screen.SetResolution(width, height, true);
#if UNITY_EDITOR
                UnityEditor.PlayerSettings.defaultScreenWidth = width;
                UnityEditor.PlayerSettings.defaultScreenHeight = height;
                UnityEditor.PlayerSettings.defaultIsFullScreen = true;
#endif
            }
            #endregion
        }
    }
}
