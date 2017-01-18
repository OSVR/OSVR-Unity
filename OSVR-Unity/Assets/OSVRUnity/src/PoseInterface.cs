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
        public class PoseInterface : InterfaceGameObjectBase
        {
            PoseAdapter adapter;
            override protected void Start()
            {
                base.Start ();
                if (adapter == null && !String.IsNullOrEmpty(usedPath))
                {
                    adapter = new PoseAdapter(
                        OSVR.ClientKit.PoseInterface.GetInterface(ClientKit.instance.context, usedPath));
                    adapter.StateChanged += adapter_StateChanged;
                }
            }

            void adapter_StateChanged(object sender, OSVR.ClientKit.TimeValue timestamp, int sensor, Pose3 report)
            {
                transform.localPosition = report.Position;
                transform.localRotation = report.Rotation;
            }

            protected override void Stop()
            {
                base.Stop();
                if(adapter != null)
                {
                    adapter.Dispose();
                    adapter = null;
                }
            }

            public OSVR.ClientKit.IInterface<OSVR.Unity.Pose3> Interface
            {
                get
                {
                    this.Start();
                    return adapter;
                }
            }
        }
    }
}
