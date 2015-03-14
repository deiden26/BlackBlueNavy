using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PenguinController : MonoBehaviour {

	/*~~~~~~ events ~~~~~~*/
	
	public delegate void enterRoomAction(int nextRoomIndex, float newRoomPosX);
	public static event enterRoomAction onEnterRoom;

	public delegate void endRoomAction();
	public static event endRoomAction onEndRoom;

	public delegate void healthChangeAction(float health, bool nextLevel);
	public static event healthChangeAction onHealthChange;

	public delegate void coinCollectAction(int coinCoint, string coinName);
	public static event coinCollectAction onCoinCollect;

	public delegate void healthCollectAction(string healthName);
	public static event healthCollectAction onHealthCollect;

	/*~~~~~~ public variables~~~~~~*/

	public float forwardVelocity;
	public float forwardVelocityFast;
	public float forwardVelocitySlow;
	public Vector3 jumpForce;
	public Vector3 jumpImpulse;
	public AudioClip coinSound;
	public AudioClip damageSound;
	public AudioClip healthSound;

	/*~~~~~~ private variables ~~~~~~*/

	private bool grounded;
	private bool canJump;
	private bool fastForward = false;
	private float savedForwardVelocity;
	private float health = 100;
	private int coins = 0;
	private int jumpCount=0;
	private float rotationZ=0;

	/*~~~~~~ unity functions ~~~~~~*/
	
	void Start () {
		rigidbody2D.velocity = new Vector3 (1, 0, 0) * forwardVelocity;
		onHealthChange(health, true);
	}

	void FixedUpdate () {
		//Jumping
		if (Input.GetButton ("Jump") && grounded == true) {
			canJump = true;
			rigidbody2D.AddForce (jumpImpulse, ForceMode2D.Impulse);
		}
		if (!Input.GetButton ("Jump") || jumpCount==15) {
			jumpCount=0;
			canJump=false;
		}
		if (canJump) {
			rigidbody2D.AddForce (jumpForce, ForceMode2D.Force);
			jumpCount++;
		}
		//Dropping
		if (Input.GetKey ("down")) {
			rigidbody2D.AddForce (-jumpImpulse / 2, ForceMode2D.Impulse);
			canJump=false;
		}
		//Speed up / slow down
		if (!fastForward && Input.GetKey ("right")) {
			rigidbody2D.velocity = new Vector3 (forwardVelocityFast, rigidbody2D.velocity.y, 0);
		}
		else if (!fastForward && Input.GetKey ("left")) {
			rigidbody2D.velocity = new Vector3 (forwardVelocitySlow, rigidbody2D.velocity.y, 0);
		}
		else {
			rigidbody2D.velocity = new Vector3 (forwardVelocity, rigidbody2D.velocity.y, 0);
		}
	}

	void Update () {
		transform.localRotation = new Quaternion (0, 0, rotationZ, 0);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag ("floor") || other.gameObject.CompareTag ("platformTop")) {
			grounded = true;
		}
		else if (other.gameObject.CompareTag ("platformSide")) {
			rigidbody2D.AddForce (new Vector3(0, .1f, 0), ForceMode2D.Impulse);
		}
		else if (other.gameObject.CompareTag ("platformBottom")) {
			grounded=true;
			rotationZ=180;
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.CompareTag ("floor") || other.gameObject.CompareTag ("platformTop")) {
			grounded = false;
		}
		else if (other.gameObject.CompareTag ("platformBottom")) {
			grounded=false;
			rotationZ=0;
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
			//Increase speed to travel through the pipe faster
			savedForwardVelocity = forwardVelocity;
			fastForward = true;
			forwardVelocity = 25;

		}
		else if(other.tag.Contains("endRoomTrigger")) {
			onEndRoom();
			//Decrease speed to prep user for next room
			forwardVelocity = savedForwardVelocity;
			fastForward = false;
		}
		else if(other.tag == "spikes") {
			//make sound
			audio.PlayOneShot (damageSound);
			health = health - 10;
			if (health<0)
				health = 0;
			onHealthChange(health, false);
		}
		else if(other.tag == "pinkBall") {
			//make sound
			audio.PlayOneShot (healthSound);
			health = health + 10;
			if (health>100)
				health=100;
			onHealthChange(health, false);
			other.transform.position = new Vector3(-100,0,0);
			onHealthCollect(other.name);
		}
		else if(other.tag == "coin") {
			//make sound
			audio.PlayOneShot (coinSound);
			//Remove coin from view
			other.transform.position = new Vector3(-100,0,0);
			//Increment the number of coins you have
			coins++;
			//Get inform other controllers about the coin collected
			onCoinCollect(coins, other.name);
		}
	}

	/*~~~~~~ public functions ~~~~~~*/

	public float getHealth() {
		return health;
	}

	public int getCoinCount() {
		return coins;
	}
	
}
