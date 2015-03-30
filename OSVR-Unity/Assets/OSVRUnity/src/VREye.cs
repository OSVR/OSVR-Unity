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
        
        public enum Eye { left, right };

        public class VREye : MonoBehaviour
        {
            #region Private Variables
            private Camera _camera;
            private ClientKit clientKit;
            #endregion
            #region Public Variables
            public Eye eye;
            public Camera Camera { get { return _camera; } set { _camera = value; } }
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
            public void MatchCamera(Camera sourceCamera)
            {
                Camera.CopyFrom(sourceCamera);
                //copy the distortion shader
                CopyDistortionShader(sourceCamera.GetComponent<OsvrDistortion>(), this.gameObject);
                SetViewportRects();
                
            }
            //this function copies the distortion shader from the original Shader attached to VRHead
            private void CopyDistortionShader(OsvrDistortion original, GameObject destination)
            {
                OsvrDistortion d = destination.AddComponent<OsvrDistortion>();
                d.distortionShader = original.distortionShader;
                d.k1Red = original.k1Red;
                d.k1Blue = original.k1Blue;
                d.k1Green = original.k1Green;
                d.leftCenter = original.leftCenter;
                d.rightCenter = original.rightCenter;
                d.fullCenter = original.fullCenter;
                d.enabled = true;
            }

            //rotate each eye outward
            public void SetEyeRotationY(float y)
            {
                cachedTransform.Rotate(0, y, 0, Space.Self);
            }
            //set the z rotation of the eye
            public void SetEyeRoll(float rollAmount)
            {
                cachedTransform.Rotate(0, 0, rollAmount, Space.Self);
            }
            #endregion

            #region Private Methods
            void Init()
            {
                if(clientKit == null)
                {
                    clientKit = GameObject.FindObjectOfType<ClientKit>();
                }
                //cache:
                cachedTransform = transform;

                if (Camera == null)
                {
                    if ((Camera = GetComponent<Camera>()) == null)
                    {
                        Camera = gameObject.AddComponent<Camera>();
                    }
                }

                SetViewportRects();
            }

            //helper method to set correct viewport for each eye
            private void SetViewportRects()
            {
                if (Camera == null)
                {
                    Init();
                }
                //camera setups:
                switch (eye)
                {
                    case Eye.left:
                        Camera.rect = new Rect(0, 0, .5f, 1);
                        break;
                    case Eye.right:
                        Camera.rect = new Rect(.5f, 0, .5f, 1);
                        break;
                }
            }

            //Called after a camera finishes rendering the scene.
            //the goal here is to update the client often to make sure we have the most recent tracker data
            //this helps reduce latency
            void OnPostRender()
            {
                clientKit.context.update();
            }
            #endregion

            
        }
    }
}
