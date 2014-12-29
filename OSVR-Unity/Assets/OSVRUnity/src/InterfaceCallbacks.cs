/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */

using UnityEngine;

namespace OSVR
{
    namespace Unity
    {
        /// <summary>
        /// OSVR Interface, supporting generic callbacks that provide the source path and a Unity-native datatype.
        /// 
        /// Note that right now, this doesn't work right.
        /// </summary>
        public class InterfaceCallbacks : MonoBehaviour
        {
            /// <summary>
            /// The interface path you want to connect to.
            /// </summary>
            public string path;

            public delegate void PoseMatrixCallback(string source, Matrix4x4 pose);
            public delegate void PoseCallback(string source, Vector3 position, Quaternion rotation);
            public delegate void PositionCallback(string source, Vector3 position);
            public delegate void OrientationCallback(string source, Quaternion rotation);
            public delegate void ButtonCallback(string source, bool pressed);
            public delegate void AnalogCallback(string source, float value);

            void Start()
            {
                if (null == iface)
                {
                    iface = OSVR.Unity.ClientKit.instance.context.getInterface(path);
                }
            }

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



            private void PoseCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report)
            {
                Vector3 position = Math.ConvertPosition(report.pose.translation);
                Quaternion rotation = Math.ConvertOrientation(report.pose.rotation);
                poseCallbacks(path, position, rotation);
            }

            private void PoseMatrixCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report)
            {
                Matrix4x4 matPose = Math.ConvertPose(report.pose);
                poseMatrixCallbacks(path, matPose);
            }

            private void PositionCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PositionReport report)
            {
                Vector3 position = Math.ConvertPosition(report.xyz);
                positionCallbacks(path, position);
            }

            private void OrientationCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.OrientationReport report)
            {
                Quaternion rotation = Math.ConvertOrientation(report.rotation);
                orientationCallbacks(path, rotation);
            }

            private void ButtonCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.ButtonReport report)
            {
                bool pressed = (report.state == 1);
                if (buttonCallbacks != null)
                {
                    buttonCallbacks(path, pressed);
                }
            }

            private void AnalogCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.AnalogReport report)
            {
                float val = (float)report.state;
                analogCallbacks(path, val);
            }

            private OSVR.ClientKit.Interface iface;
        }
    }
}
