using UnityEngine;
using System;

namespace OSVR
{
	namespace Unity
	{
		public class PoseInterface : MonoBehaviour {
			public string path;
			public OSVRClientKit ClientKit;

			private OSVR.ClientKit.Interface iface;
			private OSVR.ClientKit.PoseCallback cb;
			private Quaternion updatedRotation;
			private Vector3 updatedPosition;
			// Use this for initialization
			void Start () {
				iface = ClientKit.GetContext().getInterface (path);
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