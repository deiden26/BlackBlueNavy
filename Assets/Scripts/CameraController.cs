using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	/*~~~~~~ private variables ~~~~~~*/

	private PenguinController penguin;
	private Vector3 nextPosition;
	private Transform penguinTransform;

	/*~~~~~~ unity functions ~~~~~~*/
	
	void Start () {
		//Cache reference to penguin transform
		GameObject penguin = GameObject.Find ("penguin");
		penguinTransform = penguin.transform;
		//Initialize next position
		nextPosition = camera.transform.position;
	}

	void Update () {
		//Track penguins motion in the x direction
		nextPosition.x = penguinTransform.position.x;
		camera.transform.position = nextPosition;
	}
}
