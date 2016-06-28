
/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2016 Sensics, Inc.
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

        public class ImagingInterface : InterfaceGameObjectBase
        {

            private OSVR.ClientKit.ImagingInterface iface;
            public OSVR.ClientKit.ImagingInterface Interface
            {
                get
                {
                    this.Start();
                    return iface;
                }
            }

            override protected void Start()
            {
                base.Start();
                if (iface == null && !String.IsNullOrEmpty(usedPath))
                {
                    iface = OSVR.ClientKit.ImagingInterface.GetInterface(
                        ClientKit.instance.context, usedPath);
                }
            }

            protected override void Stop()
            {
                base.Stop();
                if (iface != null)
                {
                    iface.Dispose();
                    iface = null;
                }
            }
        }
    }
}
