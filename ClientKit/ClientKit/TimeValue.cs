using System;
using System.Runtime.InteropServices;

namespace OSVR
{
	namespace ClientKit
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct TimeValue
		{
			public Int64 seconds;
			public Int32 microseconds;
		}
	}
}

