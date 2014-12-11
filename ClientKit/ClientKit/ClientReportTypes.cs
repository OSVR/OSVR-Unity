using System;
using System.Runtime.InteropServices;
using OSVR.ClientKit;

namespace OSVR
{
	namespace ClientKit
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct PositionReport {
			public Int32 sensor;
			public Vec3 xyz;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct OrientationReport {
			public Int32 sensor;
			public Quaternion rotation;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PoseReport {
			public Int32 sensor;
			public Pose3 pose;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ButtonReport {
			public Int32 sensor;
			public Byte state;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct AnalogReport {
			public Int32 sensor;
			public Double state;
		}
	}

}

