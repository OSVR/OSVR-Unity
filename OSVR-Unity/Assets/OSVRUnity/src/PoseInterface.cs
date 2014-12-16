using UnityEngine;
using System;

namespace OSVR
{
	namespace Unity
	{
		/// <summary>
		/// Pose interface: continually (or rather, when OSVR updates, in FixedUpdate) updates its position and orientation based on the incoming tracker data.
		/// </summary>
		public class PoseInterface : MonoBehaviour {
			/// <summary>
			/// The interface path you want to connect to.
			/// </summary>
			public string path;

			/// <summary>
			/// This should be a reference to the single ClientKit instance in your project.
			/// </summary>
			public ClientKit clientKit;

			private OSVR.ClientKit.Interface iface;
			private OSVR.ClientKit.PoseCallback cb;

			void Start () {
				if (0 == path.Length) {
					Debug.LogError("Missing path for PoseInterface " + gameObject.name);
					return;
				}
				
				if (null == clientKit) {
					Debug.LogError("Missing ClientKit reference for PoseInterface " + gameObject.name);
					return;
				}

				iface = clientKit.GetContext().getInterface (path);
				cb = new OSVR.ClientKit.PoseCallback (callback);
				iface.registerCallback (cb, IntPtr.Zero);
			}

			private void callback(IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report) {
				transform.position = Math.ConvertPosition (report.pose.translation);
				transform.rotation = Math.ConvertOrientation (report.pose.rotation);
			}
		}
	}
}