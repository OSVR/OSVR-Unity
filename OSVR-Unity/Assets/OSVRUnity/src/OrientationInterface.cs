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

                iface = OSVR.Unity.ClientKit.instance.context.getInterface(path);
                cb = new OSVR.ClientKit.OrientationCallback(callback);
                iface.registerCallback(cb, IntPtr.Zero);
            }

            private void callback(IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.OrientationReport report)
            {
                transform.localRotation = Math.ConvertOrientation(report.rotation);
            }

            void OnDestroy()
            {
                iface = null;
            }
        }
    }
}
