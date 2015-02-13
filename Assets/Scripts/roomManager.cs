using UnityEngine;
using System.Collections.Generic;


public class roomNode {

	/*~~~~~~ public variables~~~~~~*/
	public string roomTile;
	public float roomSize;
	public List<roomNode> roomAdj;
	public List<KeyValuePair<Vector3, string>> roomObjectsInfo;
	public List<GameObject> placedRoomObjects;

	/*~~~~~~ public functions ~~~~~~*/
	public roomNode() {
		roomAdj = new List<roomNode> ();
		roomObjectsInfo = new List<KeyValuePair<Vector3, string>> ();
		placedRoomObjects = new List<GameObject> ();
	}
}

public class roomManager : MonoBehaviour {

	public delegate void roomChangeAction(string roomTile);
	public static event roomChangeAction onRoomChange;

	/*~~~~~~ public variables~~~~~~*/

	public roomNode currentNode;

	public GameObject[] roomObjectPrefabs;
	public int[] poolSize;

	/*~~~~~~ private variables ~~~~~~*/

	private roomNode prevNode;
	private float roomStartPosX;
	private Dictionary<string,Queue<GameObject>> roomObjectPools;

	/*~~~~~~ unity functions ~~~~~~*/

	// Use this for initialization
	void Start () {
		//Create pools of room objects
		roomObjectPools = new Dictionary<string,Queue<GameObject>> ();
		int poolSizeIndex = 0;
		foreach (GameObject roomObjectPrefab in roomObjectPrefabs) {
			//Create a queue in the dictionary
			roomObjectPools.Add(roomObjectPrefab.tag, new Queue<GameObject>());
			//And instantiate 20 copies of the room object into the queue
			for(int i=0; i<poolSize[poolSizeIndex]; i++)
				roomObjectPools[roomObjectPrefab.tag].Enqueue((GameObject)Instantiate(roomObjectPrefab, new Vector3(-100,0,0), Quaternion.identity));
			poolSizeIndex++;
		}

		//Create temporary hard-coded room nodes
		roomNode room1 = new roomNode();
		roomNode room2 = new roomNode();

		room1.roomTile = "roomDay";
		room1.roomSize = 100;
		Vector3 nextRoomTrigPos =  new Vector3 (room1.roomSize, 0, 0);
		Vector3 midRoomTrigPos =  new Vector3 (room1.roomSize/2, 0, 0);
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrigPos, "nextRoomTrigger0"));
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (midRoomTrigPos, "midRoomTrigger"));
		room1.roomAdj.Add (room2);

		room2.roomTile = "roomNight";
		room2.roomSize = 100;
		nextRoomTrigPos =  new Vector3 (room1.roomSize, 0, 0);
		midRoomTrigPos =  new Vector3 (room1.roomSize/2, 0, 0);
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrigPos, "nextRoomTrigger0"));
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(midRoomTrigPos, "midRoomTrigger"));
		room2.roomAdj.Add (room1);

		//Initialize variables to start rendering/instantiating rooms
		currentNode = room1;
		roomStartPosX = 0; //Will make first room slightly too large
		//Initialize previous node to an empty node
		prevNode = new roomNode();

		//Place all objects for the first room
		placeNewRoomObjects ();
		//Alert level tiler of roomTile
		onRoomChange(currentNode.roomTile);
	}
	
	// Update is called once per frame
	void Update () {;
	}

	/*~~~~~~ public functions ~~~~~~*/

	public void enterRoom(string tag, float newRoomPosX) {
		//Set current node to previous node
		prevNode = currentNode;
		Debug.Log ("Entered next room");

		if (tag == "nextRoomTrigger0") {
			//Set current node to next node TODO: multiple ways to go
			currentNode = currentNode.roomAdj [0];
		}
		else if (tag == "nextRoomTrigger1") {
			//Set current node to next node TODO: multiple ways to go
			currentNode = currentNode.roomAdj [1];
		}
		else if (tag == "nextRoomTrigger2") {
			//Set current node to next node TODO: multiple ways to go
			currentNode = currentNode.roomAdj [1];
		}

		//Update roomStartPos
		roomStartPosX = newRoomPosX;
		//Instantiate objects for new room
		placeNewRoomObjects ();
		//Alert subscribers that you have changed the room
		onRoomChange(currentNode.roomTile);
	}

	public void midRoom() {
		Debug.Log ("Hit mid room");
		//Destroy all of the instantiated objects from the old room
		removeOldRoomObjects();
	}

	/*~~~~~~ private functions ~~~~~~*/

	private void placeNewRoomObjects(){
		//For each object position and prefab pair in roomObjects
		foreach (KeyValuePair<Vector3, string> roomObjectPlan in currentNode.roomObjectsInfo) {
			//Get position of room object to be placed
			Vector3 roomObjectPos = new Vector3(roomObjectPlan.Key.x + roomStartPosX, roomObjectPlan.Key.y, roomObjectPlan.Key.z);
			//Remove an instance of the room object from the pool queue
			GameObject roomObject = (roomObjectPools[roomObjectPlan.Value]).Dequeue();
			//Move room object to its new position
			roomObject.transform.position = roomObjectPos;
			currentNode.placedRoomObjects.Add (roomObject);
		}
	}
	private void removeOldRoomObjects(){
		//For every placed room object
		foreach (GameObject roomObject in prevNode.placedRoomObjects) {
			//Put the object back into the queue to be reused
			roomObjectPools[roomObject.tag].Enqueue(roomObject);
		}
	}
}
