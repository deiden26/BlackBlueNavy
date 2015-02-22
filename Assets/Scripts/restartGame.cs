using UnityEngine;
using System.Collections;

public class restartGame : MonoBehaviour {

	private Vector3 nextPosition;
	private Transform penguinTransform;

	void Start () {
		//Cache reference to penguin transform
		GameObject penguin = GameObject.Find ("penguin");
		penguinTransform = penguin.transform;
	}
	
	void Update () {
//		nextPosition.x = penguinTransform.position.x-2;
//		nextPosition.y = 4.1f;
//		transform.position = nextPosition;
	}
	
	void LoadLevel() {
		Application.LoadLevel ("startScene");
	}
	
	void OnMouseDown () {
		LoadLevel ();
	}
	
}