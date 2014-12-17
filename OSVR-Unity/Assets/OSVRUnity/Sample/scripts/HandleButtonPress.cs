using UnityEngine;
using System.Collections;

public class HandleButtonPress : MonoBehaviour {
	public void Start() {
		GetComponent<OSVR.Unity.InterfaceCallbacks> ().RegisterCallback (handleButton);
	}

	public void handleButton(string path, bool state) {
		Debug.Log ("Got button: " + path + " state is " + state);
	}
}
