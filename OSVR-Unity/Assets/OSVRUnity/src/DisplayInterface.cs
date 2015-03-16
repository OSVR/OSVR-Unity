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

using System;
using System.Text;
using UnityEngine;
using SimpleJSON;

namespace OSVR
{
    namespace Unity
    {
        /// <summary>
        /// Display interface: provides information about the device display that is needed to set correct values in Unity scene.
        ///
        /// Currently parses a JSON descriptor file provided by developer in the Unity scene.
        /// If a file is not provided, it reads from /display, which will eventually be the default way that this class works.
        /// </summary>
        public class DisplayInterface : MonoBehaviour
        {
            private string _deviceDescriptorJson;
            public TextAsset JsonDescriptorFile;
            void Awake()
            {
                if (JsonDescriptorFile != null)
                {
                    _deviceDescriptorJson = JsonDescriptorFile.text; //read JSON file directly from Unity if provided
                }
                else
                {
                    _deviceDescriptorJson = ClientKit.instance.context.getStringParameter("/display"); //otherwise read from /display
                }
            }

            /// <summary>
            /// This function will parse the device parameters from a device descriptor json file.
            ///
            /// Returns a DeviceDescriptor object containing stored json values.
            /// </summary>
            public DeviceDescriptor GetDisplayParameters(TextAsset jsonDescriptor)
            {
                _deviceDescriptorJson = jsonDescriptor.text;
                return GetDeviceDescription();
            }

            /// <summary>
            /// This function will parse the device parameters from a device descriptor json file.
            ///
            /// Returns a DeviceDescriptor object containing stored json values.
            /// </summary>
            public DeviceDescriptor GetDeviceDescription()
            {

                JSONNode parsedJsonDisplayParams = JSON.Parse(_deviceDescriptorJson);

                //field of view
                float monocularHorizontal = parsedJsonDisplayParams["hmd"]["field_of_view"]["monocular_horizontal"].AsFloat;
                float monocularVertical = parsedJsonDisplayParams["hmd"]["field_of_view"]["monocular_vertical"].AsFloat;
                float overlapPercent = parsedJsonDisplayParams["hmd"]["field_of_view"]["overlap_percent"].AsFloat;
                float pitchTilt = parsedJsonDisplayParams["hmd"]["field_of_view"]["pitch_tilt"].AsFloat;

                //resolutions
                int width = parsedJsonDisplayParams["hmd"]["resolutions"][0]["width"].AsInt;
                int height = parsedJsonDisplayParams["hmd"]["resolutions"][0]["height"].AsInt;
                int videoInputs = parsedJsonDisplayParams["hmd"]["resolutions"][0]["video_inputs"].AsInt;
                string displayMode = parsedJsonDisplayParams["hmd"]["resolutions"][0]["display_mode"].Value;

                //distortion
                float k1Red = parsedJsonDisplayParams["hmd"]["distortion"]["k1_red"].AsFloat;
                float k1Green = parsedJsonDisplayParams["hmd"]["distortion"]["k1_red"].AsFloat;
                float k1Blue = parsedJsonDisplayParams["hmd"]["distortion"]["k1_red"].AsFloat;

                //rendering
                float leftRoll = parsedJsonDisplayParams["hmd"]["rendering"]["left_roll"].AsFloat;
                float rightRoll = parsedJsonDisplayParams["hmd"]["rendering"]["right_roll"].AsFloat;

                //eyes
                float centerProjX = parsedJsonDisplayParams["hmd"]["eyes"][0]["center_proj_x"].AsFloat;
                float centerProjY = parsedJsonDisplayParams["hmd"]["eyes"][0]["center_proj_y"].AsFloat;
                int rotate180 = parsedJsonDisplayParams["hmd"]["eyes"][0]["rotate_180"].AsInt;

                //print
                StringBuilder jsonPrinter = new StringBuilder(64);
                jsonPrinter.Append("FIELD OF VIEW:\n")
                .Append("monocular_horizontal = ").AppendLine( monocularHorizontal.toString() )
                .Append("monocular_vertical = ").AppendLine( monocularVertical.toString() )
                .Append("overlap_percent = ").AppendLine( overlapPercent.toString() )
                .Append("pitch_tilt = ").AppendLine( pitchTilt.toString() )
                .Append("\nRESOLUTION\n")
                .Append("width = ").AppendLine( width.toString() )
                .Append("height = ").AppendLine( height.toString() )
                .Append("video_inputs = ").AppendLine( videoInputs.toString() )
                .Append("display_mode = ").AppendLine( displayMode )
                .Append("\nDISTORTION\n")
                .Append("k1_red = ").AppendLine( k1Red.toString() )
                .Append("k1_green = ").AppendLine( k1Green.toString() )
                .Append("k1_blue = ").AppendLine( k1Blue.toString() )
                .Append("\nRENDERING\n")
                .Append("left_roll = ").AppendLine( leftRoll.toString() )
                .Append("right_roll = ").AppendLine( rightRoll.toString() )
                .Append("\nEYES\n")
                .Append("center_proj_x = ").AppendLine( centerProjX.toString() )
                .Append("center_proj_y = ").AppendLine( centerProjY.toString() )
                .Append("rotate_180 = ").AppendLine( rotate180.toString() );
                Debug.Log("Parsed " + JsonDescriptorFile.name + ".json:\n" + jsonPrinter.ToString());
                Debug.Log("Parsed " + JsonDescriptorFile.name + ".json:\n" + jsonPrinter.ToString());

                //create a device descriptor object and store the parsed json
                DeviceDescriptor deviceDescriptor = new DeviceDescriptor();
                deviceDescriptor.MonocularHorizontal = monocularHorizontal;
                deviceDescriptor.MonocularVertical = monocularVertical;
                deviceDescriptor.OverlapPercent = overlapPercent;
                deviceDescriptor.PitchTilt = pitchTilt;
                deviceDescriptor.Width = width;
                deviceDescriptor.Height = height;
                deviceDescriptor.VideoInputs = videoInputs;
                deviceDescriptor.DisplayMode = displayMode;
                deviceDescriptor.K1Red = k1Red;
                deviceDescriptor.K1Green = k1Green;
                deviceDescriptor.K1Blue = k1Blue;
                deviceDescriptor.LeftRoll = leftRoll;
                deviceDescriptor.RightRoll = rightRoll;
                deviceDescriptor.CenterProjX = centerProjX;
                deviceDescriptor.CenterProjY = centerProjY;
                deviceDescriptor.Rotate180 = rotate180;
                return deviceDescriptor;
            }
        }
    }
}
