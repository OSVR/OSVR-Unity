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

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System;

namespace OSVR
{
    namespace Unity
    {
        //*This class is a wrapper for the OSVR-Unity Rendering Plugin osvrUnityRenderingPlugin.dll,
        // which brings in functionality from the OSVR RenderManager project. RenderManager features inculde:
        // - DirectMode -- compatible with nVidia cards with a driver that has been modified to white-list the display that you are using.
        // - TimeWarp
        // - Distortion Correction
        //
        // osvrUnityRenderingPlugin.dll, osvrRenderManager.dll, SDL2.dll, and glew32.dll must be in the Plugins/x86 or x64 folders.
        // Requires Unity 5.2+
        //*/
        public class OsvrRenderManager : MonoBehaviour
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct OSVR_ProjectionMatrix
            {
                public double left;
                public double right;
                public double top;
                public double bottom;
                public double nearClip;        //< Cannot name "near" because Visual Studio keyword
                public double farClip;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct OSVR_ViewportDescription
            {
                public double left;    //< Left side of the viewport in pixels
                public double lower;   //< First pixel in the viewport at the bottom.
                public double width;   //< Last pixel in the viewport at the top
                public double height;   //< Last pixel on the right of the viewport in pixels
            }

            public const int RENDER_EVENT = 0;
            public const int SHUTDOWN_EVENT = 1;
            public const int UPDATE_RENDERINFO_EVENT = 2;
            private const string PluginName = "osvrUnityRenderingPlugin";

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            private delegate void DebugLog(string log);
            private static readonly DebugLog debugLog = DebugWrapper;
            private static readonly IntPtr functionPointer = Marshal.GetFunctionPointerForDelegate(debugLog);
            private static void DebugWrapper(string log) { Debug.Log(log); }

            //Create and Register RenderBuffers
            [DllImport(PluginName)]
            private static extern Byte 
                ConstructRenderBuffers();

            //Create a RenderManager object in the plugin, passing in a ClientContext
            [DllImport(PluginName)]
            private static extern Byte
                CreateRenderManagerFromUnity(OSVR.ClientKit.SafeClientContextHandle /*OSVR_ClientContext*/ ctx);

            [DllImport(PluginName)]
            private static extern OSVR.ClientKit.Pose3
                GetEyePose(int eye);

            [DllImport(PluginName)]
            private static extern OSVR_ProjectionMatrix
                GetProjectionMatrix(int eye);

            //get the render event function that we'll call every frame via GL.IssuePluginEvent
            [DllImport(PluginName)]
            private static extern IntPtr
                GetRenderEventFunc();

            [DllImport(PluginName)]
            private static extern OSVR_ViewportDescription
                GetViewport(int eye);

            // Allow for calling into the debug console from C++
            [DllImport(PluginName)]
            private static extern void
                LinkDebug([MarshalAs(UnmanagedType.FunctionPtr)]IntPtr debugCal);

            // OnRenderEvent is not needed

            // Pass a pointer to a texture (RenderTexture.GetNativeTexturePtr()) to the plugin
            // @todo native code may change the return type to OSVR_ReturnCode.
            // If so, change the return type here to Byte
            [DllImport(PluginName)]
            private static extern int 
                SetColorBufferFromUnity(System.IntPtr texturePtr, int eye);

            [DllImport(PluginName)]
            private static extern void
                SetFarClipDistance(double farClipPlaneDistance);

            [DllImport(PluginName)]
            private static extern void
                SetIPD(double ipdMeters);

            [DllImport(PluginName)]
            private static extern void
                SetNearClipDistance(double nearClipPlaneDistance);

            [DllImport(PluginName)]
            private static extern void
                ShutdownRenderManager();

            // UnityPluginLoad is not needed
            // UnityPluginUnload is not needed

            private bool _linkDebug = false; //causes crash on exit if true, only enable for debugging

            //persistent singleton
            private static OsvrRenderManager _instance;

            /// <summary>
            /// Use to access the single instance of this object/script in your game.
            /// </summary>
            /// <returns>The instance, or null in case of error</returns>
            public static OsvrRenderManager instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = GameObject.FindObjectOfType<OsvrRenderManager>();
                        if (_instance == null)
                        {
                            Debug.LogError("[OSVR-Unity] RenderManager not found.");
                        }
                        else
                        {
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                    return _instance;
                }
            }

