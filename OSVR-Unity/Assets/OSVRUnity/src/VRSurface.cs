using UnityEngine;
using System.Collections;
namespace OSVR
{
    namespace Unity
    {
        public class VRSurface : MonoBehaviour
        {

            private Camera _camera;
            public Camera Camera { get { return _camera; } set { _camera = value; } }

            public void SetViewport(Rect rect)
            {
                _camera.rect = rect;
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
