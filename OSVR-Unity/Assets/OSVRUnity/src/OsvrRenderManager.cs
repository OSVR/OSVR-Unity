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
            private static extern void SetColorBufferFromUnity(System.IntPtr texturePtr, int eye);

            //@todo the IntPtr should be a SafeClientContextHandle
            [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
            private static extern Byte CreateRenderManagerFromUnity(OSVR.ClientKit.SafeClientContextHandle /*OSVR_ClientContext*/ ctx);

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern void SetTimeFromUnity(float t);

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern int GetEyeWidth();

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern int GetEyeHeight();

            [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
            private static extern void Shutdown();           

           // [DllImport(PluginName, CallingConvention = CallingConvention.StdCall)]
           // private static extern int GetPixels(IntPtr buffer, int x, int y, int width, int height);

            public void InitRenderManager(OSVR.ClientKit.ClientContext clientContext)
            {
                LinkDebug(functionPointer); // Hook our c++ plugin into Unitys console log.
                Debug.Log("Event id is " + GetRenderEventID()); //test that this works
                CreateRenderManager(clientContext);
            }

            public int GetRenderTextureWidth()
            {
                return GetEyeWidth();
            }
            public int GetRenderTextureHeight()
            {
                return GetEyeHeight();
            }

            /*public int ReadPixels(IntPtr buffer, int x, int y, int width, int height)
            {
                return GetPixels(buffer, x, y, width, height);
            }*/

            //Call the Unity Rendering Plugin to initialize the RenderManager
            public void CreateRenderManager(OSVR.ClientKit.ClientContext clientContext)
            {
                CreateRenderManagerFromUnity(clientContext.ContextHandle);
            }

            //Pass pointer to eye-camera RenderTexture to the Unity Rendering Plugin
            public void SetEyeColorBuffer(IntPtr colorBuffer, int eye)
            {
                SetColorBufferFromUnity(colorBuffer, eye);
            }

            //Get a rendering event ID from the Unity Rendering Plugin
            public int GetRenderEventID()
            {
                return GetEventID();
            }

            //Pass Unity time to Unity Rendering Plugin
            //Would probably pass in Time.time
            public void SetRenderEventTime(float t)
            {
                SetTimeFromUnity(t);
            }
            public void ShutdownRenderManager()
            {
                Shutdown();
            }
        }
    }
}
