using UnityEngine;
using System.Collections;
namespace OSVR
{
    namespace Unity
    {
        public class VRSurface : MonoBehaviour
        {

            private Camera _camera;
            private K1RadialDistortion _distortionEffect;

            public Camera Camera { get { return _camera; } set { _camera = value; } }

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

        }
    }
}
