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
/// This class is a modified, translated version of InputController.js distributed with Unity's Standard Assets package.
/// The class name has been changed to avoid naming conflict and confusion.

using UnityEngine;
using System.Collections;

namespace OSVR
{
    namespace Unity
    {
        // Require a character controller to be attached to the same game object
        public class OsvrInputController : MonoBehaviour
        {
            private OsvrCharacterMotor motor;
            private Transform viewerDirection; //for moving in the direction of the Viewer

            // Use this for initialization
            void Awake()
            {
                motor = GetComponent<OsvrCharacterMotor>();
            }

            // Update is called once per frame
            void Update()
            {
                if (viewerDirection == null)
                {
                    VRViewer viewer = FindObjectOfType<VRViewer>();
                    if (viewer != null)
                    {
                        viewerDirection = viewer.transform;
                    }
                }

                // Get the input vector from keyboard or analog stick
                Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (directionVector != Vector3.zero)
                {
                    // Get the length of the directon vector and then normalize it
                    // Dividing by the length is cheaper than normalizing when we already have the length anyway
                    var directionLength = directionVector.magnitude;
                    directionVector = directionVector / directionLength;
                    if (directionVector.sqrMagnitude > 1)
                        directionVector.Normalize();

                    // Make sure the length is no bigger than 1
                    directionLength = Mathf.Min(1, directionLength);

                    // Make the input vector more sensitive towards the extremes and less sensitive in the middle
                    // This makes it easier to control slow speeds when using analog sticks
                    directionLength = directionLength * directionLength;

                    // Multiply the normalized direction vector by the modified length
                    directionVector = directionVector * directionLength;
                }


                if (viewerDirection)
                {
                    // Apply the viewer direction to the CharacterMotor
                    Vector3 theForwardDirection = viewerDirection.TransformDirection(Vector3.forward);
                    theForwardDirection.y = 0;
                    theForwardDirection.Normalize();
                    motor.inputMoveDirection = viewerDirection.rotation * directionVector;
                    motor.inputJump = Input.GetButton("Jump");
                }

            }
        }
    }
}
