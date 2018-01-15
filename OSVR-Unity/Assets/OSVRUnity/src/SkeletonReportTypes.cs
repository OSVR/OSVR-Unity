/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2015 Sensics, Inc.
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
/// <summary>

using System;
using System.Runtime.InteropServices;
using ChannelCount = System.Int32;

namespace OSVR.Unity
{
    public struct SkeletonJointState
    {
        public OSVR.ClientKit.SkeletonJoints Joint { get; set; }
        public OSVR.Unity.Pose3 Pose { get; set; }

        public static SkeletonJointState Convert(OSVR.ClientKit.SkeletonJointState jointState)
        {
            return new SkeletonJointState
            {
                Joint = jointState.joint,
                Pose = OSVR.Unity.Math.Convert(jointState.pose),
            };
        }
    }

    public struct SkeletonJointReport
    {
        public ChannelCount Sensor { get; set; }
        public SkeletonJointState State { get; set; }

        public static SkeletonJointReport Convert(OSVR.ClientKit.SkeletonJointReport jointReport)
        {
            return new SkeletonJointReport
            {
                Sensor = jointReport.sensor,
                State = SkeletonJointState.Convert(jointReport.state),
            };
        }
    }

    public struct SkeletonTrimmedState
    {
        public SkeletonJointReport Pelvis { get; set; }
        public SkeletonJointReport Spine0 { get; set; }
        public SkeletonJointReport Spine1 { get; set; }
        public SkeletonJointReport Spine2 { get; set; }
        public SkeletonJointReport Spine3 { get; set; }
        public SkeletonJointReport Neck { get; set; }
        public SkeletonJointReport Head { get; set; }
        public SkeletonJointReport ClavicleLeft { get; set; }
        public SkeletonJointReport ArmUpperLeft { get; set; }
        public SkeletonJointReport ArmLowerLeft { get; set; }
        public SkeletonJointReport HandLeft { get; set; }
        public SkeletonJointReport HandRight { get; set; }
        public SkeletonJointReport LegUpperLeft { get; set; }
        public SkeletonJointReport LegLowerLeft { get; set; }
        public SkeletonJointReport FootLeft { get; set; }
        public SkeletonJointReport ToesLeft { get; set; }
        public SkeletonJointReport LegUpperRight { get; set; }
        public SkeletonJointReport LegLowerRight { get; set; }
        public SkeletonJointReport FootRight { get; set; }
        public SkeletonJointReport ToesRight { get; set; }

        public static SkeletonTrimmedState Convert(OSVR.ClientKit.SkeletonTrimmedState trimmedState)
        {
            return new SkeletonTrimmedState
            {
                Pelvis = SkeletonJointReport.Convert(trimmedState.pelvis),
                Spine0 = SkeletonJointReport.Convert(trimmedState.spine0),
                Spine1 = SkeletonJointReport.Convert(trimmedState.spine1),
                Spine2 = SkeletonJointReport.Convert(trimmedState.spine2),
                Spine3 = SkeletonJointReport.Convert(trimmedState.spine3),
                Neck = SkeletonJointReport.Convert(trimmedState.neck),
                Head = SkeletonJointReport.Convert(trimmedState.head),
                ClavicleLeft = SkeletonJointReport.Convert(trimmedState.clavicleLeft),
                ArmUpperLeft = SkeletonJointReport.Convert(trimmedState.armUpperLeft),
                ArmLowerLeft = SkeletonJointReport.Convert(trimmedState.armLowerLeft),
                HandLeft = SkeletonJointReport.Convert(trimmedState.handLeft),
                HandRight = SkeletonJointReport.Convert(trimmedState.handRight),
                LegUpperLeft = SkeletonJointReport.Convert(trimmedState.legUpperLeft),
                LegLowerLeft = SkeletonJointReport.Convert(trimmedState.legLowerLeft),
                FootLeft = SkeletonJointReport.Convert(trimmedState.footLeft),
                ToesLeft = SkeletonJointReport.Convert(trimmedState.toesLeft),
                LegUpperRight = SkeletonJointReport.Convert(trimmedState.legUpperRight),
                LegLowerRight = SkeletonJointReport.Convert(trimmedState.legLowerRight),
                FootRight = SkeletonJointReport.Convert(trimmedState.footRight),
                ToesRight = SkeletonJointReport.Convert(trimmedState.toesRight),
            };
        }
    }

    public struct SkeletonWholeState
    {
        public SkeletonJointReport Pelvis { get; set; }
        public SkeletonJointReport Spine0 { get; set; }
        public SkeletonJointReport Spine1 { get; set; }
        public SkeletonJointReport Spine2 { get; set; }
        public SkeletonJointReport Spine3 { get; set; }
        public SkeletonJointReport Neck { get; set; }
        public SkeletonJointReport Head { get; set; }
        public SkeletonArmState LeftArm { get; set; }
        public SkeletonArmState RightArm { get; set; }
        public SkeletonLegState LeftLeg { get; set; }
        public SkeletonLegState RightLeg { get; set; }

