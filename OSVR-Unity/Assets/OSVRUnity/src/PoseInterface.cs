/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */

using System;
using UnityEngine;

namespace OSVR
{
    namespace Unity
    {
        /// <summary>
        /// Pose interface: continually (or rather, when OSVR updates) updates its position and orientation based on the incoming tracker data.
        /// 
        /// Attach to a GameObject that you'd like to have updated in this way.
        /// </summary>
        public class PoseInterface : MonoBehaviour
        {
            /// <summary>
            /// The interface path you want to connect to.
            /// </summary>
            public string path;

            private OSVR.ClientKit.Interface iface;
            private OSVR.ClientKit.PoseCallback cb;

            void Start()
            {
                if (0 == path.Length)
                {
                    Debug.LogError("Missing path for PoseInterface " + gameObject.name);
                    return;
                }

                iface = OSVR.Unity.ClientKit.instance.context.getInterface(path);
                cb = new OSVR.ClientKit.PoseCallback(callback);
                iface.registerCallback(cb, IntPtr.Zero);
            }

            private void callback(IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report)
            {
                transform.position = Math.ConvertPosition(report.pose.translation);
                transform.rotation = Math.ConvertOrientation(report.pose.rotation);
            }
        }
    }
}