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
            public Camera Camera { get { return _camera; } set { _camera = value; } }
            #endregion

            #region Private Variables
            private VREye _leftEye;
            private VREye _rightEye;
            private float _previousStereoAmount;
            private ViewMode _previousViewMode;
            private Camera _camera;
            private DeviceDescriptor _deviceDescriptor;
            private OsvrDistortion _distortionEffect;
            private bool _initDisplayInterface = false;
            #endregion

            #region Init
            void Start()
            {
                Init();
                CatalogEyes();
                _distortionEffect = GetComponent<OsvrDistortion>();
                if (_distortionEffect != null)
                {
                    _distortionEffect.enabled = (viewMode == ViewMode.mono);
                }

                //update VRHead with info from the display interface if it has been initialized
                //it might not be initialized if it is still loading/parsing a display json file
                //in that case, we will try to initialize asap in the update function
                if (GetComponent<DisplayInterface>().Initialized)
                {
                    UpdateDisplayInterface();                  
                }              
            }         
            #endregion

            #region Loop
            void Update()
            {
                if(!_initDisplayInterface && GetComponent<DisplayInterface>().Initialized)
                {
                    UpdateDisplayInterface();
                }
                UpdateStereoAmount();
                UpdateViewMode();
            }
            #endregion

            #region Public Methods
            #endregion

            #region Private Methods
            private void UpdateDisplayInterface()
            {
                GetDeviceDescription();
                MatchEyes(); //copy camera properties to each eye
                //rotate each eye based on overlap percent, must do this after match eyes
                if (_deviceDescriptor != null)
                {
                    SetEyeRotation(_deviceDescriptor.OverlapPercent, _deviceDescriptor.MonocularHorizontal);
                    SetEyeRoll(_deviceDescriptor.LeftRoll, _deviceDescriptor.RightRoll);
                }
                _initDisplayInterface = true;
            }
            void UpdateViewMode()
            {
                if (Time.realtimeSinceStartup < 100 || _previousViewMode != viewMode)
                {
                    switch (viewMode)
                    {
                        case ViewMode.mono:
                            Camera.enabled = true;
                            _leftEye.Camera.enabled = false;
                            _rightEye.Camera.enabled = false;
                            break;

                        case ViewMode.stereo:
                            Camera.enabled = false;
                            _leftEye.Camera.enabled = true;
                            _rightEye.Camera.enabled = true;
                            
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

            //this function finds and initializes each eye
            void CatalogEyes()
            {
                foreach (VREye currentEye in GetComponentsInChildren<VREye>())
                {
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

            //this function matches the camera on each eye to the camera on the head
            void MatchEyes()
            {
                foreach (VREye currentEye in GetComponentsInChildren<VREye>())
                {
                    //match:
                    currentEye.MatchCamera(Camera);
                }
            }

            void Init()
            {
                if (Camera == null)
                {
                    if ((Camera = GetComponent<Camera>()) == null)
                    {
                        Camera = gameObject.AddComponent<Camera>();
                    }               
                }

                //VR should never timeout the screen:
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

                //60 FPS whenever possible:
                Application.targetFrameRate = 60;

                _initDisplayInterface = false;
            }

            /// <summary>
            /// GetDeviceDescription: Get a Description of the HMD and apply appropriate settings
            /// 
            /// </summary>
            private void GetDeviceDescription()
            {
                _deviceDescriptor = GetComponent<DisplayInterface>().GetDeviceDescription();              
                if (_deviceDescriptor != null)
                {
                    Debug.Log(_deviceDescriptor.ToString());
                    switch (_deviceDescriptor.DisplayMode)
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
                    stereoAmount = Mathf.Clamp(_deviceDescriptor.OverlapPercent, 0, 100);
                    SetResolution(_deviceDescriptor.Width, _deviceDescriptor.Height); //set resolution before FOV
                    Camera.fieldOfView = Mathf.Clamp(_deviceDescriptor.MonocularVertical, 0, 180); //unity camera FOV is vertical
                    SetDistortion(_deviceDescriptor.K1Red, _deviceDescriptor.K1Green, _deviceDescriptor.K1Blue, 
                    _deviceDescriptor.CenterProjX, _deviceDescriptor.CenterProjY); //set distortion shader
            
                    //if the view needs to be rotated 180 degrees, create a parent game object that is flipped 180 degrees on the z axis.
                    if (_deviceDescriptor.Rotate180 > 0)
                    {
                        GameObject vrHeadParent = new GameObject();
                        vrHeadParent.name = this.transform.name + "_parent";
                        vrHeadParent.transform.position = this.transform.position;
                        vrHeadParent.transform.rotation = this.transform.rotation;
                        if (this.transform.parent != null)
                        {
                            vrHeadParent.transform.parent = this.transform.parent;
                        }
                        this.transform.parent = vrHeadParent.transform;
                        vrHeadParent.transform.Rotate(0, 0, 180, Space.Self);
                    }
                }
            }

            private void SetDistortion(float k1Red, float k1Green, float k1Blue, float centerProjX, float centerProjY)
            {
                if(_distortionEffect != null)
                {
                    _distortionEffect.k1Red = k1Red;
                    _distortionEffect.k1Green = k1Green;
                    _distortionEffect.k1Blue = k1Blue;
                    _distortionEffect.fullCenter = new Vector2(centerProjX, centerProjY);
                }
            }

            //Set the Screen Resolution
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
            
            //rotate each eye based on overlap percent and horizontal FOV
            //Formula: ((OverlapPercent/100) * hFOV)/2
            private void SetEyeRotation(float overlapPercent, float horizontalFov)
            {
                float overlap = overlapPercent* .01f * horizontalFov * 0.5f;
                
                //with a 90 degree FOV with 100% overlap, the eyes should not be rotated
                //compare rotationY with half of FOV

                float halfFOV = horizontalFov * 0.5f;
                float rotateYAmount = Mathf.Abs(overlap - halfFOV);

                foreach (VREye currentEye in GetComponentsInChildren<VREye>())
                {
                    switch (currentEye.eye)
                    {
                        case Eye.left:
                            _leftEye.SetEyeRotationY(-rotateYAmount);
                            break;
                        case Eye.right:
                            _rightEye.SetEyeRotationY(rotateYAmount);
                            break;
                    }
                }
            }
            //rotate each eye on the z axis by the specified amount, in degrees
            private void SetEyeRoll(float leftRoll, float rightRoll)
            {
                foreach (VREye currentEye in GetComponentsInChildren<VREye>())
                {
                    switch (currentEye.eye)
                    {
                        case Eye.left:
                            _leftEye.SetEyeRoll(leftRoll);
                            break;
                        case Eye.right:
                            _rightEye.SetEyeRoll(rightRoll);
                            break;
                    }
                }
            }
            #endregion
        }
    }
}
