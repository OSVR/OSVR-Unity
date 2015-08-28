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
                if (!_displayController.IsInitialized) return;
                _displayController.UpdateClient();

                // Turn off the mono camera so it doesn't waste time rendering.
                // @note mono camera is left on from beginning of frame till now
                // in order that other game logic (e.g. Camera.main) continues
                // to work as expected.
                _camera.enabled = true;

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
                        _camera.nearClipPlane, _camera.farClipPlane, OSVR.ClientKit.MatrixConventionsFlags.ColMajor);
                    
                    surface.SetProjectionMatrix(Math.ConvertMatrix(projMatrix));
                    if (DisplayController.SupportsRenderManager() && DisplayController.IsInitialized)
                    {
                        surface.Render();
                    }
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
                    _displayController.RenderManager.SetRenderEventTime(Time.time);
                    GL.IssuePluginEvent(0);
                   /* if(DisplayController.SupportsRenderManager() && DisplayController.IsInitialized && DisplayController.RtSet)
                    {
                        DisplayController.RenderManager.SetRenderEventTime(Time.time);
                        for (int i = 0; i < _displayController.EyeCount; i++)
                        {
                            //get the eye's surface
                            VRSurface surface = _displayController.Eyes[i].Surface;

                            // Remember current render textures
                            RenderTexture currentActiveRT = RenderTexture.active;
                            RenderTexture currentCamRT = surface.Camera.targetTexture;

                            // Force rendering of the camera to my render texture
                            surface.Camera.targetTexture = surface.GetRenderTexture;
                            surface.Camera.Render(); // 2nd Render seems to be necessary, but why??
                            surface.Camera.targetTexture = currentCamRT;

                            // Get a copy of the rendered data
                            RenderTexture.active = surface.GetRenderTexture;
                            surface.GetTex2D.ReadPixels(new Rect(0, 0, surface.GetRenderTexture.width*2, surface.GetRenderTexture.height), 0, 0);
                            surface.GetTex2D.Apply(); // hits perf significantly but needed otherwise actual copy does not occur

                            // Restorie previously assigned render texture
                            RenderTexture.active = currentActiveRT;
                            //Destroy(rt);
                            
                        }
                        

                        // Issue a plugin event with arbitrary integer identifier.
                        // The plugin can distinguish between different
                        // things it needs to do based on this ID.
                        // For our simple plugin, it does not matter which ID we pass here.
                        //DisplayController.RenderManager.SetRenderEventTime(Time.time);
                        
                }*/
                    
                }
            }
        }
    }
}
