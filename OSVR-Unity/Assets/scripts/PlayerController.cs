using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed;

	// Called before physics
	void FixedUpdate () {
		float moveHoriz = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (moveHoriz, 0, moveVertical);
		rigidbody.AddForce (movement * speed * Time.deltaTime);
	}
}
