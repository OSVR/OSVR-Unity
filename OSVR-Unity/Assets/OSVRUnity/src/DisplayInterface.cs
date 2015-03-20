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
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

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
            private string _deviceDescriptorJson; //a string that is the JSON file to be parsed
            public TextAsset JsonDescriptorFile; //drop the json file into this slot in the Unity inspector
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
            /// This function will parse the device parameters from a device descriptor json file using Newstonsoft
            ///
            /// Returns a DeviceDescriptor object containing stored json values.
            /// </summary>
            public DeviceDescriptor GetDeviceDescription()
            {
                //create a device descriptor object for storing the parsed json in an object
                DeviceDescriptor deviceDescriptor;
                JsonTextReader reader;
               

                reader = new JsonTextReader(new StringReader(_deviceDescriptorJson));
                if(reader != null)
                {
                    deviceDescriptor = new DeviceDescriptor();
                }
                else
                {
                    Debug.LogError("No Device Descriptor detected.");
                    return null;
                }
                if(JsonDescriptorFile != null)
                {
                    deviceDescriptor.FileName = JsonDescriptorFile.name;
                }
                else
                {
                    deviceDescriptor.FileName = "No descriptor file has been assigned. Using parameters from /display";
                }
                
                //parsey
                while (reader.Read())
                {
                    if (reader.Value != null && reader.ValueType == typeof(String))
                    {
                        string parsedJson = reader.Value.ToString().ToLower();
                        switch(parsedJson)
                        {
                            case "vendor":
                                deviceDescriptor.Vendor = reader.ReadAsString();
                                break;
                            case "model":
                                deviceDescriptor.Model = reader.ReadAsString();
                                break;
                            case "version":
                                deviceDescriptor.Version = reader.ReadAsString();
                                break;
                            case "note":
                                deviceDescriptor.Note = reader.ReadAsString();
                                break;
                            case "monocular_horizontal":
                                deviceDescriptor.MonocularHorizontal = float.Parse(reader.ReadAsString());
                                break;
                            case "monocular_vertical":
                                deviceDescriptor.MonocularVertical = float.Parse(reader.ReadAsString());
                                break;
                            case "overlap_percent":
                                deviceDescriptor.OverlapPercent = float.Parse(reader.ReadAsString());
                                break;
                            case "pitch_tilt":
                                deviceDescriptor.PitchTilt = float.Parse(reader.ReadAsString());
                                break;
                            case "width":
                                deviceDescriptor.Width = int.Parse(reader.ReadAsString());
                                break;
                            case "height":
                                deviceDescriptor.Height = int.Parse(reader.ReadAsString());
                                break;
                            case "video_inputs":
                                deviceDescriptor.VideoInputs = int.Parse(reader.ReadAsString());
                                break;
                            case "display_mode":
                                deviceDescriptor.DisplayMode = reader.ReadAsString();
                                break;
                            case "k1_red":
                                deviceDescriptor.K1Red = float.Parse(reader.ReadAsString());
                                break;
                            case "k1_green":
                                deviceDescriptor.K1Green = float.Parse(reader.ReadAsString());
                                break;
                            case "k1_blue":
                                deviceDescriptor.K1Blue = float.Parse(reader.ReadAsString());
                                break;
                            case "right_roll":
                                deviceDescriptor.RightRoll = float.Parse(reader.ReadAsString());
                                break;
                            case "left_roll":
                                deviceDescriptor.LeftRoll = float.Parse(reader.ReadAsString());
                                break;
                            case "center_proj_x":
                                deviceDescriptor.CenterProjX = float.Parse(reader.ReadAsString());
                                break;
                            case "center_proj_y":
                                deviceDescriptor.CenterProjY = float.Parse(reader.ReadAsString());
                                break;
                            case "rotate_180":
                                deviceDescriptor.Rotate180 = int.Parse(reader.ReadAsString());
                                break;
                        }
                    }
                }

                return deviceDescriptor;
            }
        }
    }
}
