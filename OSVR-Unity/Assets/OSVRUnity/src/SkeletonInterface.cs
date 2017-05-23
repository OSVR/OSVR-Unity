/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2017 Sensics, Inc.
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
        /// Skeleton interface: continually (or rather, when OSVR updates) updates its position and orientation based on the incoming tracker data.
        ///
        /// Attach to a GameObject that you'd like to have updated in this way.
        /// A custom post-rotation (applyPostRotation = true) may be desired if the poses are being used with a rigged 3d model.
        /// If a post-rotation is not applied, this class is effectively a PoseInterface.
        /// </summary>
        public class SkeletonInterface : InterfaceGameObjectBase
        {
            PoseAdapter adapter;
            [SerializeField]
            private Vector3 modelForwardDir; //for a set of hands, this represents the direction the fingers are pointing
            [SerializeField]
            private Vector3 modelNormalDir; //for a set of hands, this represents the palm facing direction
            [SerializeField]
            private bool applyPostRotation = true; //for fitting to a rigged 3d model. If false, this class is identical to PoseInterface
            [SerializeField]
            private bool applyPostRotateEuler = true; //add a post rotation by multiplying by postRotateEuler, instead of using the forward and normal dirs
            [SerializeField]
            private Vector3 postRotateEuler;

            override protected void Start()
            {
                base.Start();
                if (adapter == null && !String.IsNullOrEmpty(usedPath))
                {
                    adapter = new PoseAdapter(
                        OSVR.ClientKit.PoseInterface.GetInterface(ClientKit.instance.context, usedPath));
                }
            }

            protected override void Stop()
            {
                base.Stop();
                if (adapter != null)
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
                    transform.localPosition = state.Value.Position;

                    //apply a post-rotation
                    if (applyPostRotation)
                    {
                        //if applyPostRotateEuler is true, apply the editor-defined postRotateEuler
                        if (applyPostRotateEuler)
                        {
                            transform.localRotation = state.Value.Rotation *= Quaternion.Euler(postRotateEuler);

                        }
                        else
                        {
                            //a post-rotation based on model forward and normal directions may be desired if the poses are being used with a rigged 3d model
                            transform.localRotation = state.Value.Rotation * Quaternion.Inverse(Quaternion.LookRotation(modelForwardDir, -modelNormalDir));

                        }

                    }
                    else
                    {
                        //no post-rotate
                        transform.localRotation = state.Value.Rotation;

                    }
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
