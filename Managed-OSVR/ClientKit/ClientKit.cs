using System;
using System.Runtime.InteropServices;
using OSVR.ClientKit;
using System.Text;

namespace OSVR
{

    namespace ClientKit
    {

        /// @brief Client context object: Create and keep one in your application.
        /// Handles lifetime management and provides access to ClientKit
        /// functionality.
        /// @ingroup ClientKitCPP
        public class ClientContext : IDisposable
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

            public static Byte OSVR_RETURN_SUCCESS = 0x0;
            public static Byte OSVR_RETURN_FAILURE = 0x1;

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static IntPtr /*OSVR_ClientContext*/ osvrClientInit([MarshalAs(UnmanagedType.LPStr)] string applicationIdentifier, [MarshalAs(UnmanagedType.U4)] uint flags);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientUpdate(IntPtr /*OSVR_ClientContext*/ ctx);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientShutdown(IntPtr /*OSVR_ClientContext*/ ctx);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientGetStringParameterLength(IntPtr /*OSVR_ClientContext*/ ctx, string path, out int /* TODO size_t */ len);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientGetStringParameter(IntPtr /*OSVR_ClientContext*/ ctx, string path, StringBuilder buf, int /* TODO size_t */ len);

            #endregion

            /// @brief Initialize the library.
            /// @param applicationIdentifier A string identifying your application.
            /// Reverse DNS format strongly suggested.
            /// @param flags initialization options (reserved) - pass 0 for now.
            public ClientContext(string applicationIdentifier, uint flags)
            {
                this.m_context = osvrClientInit(applicationIdentifier, flags);
            }

            public ClientContext(string applicationIdentifier)
            {
                this.m_context = osvrClientInit(applicationIdentifier, 0);
            }

            ~ClientContext()
            {
                Dispose(false);
            }

            public void shutdown()
            {
                if (this.m_context != IntPtr.Zero)
                {
                    System.Diagnostics.Debug.WriteLine("Client shutdown");
                    osvrClientShutdown(this.m_context);
                    this.m_context = IntPtr.Zero;
                }

            }

            /// @brief Destructor: Shutdown the library.
            public void Dispose()
            {
                System.Diagnostics.Debug.WriteLine("In Dispose()");
                Dispose(true);
                // No need to call the finalizer since we've now cleaned
                // up the unmanaged memory.
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("In Dispose({0})", disposing));
                shutdown();
            }

            /// @brief Updates the state of the context - call regularly in your
            /// mainloop.
            public void update()
            {
                Byte ret = osvrClientUpdate(this.m_context);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ApplicationException("Error updating context.");
                }
            }

            /// @brief Get the interface associated with the given path.
            /// @param path A resource path.
            /// @returns The interface object.
            public Interface getInterface(string path)
            {
                IntPtr /*OSVR_ClientInterface*/ iface = IntPtr.Zero;
                Byte ret = Interface.osvrClientGetInterface(this.m_context, path, ref iface);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ArgumentException("Couldn't create interface because the path was invalid.");
                }

                return new Interface(iface);
            }

            /// @brief Get a string parameter value from the given path.
            /// @param path A resource path.
            /// @returns parameter value, or empty string if parameter does not
            /// exist or is not a string.
            public string getStringParameter(string path)
            {
                int length = 0;
                Byte ret = osvrClientGetStringParameterLength(m_context, path, out length);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ArgumentException("Invalid context or null reference to length variable.");
                }

                if (0 == length)
                {
                    return "";
                }

                StringBuilder buf = new StringBuilder(length);
                ret = osvrClientGetStringParameter(m_context, path, buf, length);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ApplicationException("Invalid context, null reference to buffer, or buffer is too small.");
                }

                return buf.ToString();
            }

            private IntPtr /*OSVR_ClientContext*/ m_context;
        }

    } // end namespace ClientKit

} // end namespace OSVR
