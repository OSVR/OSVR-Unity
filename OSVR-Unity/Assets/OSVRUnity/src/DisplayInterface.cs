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
            private string _deviceDescriptorJson; //a string that is the JSON file to be parsed

            //_initalized exists to make sure the display config has been parsed before trying to read it
            public bool Initialized
            {
                get { return _initialized; }
            }
            private bool _initialized = false; //flag set when _deviceDescriptorJson has data from /display

            void Start()
            {
                ReadDisplayPath();
            }

            //this function attemtps to retrieve display configuration from the /display path
            //this data is in JSON format. Once it has initialized, the data is used to configure Camera and other settings.
            public void ReadDisplayPath()
            {
                if (!_initialized)
                {
                    _deviceDescriptorJson = ClientKit.instance.context.getStringParameter("/display");
                    if (_deviceDescriptorJson != "")
                    {
                        _initialized = true;
                    }
                }
            }

            /// <summary>
            /// This function will parse the Json display parameters from /display using Newstonsoft
            ///
            /// Returns a DeviceDescriptor object containing stored json values.
            /// </summary>
            public DeviceDescriptor GetDeviceDescription()
            {
                return _deviceDescriptorJson == null ? null : DeviceDescriptor.Parse(_deviceDescriptorJson);               
            }
        }
    }
}
