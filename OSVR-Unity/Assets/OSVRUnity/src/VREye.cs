/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */
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

                /*camera.nearClipPlane = sourceCamera.nearClipPlane;
                camera.farClipPlane = sourceCamera.farClipPlane;
                camera.backgroundColor = sourceCamera.backgroundColor;
                camera.clearFlags = sourceCamera.clearFlags;
                camera.cullingMask = sourceCamera.cullingMask;*/
                camera.CopyFrom(sourceCamera);
                SetViewportRects();
            }

            
            #endregion

            #region Private Methods
            void Init()
            {
                //cache:
                cachedTransform = transform;

                if (camera == null) gameObject.AddComponent<Camera>();

                SetViewportRects();
            }

            //helper method to set correct viewport for each eye
            private void SetViewportRects()
            {
                //camera setups:
                switch (eye)
                {
                    case Eye.left:
                        camera.rect = new Rect(0, 0, .5f, 1);
                        break;
                    case Eye.right:
                        camera.rect = new Rect(.5f, 0, .5f, 1);
                        break;
                }
            }
            #endregion
        }
    }
}