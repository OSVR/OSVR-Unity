using System;
using System.Runtime.InteropServices;
using OSVR.ClientKit;

namespace OSVR
{
    namespace ClientKit
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Pose3
        {
            public Vec3 translation;
            public Quaternion rotation;
        }
    }
}