            void Awake()
            {
                //if an instance of this singleton does not exist, set the instance to this object and make it persist
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this);
                }
                else
                {
                    //if an instance of this singleton already exists, destroy this one
                    if (_instance != this)
                    {
                        Destroy(this.gameObject);
                    }
                }
            }

            void OnDisable()
            {
                ExitRenderManager();
            }

            //Initialize use of RenderManager via CreateRenderManager call
            public int InitRenderManager()
            {
                if (_linkDebug)
                {
                    //this will cause a crash when exiting the Unity editor or an application
                    //only use for debugging purposes, do not leave on for release.
                    LinkDebug(functionPointer); // Hook our c++ plugin into Unity's console log.
                }

                return CreateRenderManager(ClientKit.instance.context);
            }

            //Create and Register RenderBuffers in RenderManager
            //Called after RM is created and after Unity RenderTexture's are created and assigned via SetEyeColorBuffer
            public int ConstructBuffers()
            {
                return ConstructRenderBuffers();
            }

            public void SetNearClippingPlaneDistance(float near)
            {
                SetNearClipDistance((double)near);
            }

            public void SetFarClippingPlaneDistance(float far)
            {
                SetFarClipDistance((double)far);
            }

            public void SetIPDMeters(float ipd)
            {
                SetIPD((double)ipd);
            }

            //"Recenter" based on current head orientation
            public void SetRoomRotationUsingHead()
            {
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
                ClientKit.instance.context.SetRoomRotationUsingHead();
                GL.IssuePluginEvent(GetRenderEventFunc(), 3);
#endif
            }

            //Clear the room-to-world transform, undo a call to SetRoomRotationUsingHead
            public void ClearRoomToWorldTransform()
            {
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
                ClientKit.instance.context.ClearRoomToWorldTransform();
                GL.IssuePluginEvent(GetRenderEventFunc(), 4);
#endif
            }

            //Get the pose of a given eye from RenderManager
            public OSVR.ClientKit.Pose3 GetRenderManagerEyePose(int eye)
            {
                return GetEyePose(eye);
            }

            //Get the viewport of a given eye from RenderManager
            public OSVR.ClientKit.Viewport GetEyeViewport(int eye)
            {
                OSVR.ClientKit.Viewport v = new OSVR.ClientKit.Viewport();
                OSVR_ViewportDescription viewportDescription = GetViewport(eye);
                v.Left = (int)viewportDescription.left;
                v.Bottom = (int)viewportDescription.lower;
                v.Width = (int)viewportDescription.width;
                v.Height = (int)viewportDescription.height;
                return v;
            }

            //Get the projection matrix of a given eye from RenderManager
            public Matrix4x4 GetEyeProjectionMatrix(int eye)
            {
                OSVR_ProjectionMatrix pm = GetProjectionMatrix(eye);
                return PerspectiveOffCenter((float)pm.left, (float)pm.right, (float)pm.bottom, (float)pm.top, (float)pm.nearClip, (float)pm.farClip);
                
            }

            //Returns a Unity Matrix4x4 from the provided boundaries
            //from http://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html
            static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
            {
                float x = 2.0F * near / (right - left);
                float y = 2.0F * near / (top - bottom);
                float a = (right + left) / (right - left);
                float b = (top + bottom) / (top - bottom);
                float c = -(far + near) / (far - near);
                float d = -(2.0F * far * near) / (far - near);
                float e = -1.0F;
                Matrix4x4 m = new Matrix4x4();
                m[0, 0] = x;
                m[0, 1] = 0;
                m[0, 2] = a;
                m[0, 3] = 0;
                m[1, 0] = 0;
                m[1, 1] = y;
                m[1, 2] = b;
                m[1, 3] = 0;
                m[2, 0] = 0;
                m[2, 1] = 0;
                m[2, 2] = c;
                m[2, 3] = d;
                m[3, 0] = 0;
                m[3, 1] = 0;
                m[3, 2] = e;
                m[3, 3] = 0;
                return m;
            }

            //Call the Unity Rendering Plugin to initialize the RenderManager
            public int CreateRenderManager(OSVR.ClientKit.ClientContext clientContext)
            {
                int result;
                try
                {
                    result = CreateRenderManagerFromUnity(clientContext.ContextHandle);
                }
                catch (DllNotFoundException e)
                {
                    result = -1;
                    Debug.LogError("[OSVR-Unity] Could not load "  + e.Message +
                        "\nosvrUnityRenderingPlugin.dll, or one of its dependencies, is missing from the project " + 
                        "or architecture doesn't match.\n");
                }
                return result;
            }

            //Pass pointer to eye-camera RenderTexture to the Unity Rendering Plugin
            public void SetEyeColorBuffer(IntPtr colorBuffer, int eye)
            {               
                SetColorBufferFromUnity(colorBuffer, eye);
            }

            //Get a pointer to the plugin's rendering function
            public IntPtr GetRenderEventFunction()
            {
                return GetRenderEventFunc();
            }

            //Shutdown RenderManager and Dispose of the ClientContext we created for it
            public void ExitRenderManager()
            {
                ShutdownRenderManager();
            }

            //helper functions to determine is RenderManager is supported
            //Is the RenderManager supported? Requires D3D11 or OpenGL, currently.
            public bool IsRenderManagerSupported()
            {
                bool support = true;
#if UNITY_ANDROID
                Debug.Log("[OSVR-Unity] RenderManager not yet supported on Android.");
                support = false;
#endif
                if (!SystemInfo.graphicsDeviceVersion.Contains("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("Direct3D 11"))
                {
                    Debug.LogError("[OSVR-Unity] RenderManager not supported on " +
                        SystemInfo.graphicsDeviceVersion + ". Only Direct3D11 is currently supported.");
                    support = false;
                }

                if (!SystemInfo.supportsRenderTextures)
                {
                    Debug.LogError("[OSVR-Unity] RenderManager not supported. RenderTexture (Unity Pro feature) is unavailable.");
                    support = false;
                }
                if (!IsUnityVersionSupported())
                {
                    Debug.LogError("[OSVR-Unity] RenderManager not supported. Unity 5.2+ is required for RenderManager support.");
                    support = false;
                }
                return support;
            }

            //Unity 5.2+ is required as the plugin uses the native plugin interface introduced in Unity 5.2
            public bool IsUnityVersionSupported()
            {
                bool support = true;
                try
                {
                    string version = new Regex(@"(\d+\.\d+)\..*").Replace(Application.unityVersion, "$1");
                    if (new Version(version) < new Version("5.2"))
                    {
                        support = false;
                    }
                }
                catch
                {
                    Debug.LogWarning("[OSVR-Unity] Unable to determine Unity version from: " + Application.unityVersion);
                    support = false;
                }
                return support;
            }
        }
    }
}