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
            #region Public Variables
            public Eye eye;

            private Camera _camera;
            public Camera Camera { get { return _camera; } }

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
                _camera.CopyFrom(sourceCamera);
                SetViewportRects();
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
                //cache:
                cachedTransform = transform;

                if (_camera == null)
                {
                    _camera = gameObject.AddComponent<Camera>();
                }

                SetViewportRects();
            }

            //helper method to set correct viewport for each eye
            private void SetViewportRects()
            {
                //camera setups:
                switch (eye)
                {
                    case Eye.left:
                        _camera.rect = new Rect(0, 0, .5f, 1);
                        break;
                    case Eye.right:
                        _camera.rect = new Rect(.5f, 0, .5f, 1);
                        break;
                }
            }
            
            #endregion

            
        }
    }
}
