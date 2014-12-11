using System;
using System.Runtime.InteropServices;
using OSVR.ClientKit;

namespace OSVR {

namespace ClientKit {

	/// @brief Client context object: Create and keep one in your application.
	/// Handles lifetime management and provides access to ClientKit
	/// functionality.
	/// @ingroup ClientKitCPP
	class ClientContext : IDisposable {
			#region ClientKit C functions
			#if UNITY_IPHONE || UNITY_XBOX360
			// On iOS and Xbox 360, plugins are statically linked into
			// the executable, so we have to use __Internal as the
			// library name.
			const string OSVR_CORE_DLL = "__Internal";
			#else
			const string OSVR_CORE_DLL = "osvrCore";
			#endif

			[DllImport (OSVR_CORE_DLL)]
			IntPtr /*OSVR_ClientContext*/ osvrClientInit(string applicationIdentifier, uint flags);
			
			[DllImport (OSVR_CORE_DLL)]
			char osvrClientUpdate(IntPtr /*OSVR_ClientContext*/ ctx);
			
			[DllImport (OSVR_CORE_DLL)]
			char osvrClientShutdown(IntPtr /*OSVR_ClientContext*/ ctx);
			
			#endregion

		/// @brief Initialize the library.
		/// @param applicationIdentifier A string identifying your application.
		/// Reverse DNS format strongly suggested.
		/// @param flags initialization options (reserved) - pass 0 for now.
		public ClientContext(string applicationIdentifier, uint flags = 0)
		{
			this.m_context = osvrClientInit(applicationIdentifier, flags);
		}
		
		~ClientContext()
		{
			Dispose(false);
		}

		/// @brief Destructor: Shutdown the library.
		public void Dispose()
		{
			Dispose(true);
			osvrClientShutdown(m_context);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.m_context != IntPtr.Zero) {
				osvrClientShutdown (this.m_context);
				this.m_context = IntPtr.Zero;
			}

			if (disposing) {
				// No need to call the finalizer since we've now cleaned
				// up the unmanaged memory.
				GC.SuppressFinalize (this);
			}
		}

		/// @brief Updates the state of the context - call regularly in your
		/// mainloop.
		public void update()
		{
			char ret = osvrClientUpdate(this.m_context);
			if (OSVR_RETURN_SUCCESS != ret) {
				throw ApplicationException("Error updating context.");
			}
		}

		/// @brief Get the interface associated with the given path.
		/// @param path A resource path.
		/// @returns The interface object.
		public Interface getInterface(string path)
		{
			IntPtr /*OSVR_ClientInterface*/ iface = IntPtr.Zero;
			char ret = osvrClientGetInterface(this.m_context, path, ref iface);
			if (OSVR_RETURN_SUCCESS != ret) {
				throw ArgumentException("Couldn't create interface because the path was invalid.");
			}

			return new Interface(iface);
		}
		
		/// @brief Get a string parameter value from the given path.
		/// @param path A resource path.
		/// @returns parameter value, or empty string if parameter does not
		/// exist or is not a string.
		public string getStringParameter(string path)
		{
			size_t length = 0;
			char ret = osvrClientGetStringParameterLength(m_context, path.c_str(), ref length);
			if (OSVR_RETURN_SUCCESS != ret) {
				throw ArgumentException("Invalid context or null reference to length variable.");
			}
			
			if (0 == length) {
				return "";
			}
			
			boost::scoped_array<char> buf(new char[length]);
			ret = osvrClientGetStringParameter(m_context, path.c_str(), buf.get(), length);
			if (OSVR_RETURN_SUCCESS != ret) {
				throw ApplicationException("Invalid context, null reference to buffer, or buffer is too small.");
			}
			
			return std::string(buf.get(), length);
		}

		private IntPtr /*OSVR_ClientContext*/ m_context;
	}

} // end namespace ClientKit

} // end namespace OSVR
