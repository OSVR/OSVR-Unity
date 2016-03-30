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
using System;

namespace OSVR
{
    namespace Unity
    {
        public class VREye : MonoBehaviour
        {
            public const int NUM_SURFACES = 1;

            #region Private Variables           
            private VRViewer _viewer; //the viewer associated with this eye
            private VRSurface[] _surfaces; //the surfaces associated with this eye
            private uint _surfaceCount;
            private uint _eyeIndex;
            
            
            #endregion
            #region Public Variables  
            public uint EyeIndex
            {
                get { return _eyeIndex; }
                set { _eyeIndex = value; }
            }
            public VRSurface[] Surfaces { get { return _surfaces; } } 
            public uint SurfaceCount { get { return _surfaceCount; } }
            public VRViewer Viewer
            {
                get { return _viewer; }
                set { _viewer = value; }
            }
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
            #endregion

            #region Private Methods
            void Init()
            {
                //cache:
                cachedTransform = transform;
            }         
            #endregion

            // Updates the position and rotation of the eye
            // Optionally, update the viewer associated with this eye
            public void UpdateEyePose(OSVR.ClientKit.Pose3 eyePose)
            {
                // Convert from OSVR space into Unity space.
                Vector3 pos = Math.ConvertPosition(eyePose.translation);
                Quaternion rot = Math.ConvertOrientation(eyePose.rotation);

                // RenderManager produces the eyeFromSpace matrix, but
                // Unity wants the inverse of that.
                if (Viewer.DisplayController.UseRenderManager)
                {
                    // Invert the transformation
                    cachedTransform.localRotation = Quaternion.Inverse(rot);
                    Vector3 invPos = -pos;
                    cachedTransform.localPosition = Quaternion.Inverse(rot) * invPos;
                }
                else
                {
                    cachedTransform.localPosition = pos;
                    cachedTransform.localRotation = rot;
                }
            }

            //For each Surface, update viewing parameters and render the surface
            public void UpdateSurfaces()
            {
                //for each surface
                for (uint surfaceIndex = 0; surfaceIndex < SurfaceCount; surfaceIndex++)
                {
                    //get the eye's surface
                    VRSurface surface = Surfaces[surfaceIndex];

                    OSVR.ClientKit.Viewport viewport;
                    OSVR.ClientKit.Matrix44f projMatrix;

                    //get viewport from ClientKit and set surface viewport
                    if (Viewer.DisplayController.UseRenderManager)
                    {
                        viewport = Viewer.DisplayController.RenderManager.GetEyeViewport((int)EyeIndex);
                        surface.SetViewportRect(Math.ConvertViewportRenderManager(viewport));

                        //get projection matrix from RenderManager and set surface projection matrix
                        surface.SetProjectionMatrix(Viewer.DisplayController.RenderManager.GetEyeProjectionMatrix((int)EyeIndex));
                   
                        surface.Render();
                    }
                    else
                    {
                        //get viewport from ClientKit and set surface viewport
                        viewport = Viewer.DisplayController.DisplayConfig.GetRelativeViewportForViewerEyeSurface(
                            Viewer.ViewerIndex, (byte)_eyeIndex, surfaceIndex);

                        int displayInputIndex = Viewer.DisplayController.DisplayConfig.GetViewerEyeSurfaceDisplayInputIndex(Viewer.ViewerIndex, (byte)_eyeIndex, surfaceIndex);
                        int numDisplayInputs = Viewer.DisplayController.DisplayConfig.GetNumDisplayInputs();
                        surface.SetViewportRect(Math.ConvertViewport(viewport, Viewer.DisplayController.DisplayConfig.GetDisplayDimensions((byte)displayInputIndex),
                            numDisplayInputs, (int)_eyeIndex, (int)Viewer.DisplayController.TotalDisplayWidth));

                        //get projection matrix from ClientKit and set surface projection matrix
                        projMatrix = Viewer.DisplayController.DisplayConfig.GetProjectionMatrixForViewerEyeSurfacef(
                            Viewer.ViewerIndex, (byte)_eyeIndex, surfaceIndex,
                            surface.Camera.nearClipPlane, surface.Camera.farClipPlane, OSVR.ClientKit.MatrixConventionsFlags.ColMajor);

                        surface.SetProjectionMatrix(Math.ConvertMatrix(projMatrix));

                        //render the surface
                        surface.Render();
                    }                           

                }
            }

            public void ClearSurfaces()
            {
                //for each surface
                for (uint surfaceIndex = 0; surfaceIndex < SurfaceCount; surfaceIndex++)
                {
                    //get the eye's surface
                    VRSurface surface = Surfaces[surfaceIndex];
                    surface.ClearRenderTarget();
                }
            }


