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
using UnityEngine.Rendering;
using System.Collections;
using System.Runtime.InteropServices;

namespace OSVR
{
    namespace Unity
    {
        //This class is responsible for rendering for a VREye.
        [RequireComponent(typeof(Camera))]
        public class VRSurface : MonoBehaviour
        {
            private Camera _camera;
            private K1RadialDistortion _distortionEffect;
            private uint _surfaceIndex; //index in the eye's VRSurface array
            private VREye _eye; //the eye that this surface controls rendering for
            private OSVR.ClientKit.Viewport _viewport;
            [HideInInspector]
            public Texture2D PluginTexture;
            [HideInInspector]
            public RenderTexture RenderToTexture;
            private Color[] m_Pixels;
            private GCHandle m_PixelsHandle;
            private Shader basicShader;

            public Camera Camera { get { return _camera; } set { _camera = value; } }
            public uint SurfaceIndex { get { return _surfaceIndex; } set { _surfaceIndex = value; } }
            public VREye Eye { get { return _eye; } set { _eye = value; } }
            public OSVR.ClientKit.Viewport Viewport { get { return _viewport;} set {_viewport = value;} }

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

            //Set the camera's viewport rect
            public void SetViewportRect(Rect rect)
            {
                _camera.rect = rect;
            }

            //Set the camera's viewport rect
            public void SetViewport(OSVR.ClientKit.Viewport viewport)
            {
                Viewport = viewport;
            }

            //Set the camera's view matrix
            public void SetViewMatrix(Matrix4x4 viewMatrix)
            {
                _camera.worldToCameraMatrix = viewMatrix;
            }

            //Set the camera's projection matrix
            public void SetProjectionMatrix(Matrix4x4 projMatrix)
            {
                _camera.projectionMatrix = projMatrix;
            }

            //Given distortion parameters, setup the appropriate distortion method
            //@todo this should be more generalized when we have more distortion options
            public void SetDistortion(OSVR.ClientKit.RadialDistortionParameters distortionParameters)
            {
                float k1Red = (float)distortionParameters.k1.x;
                float k1Green = (float)distortionParameters.k1.y;
                float k1Blue = (float)distortionParameters.k1.z;
                Vector2 center = new Vector2((float)distortionParameters.centerOfProjection.x,
                    (float)distortionParameters.centerOfProjection.y);

                //@todo figure out which type of distortion to use
                //right now, there is only one option
                SetK1RadialDistortion(k1Red, k1Green, k1Blue, center);
            }

            //set distortion parameters for K1 Radial Distortion method
            private void SetK1RadialDistortion(float k1Red, float k1Green, float k1Blue, Vector2 center)
            {
                // disable distortion if there is no distortion for this HMD
                if (k1Red == 0 && k1Green == 0 && k1Blue == 0)
                {
                    if (DistortionEffect)
                    {
                        DistortionEffect.enabled = false;
                    }
                    return;
                }
                // Otherwise try to create distortion and set its parameters
                var distortionFactory = new K1RadialDistortionFactory();
                var effect = distortionFactory.GetOrCreateDistortion(this);
                if (effect)
                {
                    effect.k1Red = k1Red;
                    effect.k1Green = k1Green;
                    effect.k1Blue = k1Blue;
                    effect.center = center;
                }
            }

            //set the render texture that this camera will render into
            //pass the native hardware pointer to the UnityRenderingPlugin for use in RenderManager
            public void SetRenderTexture(RenderTexture rt)
            {
                RenderToTexture = rt;
                Camera.targetTexture = RenderToTexture;
                RenderTexture.active = RenderToTexture;
                
                //Set the native texture pointer so we can access this texture from the plugin
                Eye.Viewer.DisplayController.RenderManager.SetEyeColorBuffer(RenderToTexture.GetNativeTexturePtr(), (int)Eye.EyeIndex);
            }
            public RenderTexture GetRenderTexture()
            {
                return Camera.targetTexture;
            }
            public void SetActiveRenderTexture()
            {
                RenderTexture.active = RenderToTexture;
            }

            //Render the camera
            public void Render()
            {
                Camera.targetTexture = RenderToTexture;
                Camera.Render();
            }

            public void ClearRenderTarget()
            {
                if (RenderTexture.active != null)
                {
                    RenderTexture.active.DiscardContents();
                    RenderTexture.active = null;
                    GL.InvalidateState();
                }
            }
        }
    }
}
