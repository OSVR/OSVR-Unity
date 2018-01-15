﻿/// OSVR-Unity Connection
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
        /// Base class for a script that requires an OSVR interface.
        /// It primarily takes care of requiring that there is an SkeletonJointInterface component in the current game object.
        /// </summary>
        [RequireComponent(typeof(OSVR.Unity.SkeletonJointInterface))]
        public class RequiresSkeletonJointInterface : MonoBehaviour, IRequiresInterface<OSVR.Unity.SkeletonJointState>
        {
            /// <summary>
            /// Accessor for the sibling SkeletonJointInterface component.
            /// </summary>
            private SkeletonJointInterface ifaceComponent;
            public OSVR.ClientKit.IInterface<OSVR.Unity.SkeletonJointState> Interface
            {
                get
                {
                    if (ifaceComponent == null)
                    {
                        ifaceComponent = GetComponent<SkeletonJointInterface>();
                    }
                    return ifaceComponent == null ? null : ifaceComponent.Interface;
                }
            }
        }
    }
}
