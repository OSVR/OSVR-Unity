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

		[DllImport (OSVR_CORE_DLL)]
		static public extern char osvrClientGetInterface(IntPtr /*OSVR_ClientContext*/ ctx, string path, IntPtr /*OSVR_ClientInterface*/ iface);

		[DllImport (OSVR_CORE_DLL)]
		static public extern char osvrClientFreeInterface(IntPtr /*OSVR_ClientInterface*/ iface);

		#endregion

		/// @brief Constructs an Interface object from an OSVR_ClientInterface
		/// object.
		public Interface(OSVR_ClientInterface iface)
		{
			this.m_interface = iface;
		}

		/// @brief Register a callback for some report type.
		public void registerCallback(OSVR_PoseCallback cb, void* userdata)
		{
			osvrRegisterPoseCallback (m_interface, cb, userdata);
		}

		public void registerCallback(OSVR_PositionCallback cb, void* userdata)
		{
			osvrRegisterPositionCallback (m_interface, cb, userdata);
		}

		public void registerCallback(OSVR_OrientationCallback cb, void* userdata)
		{
			osvrRegisterOrientationCallback (m_interface, cb, userdata);
		}

		public void registerCallback(OSVR_ButtonCallback cb, void* userdata)
		{
			osvrRegisterButtonCallback (m_interface, cb, userdata);
		}

		public void registerCallback(OSVR_AnalogCallback cb, void* userdata)
		{
			osvrRegisterAnalogCallback (m_interface, cb, userdata);
		}

		private IntPtr /*OSVR_ClientInterface*/ m_interface;
	}

} // end namespace ClientKit

} // end namespace OSVR
