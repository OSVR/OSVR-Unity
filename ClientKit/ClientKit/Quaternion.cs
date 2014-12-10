using System;
using System.Runtime.InteropServices;

namespace OSVR
{
	namespace ClientKit
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Quaternion
		{
			public double w;
			public double x;
			public double y;
			public double z;
		}
	}
}