            //Create this Eye's VRSurfaces. 
            //Each VRSurface has a camera component which controls rendering for the VREye
            public void CreateSurfaces(uint surfaceCount)
            {
                _surfaceCount = surfaceCount;
                _surfaces = new VRSurface[_surfaceCount];
                if (surfaceCount != NUM_SURFACES)
                {
                    Debug.LogError("[OSVR-Unity] Eye" + _eyeIndex + " has " + surfaceCount + " surfaces, but " +
                        "this implementation requires exactly one surface per eye.");
                    return;
                }

                uint surfaceIndex = 0;
                uint foundSurfaces = 0;

                //Check if there are already VRSurfaces in the hierarchy.
                //If so, use them instead of creating a new gameobjects
                VRSurface[] eyeSurfaces = GetComponentsInChildren<VRSurface>();
                foundSurfaces = (uint)eyeSurfaces.Length;
                if (eyeSurfaces != null && foundSurfaces > 0)
                {
                    for (surfaceIndex = 0; surfaceIndex < eyeSurfaces.Length; surfaceIndex++)
                    {
                        VRSurface surface = eyeSurfaces[surfaceIndex];
                        // get the VRSurface gameobject
                        GameObject surfaceGameObject = surface.gameObject;
                        surfaceGameObject.name = "VRSurface" + surfaceIndex;
                        surface.Eye = this;
                        surface.Camera = surfaceGameObject.GetComponent<Camera>(); //VRSurface has camera component by default
                        //don't copy from the main camera if the VRSurface already existed before runtime
                        //CopyCamera(Viewer.Camera, surface.Camera); //copy camera properties from the "dummy" camera to surface camera
                        surface.Camera.enabled = false; //disabled so we can control rendering manually
                        surface.SurfaceIndex = surfaceIndex; //set the surface index
                        surfaceGameObject.transform.localPosition = Vector3.zero;
                        surfaceGameObject.transform.rotation = this.transform.rotation;
                        Surfaces[surfaceIndex] = surface;

                        //distortion
                        bool useDistortion = Viewer.DisplayController.DisplayConfig.DoesViewerEyeSurfaceWantDistortion(Viewer.ViewerIndex, (byte)_eyeIndex, surfaceIndex);
                        if (useDistortion)
                        {
                            //@todo figure out which type of distortion to use
                            //right now, there is only one option, SurfaceRadialDistortion
                            //get distortion parameters
                            OSVR.ClientKit.RadialDistortionParameters distortionParameters =
                            Viewer.DisplayController.DisplayConfig.GetViewerEyeSurfaceRadialDistortion(
                            Viewer.ViewerIndex, (byte)_eyeIndex, surfaceIndex);                    
                            surface.SetDistortion(distortionParameters);
                        }

                        //render manager
                        if (Viewer.DisplayController.UseRenderManager)
                        {
                            surface.SetViewport(Viewer.DisplayController.RenderManager.GetEyeViewport((int)EyeIndex));

                            //create a RenderTexture for this eye's camera to render into
                            RenderTexture renderTexture = new RenderTexture(surface.Viewport.Width, surface.Viewport.Height, 24, RenderTextureFormat.Default);
                            if (QualitySettings.antiAliasing > 0)
                            {
                                renderTexture.antiAliasing = QualitySettings.antiAliasing;
                            }
                            surface.SetRenderTexture(renderTexture);
                        }
                    }
                }


                //loop through surfaces because at some point we could support eyes with multiple surfaces
                //but this implementation currently supports exactly one
                for (; surfaceIndex < surfaceCount; surfaceIndex++)
                {
                    GameObject surfaceGameObject = new GameObject("VRSurface" + surfaceIndex);
                    VRSurface surface = surfaceGameObject.AddComponent<VRSurface>();
                    surface.Eye = this;
                    surface.Camera = surfaceGameObject.GetComponent<Camera>(); //VRSurface has camera component by default
                    CopyCamera(Viewer.Camera, surface.Camera); //copy camera properties from the "dummy" camera to surface camera
                    surface.Camera.enabled = false; //disabled so we can control rendering manually
                    surface.SurfaceIndex = surfaceIndex; //set the surface index
                    surfaceGameObject.transform.parent = this.transform; //surface is child of Eye
                    surfaceGameObject.transform.localPosition = Vector3.zero;
                    Surfaces[surfaceIndex] = surface;

                    //distortion
                    bool useDistortion = Viewer.DisplayController.DisplayConfig.DoesViewerEyeSurfaceWantDistortion(Viewer.ViewerIndex, (byte)_eyeIndex, surfaceIndex);
                    if(useDistortion)
                    {
                        //@todo figure out which type of distortion to use
                        //right now, there is only one option, SurfaceRadialDistortion
                        //get distortion parameters
                        OSVR.ClientKit.RadialDistortionParameters distortionParameters =
                        Viewer.DisplayController.DisplayConfig.GetViewerEyeSurfaceRadialDistortion(
                        Viewer.ViewerIndex, (byte)_eyeIndex, surfaceIndex);

                        surface.SetDistortion(distortionParameters);
                    }    
                    
                    //render manager
                    if(Viewer.DisplayController.UseRenderManager)
                    {
                        //Set the surfaces viewport from RenderManager
                        surface.SetViewport(Viewer.DisplayController.RenderManager.GetEyeViewport((int)EyeIndex));

                        //create a RenderTexture for this eye's camera to render into
                        RenderTexture renderTexture = new RenderTexture(surface.Viewport.Width, surface.Viewport.Height, 24, RenderTextureFormat.Default);
                        if (QualitySettings.antiAliasing > 0)
                        {
                            renderTexture.antiAliasing = QualitySettings.antiAliasing;
                        }
                        surface.SetRenderTexture(renderTexture);                       
                    }             
                }
            }

            //helper method that copies camera properties from one camera to another
            //copies from srcCamera to destCamera
            private void CopyCamera(Camera srcCamera, Camera destCamera)
            {
                //Copy the camera properties.
                destCamera.CopyFrom(srcCamera);
                destCamera.depth = 0;
                //@todo Copy other components attached to the DisplayController?
            }           
        }
    }
}
