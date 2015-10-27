using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
namespace OSVR
{
    namespace Unity
    {
        public class OsvrRenderManager : MonoBehaviour
        {

            private const string PluginName = "osvrUnityRenderingPlugin";

            // The block of code below is a neat trick to allow for calling into the debug console from C++
            [DllImport(PluginName)]
            private static extern void LinkDebug([MarshalAs(UnmanagedType.FunctionPtr)]IntPtr debugCal);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate void DebugLog(string log);

            private static readonly DebugLog debugLog = DebugWrapper;
            private static readonly IntPtr functionPointer = Marshal.GetFunctionPointerForDelegate(debugLog);

            private static void DebugWrapper(string log) { Debug.Log(log); }

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern int GetEventID();

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern void SetUnityStreamingAssetsPath([MarshalAs(UnmanagedType.LPStr)] string path);

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern IntPtr GetRenderEventFunc();

            [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
            private static extern void SetColorBufferFromUnity(System.IntPtr texturePtr, int eye);

            //@todo the IntPtr should be a SafeClientContextHandle
            [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
            private static extern Byte CreateRenderManagerFromUnity(OSVR.ClientKit.SafeClientContextHandle /*OSVR_ClientContext*/ ctx);

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern void SetTimeFromUnity(float t);

            [StructLayout(LayoutKind.Sequential)]
            public struct OSVR_ViewportDescription
            {
                public double left;    //< Left side of the viewport in pixels
                public double lower;   //< First pixel in the viewport at the bottom.
                public double width;   //< Last pixel in the viewport at the top
                public double height;   //< Last pixel on the right of the viewport in pixels
            }

            [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
            private static extern OSVR_ViewportDescription GetViewport(int eye);

            [StructLayout(LayoutKind.Sequential)]
            public struct OSVR_ProjectionMatrix
            {
                public double left;
                public double right;
                public double top;
                public double bottom;
                public double nearClip;        //< Cannot name "near" because Visual Studio keyword
                public double farClip;
            }

            [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
            private static extern OSVR_ProjectionMatrix GetProjectionMatrix(int eye);

            [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
            private static extern OSVR.ClientKit.Pose3 GetEyePose(int eye);


            [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
            private static extern void ShutdownRenderManager();

            private OSVR.ClientKit.ClientContext _renderManagerClientContext;

            public void InitRenderManager()
            {
                LinkDebug(functionPointer); // Hook our c++ plugin into Unitys console log.
                _renderManagerClientContext = new OSVR.ClientKit.ClientContext("com.sensics.rendermanagercontext", 0);
                CreateRenderManager(_renderManagerClientContext);
            }

            public OSVR.ClientKit.Pose3 GetRenderManagerEyePose(int eye)
            {
                return GetEyePose(eye);
            }

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

            public Matrix4x4 GetEyeProjectionMatrix(int eye)
            {
                OSVR_ProjectionMatrix pm = GetProjectionMatrix(eye);
                return PerspectiveOffCenter((float)pm.left, (float)pm.right, (float)pm.bottom, (float)pm.top, (float)pm.nearClip, (float)pm.farClip);
                
            }
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
                return CreateRenderManagerFromUnity(clientContext.ContextHandle);
            }

            //Pass pointer to eye-camera RenderTexture to the Unity Rendering Plugin
            public void SetEyeColorBuffer(IntPtr colorBuffer, int eye)
            {
                Debug.Log("About to call native plugin with color buffer");
                SetColorBufferFromUnity(colorBuffer, eye);
            }

            //Get a rendering event ID from the Unity Rendering Plugin
            public int GetRenderEventID()
            {
                return GetEventID();
            }

            public IntPtr GetRenderEventFunction()
            {
                return GetRenderEventFunc();
            }

            //Pass Unity time to Unity Rendering Plugin
            //Would probably pass in Time.time
            public void SetRenderEventTime(float t)
            {
                SetTimeFromUnity(t);
            }
            public void ExitRenderManager()
            {
                ShutdownRenderManager();
            }
        }
    }
}