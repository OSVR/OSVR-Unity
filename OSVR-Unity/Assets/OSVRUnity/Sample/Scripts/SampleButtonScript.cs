using UnityEngine;
using System.Collections;

public class SampleButtonScript : OSVR.Unity.InterfaceBase
{
    void Start()
    {
        osvrInterface.RegisterCallback(handleButton);
    }

    void handleButton(string path, bool state)
    {
        Debug.Log("Got button: " + path + " state is " + state);
    }
}