        public static SkeletonWholeState Convert(OSVR.ClientKit.SkeletonWholeState wholeState)
        {
            return new SkeletonWholeState
            {
                Pelvis = SkeletonJointReport.Convert(wholeState.pelvis),
                Spine0 = SkeletonJointReport.Convert(wholeState.pelvis),
                Spine1 = SkeletonJointReport.Convert(wholeState.pelvis),
                Spine2 = SkeletonJointReport.Convert(wholeState.pelvis),
                Spine3 = SkeletonJointReport.Convert(wholeState.pelvis),
                Neck = SkeletonJointReport.Convert(wholeState.pelvis),
                Head = SkeletonJointReport.Convert(wholeState.pelvis),
                LeftArm = SkeletonArmState.Convert(wholeState.leftArm),
                RightArm = SkeletonArmState.Convert(wholeState.rightArm),
                LeftLeg = SkeletonLegState.Convert(wholeState.leftLeg),
                RightLeg = SkeletonLegState.Convert(wholeState.rightLeg),
            };
        }
    }

    public struct SkeletonHandState
    {
        public SkeletonJointReport Hand { get; set; }
        public SkeletonJointReport ThumbProximal { get; set; }
        public SkeletonJointReport ThumbMedial { get; set; }
        public SkeletonJointReport ThumbDistal { get; set; }
        public SkeletonJointReport IndexProximal { get; set; }
        public SkeletonJointReport IndexMedial { get; set; }
        public SkeletonJointReport IndexDistal { get; set; }
        public SkeletonJointReport MiddleProximal { get; set; }
        public SkeletonJointReport MiddleMedial { get; set; }
        public SkeletonJointReport MiddleDistal { get; set; }
        public SkeletonJointReport RingProximal { get; set; }
        public SkeletonJointReport RingMedial { get; set; }
        public SkeletonJointReport RingDistal { get; set; }
        public SkeletonJointReport PinkyProximal { get; set; }
        public SkeletonJointReport PinkyMedial { get; set; }
        public SkeletonJointReport PinkyDistal { get; set; }

        public static SkeletonHandState Convert(OSVR.ClientKit.SkeletonHandState handState)
        {
            return new SkeletonHandState
            {
                Hand = SkeletonJointReport.Convert(handState.hand),
                ThumbProximal = SkeletonJointReport.Convert(handState.thumbProximal),
                ThumbMedial = SkeletonJointReport.Convert(handState.thumbMedial),
                ThumbDistal = SkeletonJointReport.Convert(handState.thumbDistal),
                IndexProximal = SkeletonJointReport.Convert(handState.indexProximal),
                IndexMedial = SkeletonJointReport.Convert(handState.indexMedial),
                IndexDistal = SkeletonJointReport.Convert(handState.indexDistal),
                MiddleProximal = SkeletonJointReport.Convert(handState.middleProximal),
                MiddleMedial = SkeletonJointReport.Convert(handState.middleMedial),
                MiddleDistal = SkeletonJointReport.Convert(handState.middleDistal),
                RingProximal = SkeletonJointReport.Convert(handState.ringProximal),
                RingMedial = SkeletonJointReport.Convert(handState.ringMedial),
                RingDistal = SkeletonJointReport.Convert(handState.ringDistal),
                PinkyProximal = SkeletonJointReport.Convert(handState.pinkyProximal),
                PinkyMedial = SkeletonJointReport.Convert(handState.pinkyMedial),
                PinkyDistal = SkeletonJointReport.Convert(handState.pinkyDistal),
            };
        }
    }

    public struct SkeletonArmState
    {
        public SkeletonJointReport Clavicle { get; set; }
        public SkeletonJointReport ArmUpper { get; set; }
        public SkeletonJointReport ArmLower { get; set; }
        public SkeletonHandState Hand { get; set; }

        public static SkeletonArmState Convert(OSVR.ClientKit.SkeletonArmState armState)
        {
            return new SkeletonArmState
            {
                Clavicle = SkeletonJointReport.Convert(armState.clavicle),
                ArmUpper = SkeletonJointReport.Convert(armState.armUpper),
                ArmLower = SkeletonJointReport.Convert(armState.armLower),
                Hand = SkeletonHandState.Convert(armState.hand),
            };
        }
    }

    public struct SkeletonFootState
    {
        public SkeletonJointReport Foot { get; set; }
        public SkeletonJointReport Toes { get; set; }

        public static SkeletonFootState Convert(OSVR.ClientKit.SkeletonFootState footState)
        {
            return new SkeletonFootState
            {
                Foot = SkeletonJointReport.Convert(footState.foot),
                Toes = SkeletonJointReport.Convert(footState.toes),
            };
        }
    }

    public struct SkeletonLegState
    {
        public SkeletonJointReport LegUpper { get; set; }
        public SkeletonJointReport LegLower { get; set; }
        public SkeletonFootState Foot { get; set; }

        public static SkeletonLegState Convert(OSVR.ClientKit.SkeletonLegState legState)
        {
            return new SkeletonLegState
            {
                LegUpper = SkeletonJointReport.Convert(legState.legUpper),
                LegLower = SkeletonJointReport.Convert(legState.legLower),
                Foot = SkeletonFootState.Convert(legState.foot),
            };
        }
    }
}
