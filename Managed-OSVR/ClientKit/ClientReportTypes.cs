/// Managed-OSVR binding
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

﻿using System;
using System.Runtime.InteropServices;
using OSVR.ClientKit;

namespace OSVR
{
    namespace ClientKit
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionReport
        {
            public Int32 sensor;
            public Vec3 xyz;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OrientationReport
        {
            public Int32 sensor;
            public Quaternion rotation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PoseReport
        {
            public Int32 sensor;
            public Pose3 pose;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ButtonReport
        {
            public Int32 sensor;
            public Byte state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AnalogReport
        {
            public Int32 sensor;
            public Double state;
        }
    }

}
