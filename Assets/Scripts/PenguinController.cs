﻿using UnityEngine;
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
		if (Input.GetButton ("Jump") && grounded) {
			rigidbody2D.AddForce (jumpForce, ForceMode2D.Impulse);
			grounded = false;
		}
		rigidbody2D.velocity = new Vector3 (forwardVelocity, rigidbody2D.velocity.y, 0);
	}

	void Update () {
		transform.localRotation = new Quaternion (0, 0, 0, 0);
		GameObject floor = GameObject.Find ("floor");
	}

	void OnCollisionStay2D(Collision2D other) {
		if(other.gameObject.CompareTag("floor")) {
		   grounded = true;
		}
	}
	
}
