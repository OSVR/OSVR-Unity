/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2017 Sensics, Inc.
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
/// This class manages an array of tracked gameobjects based on confidence reports from a tracking plugin.
/// One gameobject is active at a time. If disableGameObjectBelowConfidenceThreshold = true, the active
/// gameobject will become inactive (without changing the index of the current active gameobject) if the
/// confidence report is below a threshold.
/// </summary>
public class TrackedObjectConfidenceManager : OSVR.Unity.RequiresAnalogInterface
{
    //@todo do we need a timer?
    private double confidence = 0; //confidence measure as reported in tracking plugin
    public double threshold = 0.1; //minimum value at which we keep a tracked gameobject active
    public GameObject[] trackedGameObjects; //an array of tracked gameobjects
    [SerializeField]
    private int m_currentIndex = 0; //index of the currently active gameobject in trackedGameObjects
    public bool disableGameObjectBelowConfidenceThreshold = true; //whether or not we want to disable a gameobject based on confidence report

    //get the index of the current active gameobject in the array of trackedGameObjects
    public int CurrentIndex
    {
        get { return m_currentIndex; }
    }

    //set the index of the current active gameobject
    public void SetCurrentIndex(int index)
    {
        if(index > trackedGameObjects.Length - 1)
        {
            m_currentIndex = 0;
        }
        else if(index < 0)
        {
            m_currentIndex = trackedGameObjects.Length - 1;
        }
        else
        {
            m_currentIndex = index;
        }
    }

    // Use this for initialization
    void Start()
    {
        this.Interface.StateChanged += Interface_StateChanged;
    }

    void Interface_StateChanged(object sender, OSVR.ClientKit.TimeValue timestamp, int sensor, double report)
    {
        if(trackedGameObjects.Length == 0 || trackedGameObjects[CurrentIndex] == null)
        {
            return;
        }
        if(disableGameObjectBelowConfidenceThreshold)
        {
            confidence = report;
            //disable the active gameobject if confidence is below the threshold
            if (confidence < threshold)
            {

                //SetActive() may not be the best choice here, but this is where you put code to handle
                //a loss of tracking confidence
                if (trackedGameObjects[CurrentIndex].activeSelf)
                {
                    trackedGameObjects[CurrentIndex].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < trackedGameObjects.Length; i++)
                {
                    if (i == CurrentIndex)
                    {
                        if (!trackedGameObjects[i].activeSelf)
                        {
                            trackedGameObjects[i].SetActive(true);
                        }
                    }
                    else
                    {
                        if (trackedGameObjects[i].activeSelf)
                        {
                            trackedGameObjects[i].SetActive(false);
                        }
                    }
                }
            }
        }
        
    }
}
