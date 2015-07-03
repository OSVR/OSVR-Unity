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

namespace OSVR.Unity
{
    public class OrientationAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.Quaternion, UnityEngine.Quaternion>
    {
        public OrientationAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.Quaternion> iface) : base(iface) {}
        protected override UnityEngine.Quaternion Convert (OSVR.ClientKit.Quaternion sourceValue)
        {
            return OSVR.Unity.Math.ConvertOrientation(sourceValue);
        }
    }

    public class PositionAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.Vec3, UnityEngine.Vector3>
    {
        public PositionAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.Vec3> iface) : base(iface) {}
        protected override UnityEngine.Vector3 Convert (OSVR.ClientKit.Vec3 sourceValue)
        {
            return OSVR.Unity.Math.ConvertPosition(sourceValue);
        }
    }

    public struct Pose3
    {
        public UnityEngine.Quaternion Rotation { get; set; }
        public UnityEngine.Vector3 Position { get; set; }
    }

    public class PoseAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.Pose3, OSVR.Unity.Pose3>
    {
        public PoseAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.Pose3> iface) : base(iface) {}
        protected override Pose3 Convert (OSVR.ClientKit.Pose3 sourceValue)
        {
            return new Pose3
            {
                Rotation = OSVR.Unity.Math.ConvertOrientation(sourceValue.rotation),
                Position = OSVR.Unity.Math.ConvertPosition (sourceValue.translation),
            };
        }
    }
}
