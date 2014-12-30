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
        /// Pose interface: continually (or rather, when OSVR updates) updates its position and orientation based on the incoming tracker data.
        /// 
        /// Attach to a GameObject that you'd like to have updated in this way.
        /// </summary>
        public class PoseInterface : InterfaceGameObject
        {
            new void Start()
            {
                osvrInterface.RegisterCallback(callback);
            }

            private void callback(string source, Vector3 position, Quaternion rotation)
            {
                transform.localPosition = position;
                transform.localRotation = rotation;
            }
        }
    }
}