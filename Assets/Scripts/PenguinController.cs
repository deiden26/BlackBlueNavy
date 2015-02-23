﻿using UnityEngine;
using System.Collections.Generic;

public class PenguinController : MonoBehaviour {

	/*~~~~~~ events ~~~~~~*/
	
	public delegate void enterRoomAction(int nextRoomIndex, float newRoomPosX);
	public static event enterRoomAction onEnterRoom;

	public delegate void endRoomAction();
	public static event endRoomAction onEndRoom;

	public delegate void healthChangeAction(float health, bool nextLevel);
	public static event healthChangeAction onHealthChange;

	/*~~~~~~ public variables~~~~~~*/

	public float forwardVelocity;
	public Vector3 jumpForce;

	/*~~~~~~ private variables ~~~~~~*/

	private bool grounded;
	private static float health=100;

	/*~~~~~~ unity functions ~~~~~~*/
	
	void Start () {
		rigidbody2D.velocity = new Vector3 (1, 0, 0) * forwardVelocity;
		onHealthChange(health, true);
	}

	void FixedUpdate () {
		if (Input.GetButton ("Jump") && grounded) {
			rigidbody2D.AddForce (jumpForce, ForceMode2D.Impulse);;
		}
		rigidbody2D.velocity = new Vector3 (forwardVelocity, rigidbody2D.velocity.y, 0);
	}

	void Update () {
		transform.localRotation = new Quaternion (0, 0, 0, 0);
	}

	public void healthUpdate() {
		health = 100;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.CompareTag("floor") || other.gameObject.CompareTag("platform")) {
		   grounded = true;
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		if(other.gameObject.CompareTag("floor") || other.gameObject.CompareTag("platform")) {
			grounded = false;
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag.Contains("nextRoomTrigger")) {
			//Remove nextRoomTrigger part of tag to leave only the room index in the tag string
			string nextRoomIndexString = other.tag.Remove(other.tag.IndexOf("nextRoomTrigger"), "nextRoomTrigger".Length);
			//Parse the string as an integer
			int nextRoomIndexInt = int.Parse(nextRoomIndexString);
			//Enter the next room node at the parsed index
			onEnterRoom(nextRoomIndexInt, other.transform.position.x);
		}
		else if(other.tag.Contains("endRoomTrigger")) {
			onEndRoom();
		}
		else if(other.tag == "spikes") {
			health = health - 10;
			float healthPass=health;
			if (health<=0)
				healthUpdate ();
			onHealthChange(healthPass, false);
		}
	}
	
}
