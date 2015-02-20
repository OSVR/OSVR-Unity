using UnityEngine;
using System.Collections;

/// <summary>
/// Note that this derives from the OSVR.Unity.InterfaceBase, and so unlike the code in HandleButtonPress.cs,
/// no class attributes are required to enforce the presence of an InterfaceGameObject component, nor is any
/// GetComponent usage required to get to the InterfaceGameObject. We can just rely on it being there and use
/// the inherited properties to access it concisely. Thus, this is the preferred technique for handling callbacks.
/// </summary>
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
