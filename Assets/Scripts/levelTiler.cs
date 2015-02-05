using UnityEngine;
using System.Collections.Generic;

public class levelTiler : MonoBehaviour {

	public GameObject[] availableBackgrounds;

	private Dictionary<string,Queue<GameObject>> roomPools;
	private string roomName; //temporary
	private float screenWidthInPoints;
	private Vector3 nextPosition;
	private Transform penguinTransform;

	// Use this for initialization
	void Start () {
		//Initialize next position
		nextPosition = new Vector3 (0, 0, 0);
		//Cache reference to penguin transform
		GameObject penguin = GameObject.Find ("penguin");
		penguinTransform = penguin.transform;
		//Get width of screen
		float height = 2.0f * Camera.main.orthographicSize;
		screenWidthInPoints = height * Camera.main.aspect;
		//Initialize the dictionary of room pools
		roomPools = new Dictionary<string, Queue<GameObject>>();
		//For each type of room...
		foreach (GameObject room in availableBackgrounds) {
			//Create a queue in the dictionary
			roomPools.Add(room.name, new Queue<GameObject>());
			roomName = room.name;
			//And instantiate 2 copies of the room type into the queue
			for(int i=0; i<2; i++)
				roomPools[room.name].Enqueue((GameObject)Instantiate(room, new Vector3(-100,0,0), Quaternion.identity));
		}
		//Load the first room
		recycleBackgrounds (roomName);
	}
	
	// Update is called once per frame
	void Update () {
		//If the penguin is halfway through a room...
		if (penguinTransform.position.x + screenWidthInPoints >= nextPosition.x)
			//Take the room that just went out of view and put it infront of the penguin
			recycleBackgrounds (roomName);
	}


	private void recycleBackgrounds(string roomType) {
		//Get next room from pool
		GameObject nextRoom = (roomPools[roomType]).Dequeue();
		//Set position of next room to next position
		nextRoom.transform.position = nextPosition;
		//Update next position to next room's position + next room's width
		nextPosition.x = nextRoom.transform.position.x + nextRoom.renderer.bounds.size.x;
		//Add back to the queue
		roomPools [roomType].Enqueue (nextRoom);
	}
}
