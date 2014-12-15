using UnityEngine;
using System.Collections;

public class HandleButtonPress : MonoBehaviour {
	public OSVR.Unity.OSVRClientKit clientKit;
	
	public OSVR.Unity.InterfaceCallbacks cb;

	public void Start() {
		GetComponent<OSVR.Unity.InterfaceCallbacks> ().RegisterCallback (handleButton);
	}

	public void handleButton(string path, bool state) {
		Debug.Log ("Got button: " + path + " state is " + state);
	}

}
