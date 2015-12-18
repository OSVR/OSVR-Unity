/// OSVR-Unity Demo
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
/// 
using UnityEngine;
using System.Collections;

//This sample shows how you can handle adding Image Effects to each Camera in the scene.
//Subscribe to the event that will be fired.
//Add image effects and other components custom to your game in the HandleSurfaceCreation callback method.
public class SampleCameraEffects : MonoBehaviour {

    void OnEnable()
    {
        //subscribe to the VRSurfaceCreated event
        //this will be fired after the camera is added
        //used for adding image effects and other components specific to your game to each VRSurface (camera)
        OSVR.Unity.VREye.OnVRSurfaceCreated += HandleSurfaceCreation;
    }

    void OnDisable()
    {
        //remember to unsubscribe
        OSVR.Unity.VREye.OnVRSurfaceCreated -= HandleSurfaceCreation;
    }

    //Called when VRSurface is created
    private void HandleSurfaceCreation(OSVR.Unity.VRSurface surface)
    {
        //put your game's code here:
        //add image effect to camera, add other game components, set camera references if necessary
        //surface.gameObject.AddComponent<YourImageEffect>();
    }
}
