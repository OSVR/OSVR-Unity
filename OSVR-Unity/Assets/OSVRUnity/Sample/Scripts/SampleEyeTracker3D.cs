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

public class SampleEyeTracker3D : OSVR.Unity.RequiresEyeTracker3DInterface
{
    // Update is called once per frame
    void Update()
    {
        var state = this.Interface.GetState().Value;
        if (state.BasePointValid)
        {
            this.transform.localPosition = state.BasePoint;
        }
        else
        {
            this.transform.localPosition = new Vector3(0f, 0f, 0f);
        }

        if (state.DirectionValid)
        {
            this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, state.Direction);
        }
        else
        {
            this.transform.localRotation = Quaternion.identity;
        }
    }
}
