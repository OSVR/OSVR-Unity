		public delegate void PositionCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref PositionReport report);
		public delegate void PoseCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref PoseReport report);
		public delegate void OrientationCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref OrientationReport report);
		public delegate void ButtonCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref ButtonReport report);
		public delegate void AnalogCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref AnalogReport report);
﻿using System;
using System.Runtime.InteropServices;

namespace OSVR
{

    namespace ClientKit
    {

        /// @brief Interface handle object. Typically acquired from a ClientContext.
        /// @ingroup ClientKitCPP
        public class Interface
        {

            #region ClientKit C functions
#if UNITY_IPHONE || UNITY_XBOX360
			// On iOS and Xbox 360, plugins are statically linked into
			// the executable, so we have to use __Internal as the
			// library name.
			const string OSVR_CORE_DLL = "__Internal";
#else
            const string OSVR_CORE_DLL = "osvrClientKit";
#endif

            //typedef struct OSVR_ClientContextObject *OSVR_ClientContext;
            //typedef struct OSVR_ClientInterfaceObject *OSVR_ClientInterface;
            //typedef char OSVR_ReturnCode; (0 == OSVR_RETURN_SUCCESS; 1 == OSVR_RETURN_FAILURE)


            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterPositionCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] PositionCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterPoseCallback(IntPtr /*OSVR_ClientInterface*/ iface, PoseCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterOrientationCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] OrientationCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterButtonCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] ButtonCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterAnalogCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] AnalogCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            static public extern Byte osvrClientGetInterface(IntPtr /*OSVR_ClientContext*/ ctx, string path, ref IntPtr /*OSVR_ClientInterface*/ iface);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            static public extern Byte osvrClientFreeInterface(IntPtr /*OSVR_ClientInterface*/ iface);

            #endregion

            /// @brief Constructs an Interface object from an OSVR_ClientInterface
            /// object.
            public Interface(IntPtr /*OSVR_ClientInterface*/ iface)
            {
                this.m_interface = iface;
            }

            /// @brief Register a callback for some report type.
            public void registerCallback(PoseCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterPoseCallback(m_interface, cb, userdata);
            }

            public void registerCallback(PositionCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterPositionCallback(m_interface, cb, userdata);
            }

            public void registerCallback(OrientationCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterOrientationCallback(m_interface, cb, userdata);
            }

            public void registerCallback(ButtonCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterButtonCallback(m_interface, cb, userdata);
            }

            public void registerCallback(AnalogCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterAnalogCallback(m_interface, cb, userdata);
            }

            private IntPtr /*OSVR_ClientInterface*/ m_interface;
        }

    } // end namespace ClientKit

} // end namespace OSVR
