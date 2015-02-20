using UnityEngine;
using System.Collections;

/// <summary>
/// This is a manual way of accessing the InterfaceGameObject component. See SampleButtonScript for the recommended, more elegant solution.
/// </summary>
[RequireComponent(typeof(OSVR.Unity.InterfaceGameObject))]
public class HandleButtonPress : MonoBehaviour
{
    public void Start()
    {
        gameObject.GetComponent<OSVR.Unity.InterfaceGameObject>().osvrInterface.RegisterCallback(handleButton);
    }

    public void handleButton(string path, bool state)
    {
        Debug.Log("Got button: " + path + " state is " + state);
    }
}
