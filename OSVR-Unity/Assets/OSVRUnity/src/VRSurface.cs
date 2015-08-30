/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2015 Sensics, Inc.
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
/// Author: Greg Aring
/// Email: greg@sensics.com
/// </summary>
/// 
using UnityEngine;
using System.Collections;
namespace OSVR
{
    namespace Unity
    {
        //This class is responsible for rendering for a VREye.
        public class VRSurface : MonoBehaviour
        {
            private Camera _camera;
            private K1RadialDistortion _distortionEffect;

            public Camera Camera { get { return _camera; } set { _camera = value; } }
            private RenderTexture _renderTarget;
            public RenderTexture RenderTarget { get { return _renderTarget; } set { _renderTarget = value; } }

            private Texture2D _textureToNative;
            public Texture2D TextureToNative { get { return _textureToNative; } set { _textureToNative = value; } }

            [HideInInspector]
            public K1RadialDistortion DistortionEffect
            {
                get
                {
                    if (!_distortionEffect)
                    {
                        _distortionEffect = GetComponent<K1RadialDistortion>();
                    }
                    return _distortionEffect;
                }
                set
                {
                    _distortionEffect = value;
                }
            }


            public void SetViewport(Rect rect)
            {
                _camera.rect = rect;
            }

            public void SetViewMatrix(Matrix4x4 viewMatrix)
            {
                _camera.worldToCameraMatrix = viewMatrix;
            }

            public void SetProjectionMatrix(Matrix4x4 projMatrix)
            {
                _camera.projectionMatrix = projMatrix;
            }

            public void Render()
            {
                _camera.Render();
            }

            void OnPostRender()
            {
                //GL.IssuePluginEvent(0); 
            }

            public void SetRenderTexture(RenderTexture renderTexture)
            {
                _renderTarget = renderTexture;
                Camera.targetTexture = _renderTarget;

                //also create a texture2d of the same size
                _textureToNative = new Texture2D(_renderTarget.width, _renderTarget.height, TextureFormat.ARGB32, false);
                RenderTexture.active = _renderTarget;
                _textureToNative.ReadPixels(new Rect(0, 0, _renderTarget.width, _renderTarget.height), 0, 0);
                _textureToNative.Apply();
                
               // Camera.targetTexture = rt;
            }
        }
    }
}
