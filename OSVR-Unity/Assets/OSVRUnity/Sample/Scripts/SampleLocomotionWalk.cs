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
/// A simple script to update the local position of the object based on the
/// locomotion position, mapped to the X-Z plane.
/// </summary>
public class SampleLocomotionWalk : OSVR.Unity.RequiresNaviPositionInterface
{
    // Update is called once per frame
    void Update()
    {
		if (this.Interface != null)
		{
			var state = this.Interface.GetState ();
			this.transform.localPosition = new Vector3
	        {
	            x = state.Value.x,
	            y = this.transform.localPosition.y,
	            z = state.Value.y,
	        };
		}
    }
}
