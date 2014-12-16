/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */

using UnityEngine;
using System;

namespace OSVR
{
    namespace Unity
    {
        /// <summary>
        /// Orientation Interface: continually (or rather, when OSVR updates) updates its orientation based on the incoming tracker data.
        /// 
        /// Attach to a GameObject that you'd like to have updated in this way.
        /// </summary>
        public class OrientationInterface : MonoBehaviour
        {
            /// <summary>
            /// The interface path you want to connect to.
            /// </summary>
            public string path;

            /// <summary>
            /// This should be a reference to the single ClientKit instance in your project.
            /// </summary>
            public ClientKit clientKit;

            private OSVR.ClientKit.Interface iface;
            private OSVR.ClientKit.OrientationCallback cb;

            // Use this for initialization
            void Start()
            {
                if (0 == path.Length)
                {
                    Debug.LogError("Missing path for OrientationInterface " + gameObject.name);
                    return;
                }

                if (null == clientKit)
                {
                    Debug.LogError("Missing ClientKit reference for OrientationInterface " + gameObject.name);
                    return;
                }
                iface = clientKit.GetContext().getInterface(path);
                cb = new OSVR.ClientKit.OrientationCallback(callback);
                iface.registerCallback(cb, IntPtr.Zero);
            }

            private void callback(IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.OrientationReport report)
            {
                transform.rotation = Math.ConvertOrientation(report.rotation);
            }
        }
    }
}