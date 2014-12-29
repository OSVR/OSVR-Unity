/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */
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
            #endregion
        }
    }
}