using System;
using System.Runtime.InteropServices;

namespace OSVR
{
	namespace ClientKit
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Vec3
		{
			public double x;
			public double y;
			public double z;
		}
	}
}

