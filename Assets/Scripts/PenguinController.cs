using UnityEngine;
using System.Collections.Generic;

public class PenguinController : MonoBehaviour {

	/*~~~~~~ events ~~~~~~*/
	
	public delegate void enterRoomAction(string tag, float newRoomPosX);
	public static event enterRoomAction onEnterRoom;

	public delegate void midRoomAction();
	public static event midRoomAction onMidRoom;

	/*~~~~~~ public variables~~~~~~*/

	public float forwardVelocity;
	public Vector3 jumpForce;

	/*~~~~~~ private variables ~~~~~~*/

	private bool grounded;

	/*~~~~~~ unity functions ~~~~~~*/
	
	void Start () {
		rigidbody2D.velocity = new Vector3 (1, 0, 0) * forwardVelocity;
	}

	void FixedUpdate () {
		if (Input.GetButton ("Jump") && grounded) {
			rigidbody2D.AddForce (jumpForce, ForceMode2D.Impulse);
			//grounded = false;
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
			onEnterRoom(other.tag, other.transform.position.x);
		}
		else if(other.CompareTag("midRoomTrigger")) {
			onMidRoom();
		}
	}
	
}
