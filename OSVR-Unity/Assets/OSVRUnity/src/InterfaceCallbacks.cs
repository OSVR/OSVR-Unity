/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>

using UnityEngine;

namespace OSVR
{
    namespace Unity
    {
        /// <summary>
        /// OSVR Interface, supporting generic callbacks that provide the source path and a Unity-native datatype.
        /// </summary>
        public class InterfaceCallbacks : ScriptableObject
        {
            /// <summary>
            /// The interface path you want to connect to.
            /// </summary>
            public string path;

            #region Callback (delegate) types
            public delegate void PoseMatrixCallback(string source, Matrix4x4 pose);
            public delegate void PoseCallback(string source, Vector3 position, Quaternion rotation);
            public delegate void PositionCallback(string source, Vector3 position);
            public delegate void OrientationCallback(string source, Quaternion rotation);
            public delegate void ButtonCallback(string source, bool pressed);
            public delegate void AnalogCallback(string source, float value);
            #endregion

            #region Event methods
            public void Start()
            {
                if (null == iface)
                {
                    iface = OSVR.Unity.ClientKit.instance.context.getInterface(path);
                }
            }

            public void OnDestroy()
            {
                Stop();
            }

            public void OnApplicationQuit()
            {
                Stop();
            }

            #endregion

            /// <summary>
            /// Used in the end-of-life method, this can also be called manually to free the internal interface.
            /// </summary>
            public void Stop()
            {
                iface = null;
                poseMatrixCallbacks = null;
                poseCallbacks = null;
                positionCallbacks = null;
                orientationCallbacks = null;
                buttonCallbacks = null;
                analogCallbacks = null;
            }

            #region Generated RegisterCallback overloads and associated data
            /// The section between the BEGIN and END generated code can be replaced with the output of running
            /// the unity-generate.lua script to update it.
            /* BEGIN GENERATED CODE - unity-generate.lua */
            public void RegisterCallback(PoseMatrixCallback callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == poseMatrixCallbacks)
                {
                    poseMatrixCallbacks = callback;
                    rawPoseMatrixCallback = new OSVR.ClientKit.PoseCallback(PoseMatrixCb);
                    iface.registerCallback(rawPoseMatrixCallback, System.IntPtr.Zero);
                }
                else
                {
                    poseMatrixCallbacks += callback;
                }
            }

            private OSVR.ClientKit.PoseCallback rawPoseMatrixCallback;
            private PoseMatrixCallback poseMatrixCallbacks;

            public void RegisterCallback(PoseCallback callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == poseCallbacks)
                {
                    poseCallbacks = callback;
                    rawPoseCallback = new OSVR.ClientKit.PoseCallback(PoseCb);
                    iface.registerCallback(rawPoseCallback, System.IntPtr.Zero);
                }
                else
                {
                    poseCallbacks += callback;
                }
            }

            private OSVR.ClientKit.PoseCallback rawPoseCallback;
            private PoseCallback poseCallbacks;

            public void RegisterCallback(PositionCallback callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == positionCallbacks)
                {
                    positionCallbacks = callback;
                    rawPositionCallback = new OSVR.ClientKit.PositionCallback(PositionCb);
                    iface.registerCallback(rawPositionCallback, System.IntPtr.Zero);
                }
                else
                {
                    positionCallbacks += callback;
                }
            }

            private OSVR.ClientKit.PositionCallback rawPositionCallback;
            private PositionCallback positionCallbacks;

            public void RegisterCallback(OrientationCallback callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == orientationCallbacks)
                {
                    orientationCallbacks = callback;
                    rawOrientationCallback = new OSVR.ClientKit.OrientationCallback(OrientationCb);
                    iface.registerCallback(rawOrientationCallback, System.IntPtr.Zero);
                }
                else
                {
                    orientationCallbacks += callback;
                }
            }

            private OSVR.ClientKit.OrientationCallback rawOrientationCallback;
            private OrientationCallback orientationCallbacks;

            public void RegisterCallback(ButtonCallback callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == buttonCallbacks)
                {
                    buttonCallbacks = callback;
                    rawButtonCallback = new OSVR.ClientKit.ButtonCallback(ButtonCb);
                    iface.registerCallback(rawButtonCallback, System.IntPtr.Zero);
                }
                else
                {
                    buttonCallbacks += callback;
                }
            }

            private OSVR.ClientKit.ButtonCallback rawButtonCallback;
            private ButtonCallback buttonCallbacks;

            public void RegisterCallback(AnalogCallback callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == analogCallbacks)
                {
                    analogCallbacks = callback;
                    rawAnalogCallback = new OSVR.ClientKit.AnalogCallback(AnalogCb);
                    iface.registerCallback(rawAnalogCallback, System.IntPtr.Zero);
                }
                else
                {
                    analogCallbacks += callback;
                }
            }

            private OSVR.ClientKit.AnalogCallback rawAnalogCallback;
            private AnalogCallback analogCallbacks;

            /* END GENERATED CODE - unity-generate.lua */
            #endregion

            /// These wrappers sadly have to be mostly hand-written, despite their similarity, since they convert data types
            /// and also data conventions (into Unity's left-handed coordinate system)
            #region Private wrapper callbacks/trampolines
            /// <summary>
            /// Pose (as position and orientation) wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker pose report</param>
            private void PoseCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report)
            {
                Vector3 position = Math.ConvertPosition(report.pose.translation);
                Quaternion rotation = Math.ConvertOrientation(report.pose.rotation);
                if (null != poseCallbacks)
                {
                    poseCallbacks(path, position, rotation);
                }
            }

            /// <summary>
            /// Pose (as a 4x4 matrix) wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker pose report</param>
            private void PoseMatrixCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report)
            {
                Matrix4x4 matPose = Math.ConvertPose(report.pose);
                if (null != poseMatrixCallbacks)
                {
                    poseMatrixCallbacks(path, matPose);
                }
            }

            /// <summary>
            /// Position wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker position report</param>
            private void PositionCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PositionReport report)
            {
                Vector3 position = Math.ConvertPosition(report.xyz);
                if (null != positionCallbacks)
                {
                    positionCallbacks(path, position);
                }
            }

            /// <summary>
            /// Orientation wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker orientation report</param>
            private void OrientationCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.OrientationReport report)
            {
                Quaternion rotation = Math.ConvertOrientation(report.rotation);
                if (null != orientationCallbacks)
                {
                    orientationCallbacks(path, rotation);
                }
            }

            /// <summary>
            /// Button wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Button report</param>
            private void ButtonCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.ButtonReport report)
            {
                bool pressed = (report.state == 1);
                if (buttonCallbacks != null)
                {
                    buttonCallbacks(path, pressed);
                }
            }

            /// <summary>
            /// Analog wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Analog report</param>
            private void AnalogCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.AnalogReport report)
            {
                float val = (float)report.state;
                if (null != analogCallbacks)
                {
                    analogCallbacks(path, val);
                }
            }
            #endregion

            #region Private variables
            private OSVR.ClientKit.Interface iface;
            #endregion
        }
    }
}
