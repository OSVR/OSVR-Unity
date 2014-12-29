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
        /// Position interface: continually (or rather, when OSVR updates) updates its position based on the incoming tracker data.
        /// 
        /// Attach to a GameObject that you'd like to have updated in this way.
        /// </summary>
        public class PositionInterface : MonoBehaviour
        {
            /// <summary>
            /// The interface path you want to connect to.
            /// </summary>
            public string path;

            private OSVR.ClientKit.Interface iface;
            private OSVR.ClientKit.PositionCallback cb;

            // Use this for initialization
            void Start()
            {
                if (0 == path.Length)
                {
                    Debug.LogError("Missing path for PositionInterface " + gameObject.name);
                    return;
                }

                iface = OSVR.Unity.ClientKit.instance.context.getInterface(path);
                cb = new OSVR.ClientKit.PositionCallback(callback);
                iface.registerCallback(cb, IntPtr.Zero);
            }

            private void callback(IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PositionReport report)
            {
                transform.localPosition = Math.ConvertPosition(report.xyz);
            }

            void OnDestroy()
            {
                iface = null;
            }
        }
    }
}