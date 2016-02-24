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
using System.Collections;

/// <summary>
/// Note that this derives from the OSVR.Unity.RequiresButtonInterface, and so unlike the code in HandleButtonPress.cs,
/// no class attributes are required to enforce the presence of an ButtonInterface component, nor is any
/// GetComponent usage required to get to the ButtonInterface. We can just rely on it being there and use
/// the inherited properties to access it concisely. Thus, this is the preferred technique for handling callbacks.
/// </summary>
public class SampleButton : OSVR.Unity.RequiresButtonInterface
{
    void Start()
    {
        this.Interface.StateChanged += handleButton;
    }

    void handleButton(object sender, OSVR.ClientKit.TimeValue timestamp, int sensor, byte report)
    {
        Debug.Log("[OSVR-Unity-Samples] Got button: " + sensor.ToString() + " state is " + report);
    }
}
