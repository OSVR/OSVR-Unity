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
        public class OrientationInterface : InterfaceGameObjectBase
        {
            OrientationAdapter adapter;

            override protected void Start()
            {
                base.Start();
                if (adapter == null && !String.IsNullOrEmpty(usedPath))
                {
                    adapter = new OrientationAdapter(
                        OSVR.ClientKit.OrientationInterface.GetInterface(ClientKit.instance.context, usedPath));
                }
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

            void Update()
            {
                if (this.adapter != null)
                {
                    var state = this.adapter.GetState();
                    transform.localRotation = state.Value;
                }
            }

            public OSVR.ClientKit.IInterface<Quaternion> Interface
            {
                get
                {
                    this.Start();
                    return this.adapter;
                }
            }
        }
    }
}
