using UnityEngine;
using System.Collections;

public class QuitOnEscape : MonoBehaviour {

    private bool quit = false;
	// Use this for initialization
	void Start () {
        quit = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && quit == false)
        {
            quit = true;
            Application.Quit();
        }
    }
}
