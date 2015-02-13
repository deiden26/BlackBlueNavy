using UnityEngine;
using System.Collections.Generic;

public class PenguinController : MonoBehaviour {

	public float forwardVelocity;
	public Vector3 jumpForce;

	private roomManager roomController; 
	private bool grounded;

	// Use this for initialization
	void Start () {
		rigidbody2D.velocity = new Vector3 (1, 0, 0) * forwardVelocity;
		GameObject roomControllerObj = GameObject.Find("roomManager");
		roomController = (roomManager) roomControllerObj.GetComponent(typeof(roomManager));
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
	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("nextRoomTrigger0") || other.CompareTag("nextRoomTrigger1") || other.CompareTag("nextRoomTrigger2")) {
			roomController.enterRoom(other.tag, other.transform.position.x);
		}
		else if(other.CompareTag("midRoomTrigger")) {
			roomController.midRoom();
		}
	}
	
}
