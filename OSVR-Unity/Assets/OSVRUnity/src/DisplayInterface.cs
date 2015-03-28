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
using System.Collections;

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
            const string HmdJsonFileName = "hmd.json"; //hardcoded filename of hmd config in Data folder
            private string _deviceDescriptorJson; //a string that is the JSON file to be parsed
            public TextAsset JsonDescriptorFile; //drop the json file into this slot in the Unity inspector
            public bool Initialized
            {
                get { return _initialized; }
            }
            private bool _initialized = false; //flag set when _deviceDescriptorJson has data

            void Awake()
            {
                //check to see if "hmd.json" is provided in the Data folder
                //if so, load it
                string filePath = Application.dataPath + "/" + HmdJsonFileName;
                if (System.IO.File.Exists(filePath))
                {
                    StartCoroutine(LoadJsonFile(filePath));
                }
                else //if not, load the json file provided in the Unity editor
                {
                    if (JsonDescriptorFile != null)
                    {
                        _deviceDescriptorJson = JsonDescriptorFile.text; //read JSON file directly from Unity if provided
                    }
                    else
                    {
                        _deviceDescriptorJson = ClientKit.instance.context.getStringParameter("/display"); //otherwise read from /display
                    }
                    _initialized = true;
                }  
            }

            //coroutine for loading an external json config file
            //this could be more generic, but I'm not sure we will be loading external files
            //this will eventually go away anyway when we get display config data from /display
            private IEnumerator LoadJsonFile(string filePath)
            {
                WWW jsonFile = new WWW("file://" + filePath);
                yield return jsonFile;
                _initialized = true;
                _deviceDescriptorJson = jsonFile.text;
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
                DeviceDescriptor deviceDescriptor = DeviceDescriptor.Parse(_deviceDescriptorJson);
                if (deviceDescriptor != null) {
                    if (JsonDescriptorFile != null) {
                        deviceDescriptor.FileName = JsonDescriptorFile.name;
                    } else {
                        deviceDescriptor.FileName = "No descriptor file has been assigned. Using parameters from /display";
                    }
                }
                return deviceDescriptor;
            }
        }
    }
}
