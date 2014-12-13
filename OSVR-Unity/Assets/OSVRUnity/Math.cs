using UnityEngine;

namespace OSVR
{
	namespace Unity
	{
		public class Math
		{
			public static Vector3 ConvertPosition(OSVR.ClientKit.Vec3 vec) {
				/// Convert to left-handed
				return new Vector3 ((float)vec.x, (float)vec.y, (float)-vec.z);
			}
			public static Quaternion ConvertOrientation(OSVR.ClientKit.Quaternion quat) {
				/// Quaternions are not handed
				return new Quaternion ((float)quat.x, (float)quat.y, (float)quat.z, (float)quat.w);
			}
			public static Matrix4x4 ConvertPose(OSVR.ClientKit.Pose3 pose) {
				return Matrix4x4.TRS (Math.ConvertPosition (pose.translation), Math.ConvertOrientation (pose.rotation), Vector3.zero);
			}
		}
	}
}