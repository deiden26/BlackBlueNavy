using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public PenguinController penguin;

	private Vector3 nextPosition;
	private Transform penguinTransform;

	// Use this for initialization
	void Start () {
		//Cache reference to penguin transform
		GameObject penguin = GameObject.Find ("penguin");
		penguinTransform = penguin.transform;
		//Initialize next position
		nextPosition = camera.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//Track penguins motion in the x direction
		nextPosition.x = penguinTransform.position.x;
		camera.transform.position = nextPosition;
	}
}
