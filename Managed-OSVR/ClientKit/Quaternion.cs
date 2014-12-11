using System;
using System.Runtime.InteropServices;

namespace OSVR
{
    namespace ClientKit
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Quaternion
        {
            /*
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public double[] data;
            */
            public double w;
            public double x;
            public double y;
            public double z;
        }
    }
}

