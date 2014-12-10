using System;
using System.Runtime.InteropServices;

namespace OSVR {

	namespace ClientKit {

		/// @brief Interface handle object. Typically acquired from a ClientContext.
		/// @ingroup ClientKitCPP
		public class Interface {

			#region ClientKit C functions
			#if UNITY_IPHONE || UNITY_XBOX360
			// On iOS and Xbox 360, plugins are statically linked into
			// the executable, so we have to use __Internal as the
			// library name.
			const string OSVR_CORE_DLL = "__Internal";
			#else
			const string OSVR_CORE_DLL = "osvrCore";
			#endif

			//typedef struct OSVR_ClientContextObject *OSVR_ClientContext;
			//typedef struct OSVR_ClientInterfaceObject *OSVR_ClientInterface;
			//typedef char OSVR_ReturnCode; (0 == OSVR_RETURN_SUCCESS; 1 == OSVR_RETURN_FAILURE)

			#region Report structures
			[StructLayout(LayoutKind.Sequential)]
			struct OSVR_Vec3 {
				[MarshalAs(UnmanagedType.double, SizeConst=3)] public double data;
			}

			[StructLayout(LayoutKind.Sequential)]
			struct OSVR_Quaternion {
				double data[4];
			}

			[StructLayout(LayoutKind.Sequential)]
			struct OSVR_PoseState {
				OSVR_Vec3 translation;
				OSVR_Quaternion rotation;
			}

			[StructLayout(LayoutKind.Sequential)]
			struct OSVR_PoseReport {
				UInt32 sensors;
				OSVR_PoseState state;
			}
			#endregion

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate void OSVR_PositionCallback(IntPtr /*void*/ userdata, IntPtr /*const OSVR_TimeValue*/ timestamp, IntPtr /*const OSVR_PositionReport*/ report);
			public delegate void OSVR_PoseCallback(IntPtr /*void*/ userdata, IntPtr /*const OSVR_TimeValue*/ timestamp, IntPtr /*const OSVR_PositionReport*/ report);
			public delegate void OSVR_OrientationCallback(IntPtr /*void*/ userdata, IntPtr /*const OSVR_TimeValue*/ timestamp, IntPtr /*const OSVR_PositionReport*/ report);
			public delegate void OSVR_ButtonCallback(IntPtr /*void*/ userdata, IntPtr /*const OSVR_TimeValue*/ timestamp, IntPtr /*const OSVR_PositionReport*/ report);
			public delegate void OSVR_AnalogCallback(IntPtr /*void*/ userdata, IntPtr /*const OSVR_TimeValue*/ timestamp, IntPtr /*const OSVR_PositionReport*/ report);

			[DllImport (OSVR_CORE_DLL)]
			public extern static char osvrRegisterPositionCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] OSVR_PositionCallback cb, IntPtr /*void*/ userdata);

			[DllImport (OSVR_CORE_DLL)]
			public extern static char osvrRegisterPoseCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] OSVR_PoseCallback cb, IntPtr /*void*/ userdata);

			[DllImport (OSVR_CORE_DLL)]
			public extern static char osvrRegisterOrientationCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] OSVR_OrientationCallback cb, IntPtr /*void*/ userdata);

			[DllImport (OSVR_CORE_DLL)]
			public extern static char osvrRegisterButtonCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] OSVR_ButtonCallback cb, IntPtr /*void*/ userdata);

			[DllImport (OSVR_CORE_DLL)]
			public extern static char osvrRegisterAnalogCallback(IntPtr /*OSVR_ClientInterface*/ iface, [MarshalAs(UnmanagedType.FunctionPtr)] OSVR_AnalogCallback cb, IntPtr /*void*/ userdata);

			[DllImport (OSVR_CORE_DLL)]
			static public extern char osvrClientGetInterface(IntPtr /*OSVR_ClientContext*/ ctx, string path, ref IntPtr /*OSVR_ClientInterface*/ iface);

			[DllImport (OSVR_CORE_DLL)]
			static public extern char osvrClientFreeInterface(IntPtr /*OSVR_ClientInterface*/ iface);

			#endregion

			/// @brief Constructs an Interface object from an OSVR_ClientInterface
			/// object.
			public Interface(IntPtr /*OSVR_ClientInterface*/ iface)
			{
				this.m_interface = iface;
			}

			/// @brief Register a callback for some report type.
			public void registerCallback(OSVR_PoseCallback cb, IntPtr /*void*/ userdata)
			{
				osvrRegisterPoseCallback (m_interface, cb, userdata);
			}

			public void registerCallback(OSVR_PositionCallback cb, IntPtr /*void*/ userdata)
			{
				osvrRegisterPositionCallback (m_interface, cb, userdata);
			}

			public void registerCallback(OSVR_OrientationCallback cb, IntPtr /*void*/ userdata)
			{
				osvrRegisterOrientationCallback (m_interface, cb, userdata);
			}

			public void registerCallback(OSVR_ButtonCallback cb, IntPtr /*void*/ userdata)
			{
				osvrRegisterButtonCallback (m_interface, cb, userdata);
			}

			public void registerCallback(OSVR_AnalogCallback cb, IntPtr /*void*/ userdata)
			{
				osvrRegisterAnalogCallback (m_interface, cb, userdata);
			}

			private IntPtr /*OSVR_ClientInterface*/ m_interface;
		}

	} // end namespace ClientKit

} // end namespace OSVR
