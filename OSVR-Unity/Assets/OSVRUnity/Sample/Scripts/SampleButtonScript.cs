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
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Note that this derives from the OSVR.Unity.InterfaceBase, and so unlike the code in HandleButtonPress.cs,
/// no class attributes are required to enforce the presence of an InterfaceGameObject component, nor is any
/// GetComponent usage required to get to the InterfaceGameObject. We can just rely on it being there and use
/// the inherited properties to access it concisely. Thus, this is the preferred technique for handling callbacks.
/// </summary>
public class SampleButtonScript : OSVR.Unity.InterfaceBase
{
    public UnityEvent OnPress;
    public UnityEvent OnRelease;

    void Start()
    {
        osvrInterface.RegisterCallback(handleButton);
    }

    void handleButton(string path, bool state)
    {
        Debug.Log("Got button: " + path + " state is " + state);
        if(state) OnPress.Invoke();
        else OnRelease.Invoke();
    }
}
