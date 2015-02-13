using UnityEngine;
using System.Collections.Generic;

public class roomNode {
	public string roomColor;
	public float roomSize;
	public List<roomNode> roomAdj;
	public List<KeyValuePair<Vector3, GameObject>> objects;
}

public class roomController : MonoBehaviour {

	public roomNode currentNode;
	public GameObject nextRoomTrigger;

	private float roomStartPos;
	private float roomPos;
	private Transform penguinTransform;

	// Use this for initialization
	void Start () {
		roomNode room1 = new roomNode();
		roomNode room2 = new roomNode();

		room1.roomColor = "hallwayDay";
		room1.roomSize = 200;
		room1.roomAdj.Add (room2);
		Vector3 triggerPosition1 =  new Vector3 (room1.roomSize, Screen.height / 2, 0);
		room1.objects.Add (new KeyValuePair<Vector3, GameObject>(triggerPosition1, nextRoomTrigger));

		room2.roomColor = "hallwaynight";
		room2.roomSize = 200;
		room2.roomAdj.Add (room1);
		Vector3 triggerPosition2 =  new Vector3 (room1.roomSize, Screen.height / 2, 0);
		room1.objects.Add (new KeyValuePair<Vector3, GameObject>(triggerPosition2, nextRoomTrigger));

		GameObject penguin = GameObject.Find ("penguin");
		penguinTransform = penguin.transform;

		currentNode = room1;
		roomPos = 0;
		//Will make first room slightly too large
		roomStartPos = 0;
	}
	
	// Update is called once per frame
	void Update () {
		roomPos = penguinTransform.position.x - roomStartPos;
	
	}

	private void placeNewRoomObjects(){

	}
}
