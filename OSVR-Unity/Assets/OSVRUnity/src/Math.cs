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

using UnityEngine;

namespace OSVR
{
    namespace Unity
    {
        /// <summary>
        /// Class of static methods for converting from OSVR math/tracking types to Unity-native data types, including coordinate system change as needed.
        /// </summary>
        public class Math
        {
            public static Vector3 ConvertPosition(OSVR.ClientKit.Vec3 vec)
            {
                /// Convert to left-handed
                return new Vector3((float)vec.x, (float)vec.y, (float)-vec.z);
            }

            public static Vector2 ConvertPosition(OSVR.ClientKit.Vec2 vec)
            {
                return new Vector2((float)vec.x, (float)vec.y);
            }

            public static Quaternion ConvertOrientation(OSVR.ClientKit.Quaternion quat)
            {
                /// Wikipedia may say quaternions are not handed, but these needed modification.
                return new Quaternion(-(float)quat.x, -(float)quat.y, (float)quat.z, (float)quat.w);
            }

            public static Matrix4x4 ConvertPose(OSVR.ClientKit.Pose3 pose)
            {
                return Matrix4x4.TRS(Math.ConvertPosition(pose.translation), Math.ConvertOrientation(pose.rotation), Vector3.zero);
            }

            //@todo Rect takes left, top, width, height
            public static Rect ConvertViewport(OSVR.ClientKit.Viewport viewport)
            {
                return new Rect(viewport.Left, viewport.Bottom + viewport.Height, viewport.Width, viewport.Height);
            }

            //@todo is this correct?
            public static Matrix4x4 ConvertMatrix(OSVR.ClientKit.Matrix44f matrix)
            {
                Matrix4x4 matrix4x4 = new Matrix4x4();
                matrix4x4.m00 = matrix.M0;
                matrix4x4.m01 = matrix.M1;
                matrix4x4.m02 = matrix.M2;
                matrix4x4.m03 = matrix.M3;
                matrix4x4.m10 = matrix.M4;
                matrix4x4.m11 = matrix.M5;
                matrix4x4.m12 = matrix.M6;
                matrix4x4.m13 = matrix.M7;
                matrix4x4.m20 = matrix.M8;
                matrix4x4.m21 = matrix.M9;
                matrix4x4.m22 = matrix.M10;
                matrix4x4.m23 = matrix.M11;
                matrix4x4.m30 = matrix.M12;
                matrix4x4.m31 = matrix.M13;
                matrix4x4.m32 = matrix.M14;
                matrix4x4.m33 = matrix.M15;
                return matrix4x4;
            }
        }
    }
}
