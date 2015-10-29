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
            return OSVR.Unity.Math.Convert(sourceValue);
        }
    }

    public class Location2DAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.Vec2, UnityEngine.Vector2>
    {
        public Location2DAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.Vec2> iface) : base(iface) {}
        protected override UnityEngine.Vector2 Convert (OSVR.ClientKit.Vec2 sourceValue)
        {
            return OSVR.Unity.Math.ConvertPosition(sourceValue);
        }
    }

    public struct EyeTracker3DState
    {
        public bool DirectionValid { get; set; }
        public UnityEngine.Vector3 Direction { get; set; }
        public bool BasePointValid { get; set; }
        public UnityEngine.Vector3 BasePoint { get; set; }
    }

    public class EyeTracker3DAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.EyeTracker3DState, EyeTracker3DState>
    {
        public EyeTracker3DAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.EyeTracker3DState> iface) : base(iface) { }
        protected override EyeTracker3DState Convert(OSVR.ClientKit.EyeTracker3DState sourceValue)
        {
            return new EyeTracker3DState
            {
                BasePoint = Math.ConvertPosition(sourceValue.basePoint),
                BasePointValid = sourceValue.basePointValid,
                Direction = Math.ConvertPosition(sourceValue.direction),
                DirectionValid = sourceValue.directionValid,
            };
        }
    }

    public class SkeletonJointAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.SkeletonJointState, SkeletonJointState>
    {
        public SkeletonJointAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.SkeletonJointState> iface) : base(iface) { }
        protected override SkeletonJointState Convert(OSVR.ClientKit.SkeletonJointState sourceValue)
        {
            return SkeletonJointState.Convert(sourceValue);
        }
    }

    public class SkeletonTrimmedAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.SkeletonTrimmedState, SkeletonTrimmedState>
    {
        public SkeletonTrimmedAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.SkeletonTrimmedState> iface) : base(iface) { }
        protected override SkeletonTrimmedState Convert(OSVR.ClientKit.SkeletonTrimmedState sourceValue)
        {
            return SkeletonTrimmedState.Convert(sourceValue);
        }
    }

    public class SkeletonWholeAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.SkeletonWholeState, SkeletonWholeState>
    {
        public SkeletonWholeAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.SkeletonWholeState> iface) : base(iface) { }
        protected override SkeletonWholeState Convert(OSVR.ClientKit.SkeletonWholeState sourceValue)
        {
            return SkeletonWholeState.Convert(sourceValue);
        }
    }

    public class SkeletonHandAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.SkeletonHandState, SkeletonHandState>
    {
        public SkeletonHandAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.SkeletonHandState> iface) : base(iface) { }
        protected override SkeletonHandState Convert(OSVR.ClientKit.SkeletonHandState sourceValue)
        {
            return SkeletonHandState.Convert(sourceValue);
        }
    }

    public class SkeletonArmAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.SkeletonArmState, SkeletonArmState>
    {
        public SkeletonArmAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.SkeletonArmState> iface) : base(iface) { }
        protected override SkeletonArmState Convert(OSVR.ClientKit.SkeletonArmState sourceValue)
        {
            return SkeletonArmState.Convert(sourceValue);
        }
    }

    public class SkeletonFootAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.SkeletonFootState, SkeletonFootState>
    {
        public SkeletonFootAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.SkeletonFootState> iface) : base(iface) { }
        protected override SkeletonFootState Convert(OSVR.ClientKit.SkeletonFootState sourceValue)
        {
            return SkeletonFootState.Convert(sourceValue);
        }
    }

    public class SkeletonLegAdapter :
        OSVR.ClientKit.InterfaceAdapter<OSVR.ClientKit.SkeletonLegState, SkeletonLegState>
    {
        public SkeletonLegAdapter(OSVR.ClientKit.IInterface<OSVR.ClientKit.SkeletonLegState> iface) : base(iface) { }
        protected override SkeletonLegState Convert(OSVR.ClientKit.SkeletonLegState sourceValue)
        {
            return SkeletonLegState.Convert(sourceValue);
        }
    }
}
