using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public PenguinController penguin;

	// Use this for initialization
	void Start () {
		rigidbody2D.velocity = new Vector3 (1, 0, 0) * penguin.forwardVelocity;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
