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
        public class VRViewer : MonoBehaviour
        {
            #region Public Variables
            public Camera Camera 
            { 
                get 
                { 
                    if(_camera == null)
                    {
                        _camera = GetComponent<Camera>();
                    }
                    return _camera; 
                } 
                set { _camera = value; } 
            }
            public DisplayController DisplayController { get { return _displayController; } set { _displayController = value; } }
            [HideInInspector]
            public Transform cachedTransform;
            #endregion

            #region Private Variables
            private DisplayController _displayController;
            private Camera _camera;
            private bool _disabledCamera = true;
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

            void OnEnable()
            {
                StartCoroutine("EndOfFrame");
            }

            void OnDisable()
            {
                StopCoroutine("EndOfFrame");
            }

            //Updates the position and rotation of the head
            public void UpdateViewerHeadPose(OSVR.ClientKit.Pose3 headPose)
            {
                cachedTransform.localPosition = Math.ConvertPosition(headPose.translation);
                cachedTransform.localRotation = Math.ConvertOrientation(headPose.rotation);
            }

            //Culling determines which objects are visible to the camera. OnPreCull is called just before this process.
            void OnPreCull()
            {
                //update the client
                _displayController.UpdateClient();

                // Disable dummy camera during rendering
                // Enable after frame ends
                _camera.enabled = false;

                //update the viewer's head pose
                UpdateViewerHeadPose(_displayController.DisplayConfig.GetViewerPose(DisplayController.DEFAULT_VIEWER));

                //render each eye camera (each surface)
                //assumes one surface per eye
                for (int i = 0; i < _displayController.EyeCount; i++)
                {
                    //update the eye pose
                    VREye eye = _displayController.Eyes[i];
                    eye.UpdateEyePose(_displayController.DisplayConfig.GetViewerEyePose(DisplayController.DEFAULT_VIEWER, (byte)i));

                    //get the eye's surface
                    VRSurface surface = eye.Surface;

                    //get viewport from ClientKit and set surface viewport
                    OSVR.ClientKit.Viewport viewport = _displayController.DisplayConfig.GetRelativeViewportForViewerEyeSurface(
                        DisplayController.DEFAULT_VIEWER, (byte)i, DisplayController.DEFAULT_SURFACE);
                    
                    surface.SetViewport(Math.ConvertViewport(viewport));
                    
                    //get projection matrix from ClientKit and set surface projection matrix
                    OSVR.ClientKit.Matrix44f projMatrix = _displayController.DisplayConfig.GetProjectionMatrixForViewerEyeSurfacef(
                        DisplayController.DEFAULT_VIEWER, (byte)i, DisplayController.DEFAULT_SURFACE,
                        _camera.nearClipPlane, _camera.farClipPlane, OSVR.ClientKit.MatrixConventionsFlags.ColMajor);
                    
                    surface.SetProjectionMatrix(Math.ConvertMatrix(projMatrix));
                    
                    //render the surface
                    surface.Render();
                }

                // Remember to reenable.
                _disabledCamera = true;
            }

            IEnumerator EndOfFrame()
            {
                while (true)
                {
                    //if we disabled the dummy camera, enable it here
                    if (_disabledCamera)
                    {
                        Camera.enabled = true;
                        _disabledCamera = false;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}
