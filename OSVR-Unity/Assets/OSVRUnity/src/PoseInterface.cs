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
        public class PoseInterface : InterfaceGameObject
        {
            //private int currentFrame = 0;
            //private int frameCount = 0;
            new void Start()
            {
                osvrInterface.RegisterCallback(callback);
                //currentFrame = Time.frameCount;
                //frameCount = 0;
            }

            private void callback(string source, Vector3 position, Quaternion rotation)
            {
                transform.localPosition = position;
                transform.localRotation = rotation;
                //keeping this here for now for debugging purposes
                /*if(currentFrame != Time.frameCount)
                {
                    Debug.Log("Time.frameCount = " + currentFrame + ", Time.time = " + Time.time + ". Callbacks per frame = " + frameCount);
                    frameCount = 0;
                    currentFrame = Time.frameCount;
                }
                else
                {
                    frameCount++;
                }*/              
            }
        }
    }
}
