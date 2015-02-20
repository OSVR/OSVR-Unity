using UnityEngine;
using System.Collections;

/// <summary>
/// Note that this derives from the OSVR.Unity.InterfaceBase, and so unlike the code in HandleButtonPress.cs,
/// no class attributes are required to enforce the presence of an InterfaceGameObject component, nor is any
/// GetComponent usage required to get to the InterfaceGameObject. We can just rely on it being there and use
/// the inherited properties to access it concisely. Thus, this is the preferred technique for handling callbacks.
/// </summary>
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
