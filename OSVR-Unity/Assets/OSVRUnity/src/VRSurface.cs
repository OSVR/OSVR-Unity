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
            private RenderTexture rt;
            public RenderTexture GetRenderTexture { get { return rt; } }

            private Texture2D tex2D;
            public Texture2D GetTex2D { get { return tex2D; } }

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
            public void PostRender(bool vrMode)
            {
                
            }

            public void SetRenderTexture(RenderTexture renderTexture)
            {
                rt = renderTexture;
                tex2D = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
                Camera.targetTexture = rt;
            }
        }
    }
}
