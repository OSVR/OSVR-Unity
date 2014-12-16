using UnityEngine;
using System;

namespace OSVR
{
	namespace Unity
	{
		/// <summary>
		/// Position interface: continually (or rather, when OSVR updates, in FixedUpdate) updates its position based on the incoming tracker data.
		/// </summary>
		public class PositionInterface : MonoBehaviour {
			/// <summary>
			/// The interface path you want to connect to.
			/// </summary>
			public string path;
			
			/// <summary>
			/// This should be a reference to the single ClientKit instance in your project.
			/// </summary>
			public ClientKit clientKit;
			
			private OSVR.ClientKit.Interface iface;
			private OSVR.ClientKit.PositionCallback cb;
			
			// Use this for initialization
			void Start () {
				if (0 == path.Length) {
					Debug.LogError("Missing path for PositionInterface " + gameObject.name);
					return;
				}
				
				if (null == clientKit) {
					Debug.LogError("Missing ClientKit reference for PositionInterface " + gameObject.name);
					return;
				}
				iface = clientKit.GetContext().getInterface (path);
				cb = new OSVR.ClientKit.PositionCallback (callback);
				iface.registerCallback (cb, IntPtr.Zero);
			}
			
			private void callback(IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PositionReport report) {
				transform.position = Math.ConvertPosition (report.xyz);
			}
		}
	}
}