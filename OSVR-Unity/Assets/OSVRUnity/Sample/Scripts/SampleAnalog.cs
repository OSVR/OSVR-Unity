using UnityEngine;
using System.Collections;

public class SampleAnalog : OSVR.Unity.InterfaceBase
{

    // Use this for initialization
    void Start()
    {
        osvrInterface.RegisterCallback(callback);
    }

    void callback(string path, float value)
    {
        Debug.Log("Got analog value " + value);
    }
}
