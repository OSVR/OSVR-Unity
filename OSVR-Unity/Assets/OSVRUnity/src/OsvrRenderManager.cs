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

            private const string PluginName = "OsvrRenderingPlugin";

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


            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern void Shutdown();

            public void InitRenderManager(OSVR.ClientKit.ClientContext clientContext)
            {
                LinkDebug(functionPointer); // Hook our c++ plugin into Unitys console log.
                CreateRenderManager(clientContext);
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

            /*public int ReadPixels(IntPtr buffer, int x, int y, int width, int height)
            {
                return GetPixels(buffer, x, y, width, height);
            }*/

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
            public void ShutdownRenderManager()
            {
                GL.IssuePluginEvent(GetRenderEventFunc(), 1);
            }
        }
    }
}