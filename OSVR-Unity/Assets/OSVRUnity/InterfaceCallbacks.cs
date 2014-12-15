using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OSVR
{
	namespace Unity
	{
		/// <summary>
		/// OSVR Interface, supporting generic callbacks that provide the source path and a Unity-native datatype.
		/// </summary>
		public class InterfaceCallbacks : MonoBehaviour {
			/// <summary>
			/// The interface path you want to connect to.
			/// </summary>
			public string path;
			
			/// <summary>
			/// This should be a reference to the single ClientKit instance in your project.
			/// </summary>
			public OSVRClientKit ClientKit;

			public delegate void PoseMatrixCallback(string source, Matrix4x4 pose);
			public delegate void PoseCallback(string source, Vector3 position, Quaternion rotation);
			public delegate void PositionCallback(string source, Vector3 position);
			public delegate void OrientationCallback(string source, Quaternion rotation);
			public delegate void ButtonCallback(string source, bool pressed);
			public delegate void AnalogCallback(string source, float value);

			
			void Start () {
				iface = ClientKit.GetContext().getInterface (path);
			}

			/* BEGIN GENERATED CODE - unity-generate.lua */
			public void RegisterCallback(PoseMatrixCallback callback) {
				if (null == poseMatrixCallbacks) {
					poseMatrixCallbacks = new List<PoseMatrixCallback>();
					iface.registerCallback (PoseMatrixCb, System.IntPtr.Zero);
				}
				poseMatrixCallbacks.Add (callback);
			}
			
			private List<PoseMatrixCallback> poseMatrixCallbacks;
			
			public void RegisterCallback(PoseCallback callback) {
				if (null == poseCallbacks) {
					poseCallbacks = new List<PoseCallback>();
					iface.registerCallback (PoseCb, System.IntPtr.Zero);
				}
				poseCallbacks.Add (callback);
			}
			
			private List<PoseCallback> poseCallbacks;
			
			public void RegisterCallback(PositionCallback callback) {
				if (null == positionCallbacks) {
					positionCallbacks = new List<PositionCallback>();
					iface.registerCallback (PositionCb, System.IntPtr.Zero);
				}
				positionCallbacks.Add (callback);
			}
			
			private List<PositionCallback> positionCallbacks;
			
			public void RegisterCallback(OrientationCallback callback) {
				if (null == orientationCallbacks) {
					orientationCallbacks = new List<OrientationCallback>();
					iface.registerCallback (OrientationCb, System.IntPtr.Zero);
				}
				orientationCallbacks.Add (callback);
			}
			
			private List<OrientationCallback> orientationCallbacks;
			
			public void RegisterCallback(ButtonCallback callback) {
				if (null == buttonCallbacks) {
					buttonCallbacks = new List<ButtonCallback>();
					iface.registerCallback (ButtonCb, System.IntPtr.Zero);
				}
				buttonCallbacks.Add (callback);
			}
			
			private List<ButtonCallback> buttonCallbacks;
			
			public void RegisterCallback(AnalogCallback callback) {
				if (null == analogCallbacks) {
					analogCallbacks = new List<AnalogCallback>();
					iface.registerCallback (AnalogCb, System.IntPtr.Zero);
				}
				analogCallbacks.Add (callback);
			}
			
			private List<AnalogCallback> analogCallbacks;
			
			/* END GENERATED CODE - unity-generate.lua */
			
			private void PoseCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report) {
				Vector3 position = Math.ConvertPosition (report.pose.translation);
				Quaternion rotation = Math.ConvertOrientation (report.pose.rotation);
				foreach (PoseCallback cb in poseCallbacks) {
					cb(path, position, rotation);
				}
			}

			private void PoseMatrixCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report) {
				Matrix4x4 matPose = Math.ConvertPose(report.pose);
				foreach (PoseMatrixCallback cb in poseMatrixCallbacks) {
					cb(path, matPose);
				}
			}

			private void PositionCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PositionReport report) {
				Vector3 position = Math.ConvertPosition (report.xyz);
				foreach (PositionCallback cb in positionCallbacks) {
					cb (path, position);
				}
			}

			private void OrientationCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.OrientationReport report) {
				Quaternion rotation = Math.ConvertOrientation (report.rotation);
				foreach (OrientationCallback cb in orientationCallbacks) {
					cb (path, rotation);
				}
			}

			private void ButtonCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.ButtonReport report) {
				bool pressed = (report.state == 1);
				foreach (ButtonCallback cb in buttonCallbacks) {
					cb (path, pressed);
				}
			}

			private void AnalogCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.AnalogReport report) {
				float val = (float)report.state;
				foreach (AnalogCallback cb in analogCallbacks) {
					cb (path, val);
				}
			}

			private OSVR.ClientKit.Interface iface;
		}
	}
}
