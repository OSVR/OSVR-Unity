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
                set { _camera = value; } }
            public DisplayController DisplayController { get { return _displayController; } set { _displayController = value; } }
            #endregion

            #region Private Variables
            private DisplayController _displayController;
            private Camera _camera;
            private bool renderedStereo = true;
            private bool updated = false; //whether the headpose has been updated this frame
            private bool updateEarly = false; //if false, update in LateUpdate
            #endregion

            void OnEnable()
            {
                StartCoroutine("EndOfFrame");
            }

            void OnDisable()
            {
                StopCoroutine("EndOfFrame");
            }

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

            void OnPreCull()
            {
                _displayController.UpdateClient();

                // Turn off the mono camera so it doesn't waste time rendering.
                // @note mono camera is left on from beginning of frame till now
                // in order that other game logic (e.g. Camera.main) continues
                // to work as expected.
                _camera.enabled = false;

                float near = 0.1f;
                float far = 1000f;

                //render each eye camera (each surface)
                //assumes one surface per eye
                //@todo cache eyes, eyecount?
                for (int i = 0; i < _displayController.EyeCount; i++)
                {
                    //get the eye's surface
                    VRSurface surface = _displayController.Eyes[i].Surface;
                    
                    //get viewport from ClientKit
                    OSVR.ClientKit.Viewport viewport = _displayController.DisplayConfig.GetRelativeViewportForViewerEyeSurface(
                        DisplayController.DEFAULT_VIEWER, (byte)i, DisplayController.DEFAULT_SURFACE);
                    
                    surface.SetViewport(Math.ConvertViewport(viewport));
                    
                    //get projection matrix from ClientKit
                    OSVR.ClientKit.Matrix44f projMatrix = _displayController.DisplayConfig.GetProjectionMatrixForViewerEyeSurfacef(
                        DisplayController.DEFAULT_VIEWER, (byte)i, DisplayController.DEFAULT_SURFACE,
                        near, far, OSVR.ClientKit.MatrixConventionsFlags.ColMajor);
                    
                    surface.SetProjectionMatrix(Math.ConvertMatrix(projMatrix));
                    //surface.Render();
                }

                // Remember to reenable.
                renderedStereo = true;
            }

            IEnumerator EndOfFrame()
            {
                while (true)
                {
                    // If *we* turned off the mono cam, turn it back on for next frame.
                    if (renderedStereo)
                    {
                        Camera.enabled = true;
                        renderedStereo = false;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}
