using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class manages arrays of gameobjects representing left and right hands
// Keypresses and GUI buttons control cycling through models
public class SampleHandsController : MonoBehaviour {

    public TrackedObjectConfidenceManager leftHandObjectsManager;
    public TrackedObjectConfidenceManager rightHandObjectsManager;
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            leftHandObjectsManager.SetCurrentIndex(leftHandObjectsManager.CurrentIndex + 1);
            rightHandObjectsManager.SetCurrentIndex(rightHandObjectsManager.CurrentIndex + 1);

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftHandObjectsManager.SetCurrentIndex(leftHandObjectsManager.CurrentIndex - 1);
            rightHandObjectsManager.SetCurrentIndex(rightHandObjectsManager.CurrentIndex - 1);

        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("Next Hands (right arrow)"))
        {
            leftHandObjectsManager.SetCurrentIndex(leftHandObjectsManager.CurrentIndex + 1);
            rightHandObjectsManager.SetCurrentIndex(rightHandObjectsManager.CurrentIndex + 1);
        }
        if (GUILayout.Button("Prev Hands (left arrow)"))
        {
            leftHandObjectsManager.SetCurrentIndex(leftHandObjectsManager.CurrentIndex - 1);
            rightHandObjectsManager.SetCurrentIndex(rightHandObjectsManager.CurrentIndex - 1);
        }
    }
}
