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
/// 
/// This class is a modified version of MouseLook.cs distributed with Unity's Standard Assets package.
/// The class name has been changed to avoid naming conflict and confusion.

using UnityEngine;
using System.Collections;

namespace OSVR
{
    namespace Unity
    {
        // OsvrMouseLook rotates the transform based on the mouse delta.
        // Minimum and Maximum values can be used to constrain the possible rotation
        // Attach this script to an FPS style character.
        public class OsvrMouseLook : MonoBehaviour
        {
            public KeyCode MouseLookToggleKey = KeyCode.M;
            public bool useMouseLook = false;
            public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2, RightJoystick = 3 }
            public RotationAxes axes = RotationAxes.MouseXAndY;
            public float sensitivityX = 15F;
            public float sensitivityY = 15F;

            public float minimumX = -360F;
            public float maximumX = 360F;

            public float minimumY = -60F;
            public float maximumY = 60F;

            float rotationY = 0F;
            private float lastMouseLookTime = 0;

            void Start()
            {
                // Make the rigid body not change rotation
                if (GetComponent<Rigidbody>())
                {
                    GetComponent<Rigidbody>().freezeRotation = true;
                }
            }

            void Update()
            {
                if (Input.GetKeyDown(MouseLookToggleKey) && Time.time - lastMouseLookTime > 0.3f)
                {
                    lastMouseLookTime = Time.time;
                    useMouseLook = !useMouseLook;
                }
                if (!useMouseLook)
                {
                    return;
                }
                if (axes == RotationAxes.MouseXAndY)
                {
                    float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                }
                else if (axes == RotationAxes.RightJoystick)
                {
                    if (Mathf.Abs(Input.GetAxis("Right Joystick")) > 0.08f)
                    {
                        transform.Rotate(0, Input.GetAxis("Right Joystick") * sensitivityX, 0);
                    }
                }
                else if (axes == RotationAxes.MouseY)
                {
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
                }
                else if (axes == RotationAxes.MouseX)
                {
                    float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                    transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                }
            }
        }
    }
}