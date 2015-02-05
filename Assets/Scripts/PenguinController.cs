using UnityEngine;
using System.Collections.Generic;

public class PenguinController : MonoBehaviour {

	public float forwardVelocity;
	public Vector3 jumpForce;

	private bool grounded;

	// Use this for initialization
	void Start () {
		rigidbody2D.velocity = new Vector3 (1, 0, 0) * forwardVelocity;
	}

	void FixedUpdate () {
		if (Input.GetButton ("Jump") && grounded)
			rigidbody2D.AddForce (jumpForce, ForceMode2D.Impulse);
	}


	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.CompareTag("floor")) {
		   grounded = true;
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		if(other.gameObject.CompareTag("floor")) {
			grounded = false;
		}
	}
	
}
