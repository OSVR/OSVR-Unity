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
using System.Collections;

namespace OSVR
{
    namespace Unity
    {

        /// <summary>
        /// (OBSOLETE) A script component to add to a GameObject in order to access an interface, managing lifetime and centralizing the path specification.
        /// </summary>
        [System.Obsolete("Use one of the OSVR.Unity.Requires*Interface base classes instead.")]
        public class InterfaceGameObject : InterfaceGameObjectBase
        {
            public InterfaceCallbacks osvrInterface
            {
                get
                {
                    Start();
                    return iface;
                }
            }

            protected InterfaceGameObject interfaceGameObject
            {
                get
                {
                    return GetComponent<InterfaceGameObject>();
                }
            }

            #region Private implementation

            private InterfaceCallbacks iface;

            #endregion

            #region Methods for Derived Classes

            /// <summary>
            /// Call from your Start method
            /// </summary>
            protected override void Start()
            {
                base.Start ();
                if (null != iface)
                {
                    return;
                }

                iface = ScriptableObject.CreateInstance<InterfaceCallbacks>();
                iface.path = usedPath;
                iface.Start();
            }

            protected override void Stop()
            {
                base.Stop ();
                if (null != iface)
                {
                    Object.Destroy(iface);
                    iface = null;
                }
            }
            #endregion
        }
    }
}
