using UnityEngine;
using System.Collections.Generic;


/*##########################################*/
/* roomNode data type                       */
/*##########################################*/


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

/*##########################################*/
/* roomManager class                        */
/*##########################################*/

public class roomManager : MonoBehaviour {

	/*~~~~~~ events ~~~~~~*/

	public delegate void roomChangeAction(string roomTile);
	public static event roomChangeAction onRoomChange;

	/*~~~~~~ public variables ~~~~~~*/

	public GameObject[] roomObjectPrefabs;
	public int[] poolSize;

	/*~~~~~~ private variables ~~~~~~*/

	private roomNode currentNode;
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
			roomObjectPools.Add(roomObjectPrefab.name, new Queue<GameObject>());
			//And instantiate copies of the room object into the queue
			GameObject roomObject;
			for(int i=0; i<poolSize[poolSizeIndex]; i++) {
				roomObject = (GameObject)Instantiate(roomObjectPrefab, new Vector3(-100,0,0), Quaternion.identity);
				//Remove "(Clone)" from the game object name
				int index = roomObject.name.IndexOf("(Clone)");
				roomObject.name = (index < 0)
					? roomObject.name
					: roomObject.name.Remove(index, "(Clone)".Length);
				roomObjectPools[roomObjectPrefab.name].Enqueue(roomObject);
			}
			poolSizeIndex++;
		}

		//Create temporary hard-coded room nodes
		roomNode room1 = new roomNode();
		roomNode room2 = new roomNode();
		roomNode room3 = new roomNode();

		float fourthCamHeight = Camera.main.orthographicSize / 2;
		float halfCamWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;

		room1.roomTile = "pinkRoom";
		room1.roomSize = 100;
		Vector3 nextRoomTrig0Pos =  new Vector3 (room1.roomSize, fourthCamHeight, 0);
		Vector3 nextRoomTrig1Pos =  new Vector3 (room1.roomSize, -fourthCamHeight, 0);
		Vector3 pipePos0 =  new Vector3 (room1.roomSize + halfCamWidth, fourthCamHeight, 0);
		Vector3 pipePos1 =  new Vector3 (room1.roomSize + halfCamWidth, -fourthCamHeight, 0);
		Vector3 wallPos =  new Vector3 (room1.roomSize + halfCamWidth, 0, 0);
		Vector3 midRoomTrigPos =  new Vector3 (room1.roomSize/2, 0, 0);
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrig0Pos, "nextRoomTriggerHalf"));
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrig1Pos, "nextRoomTriggerHalf"));
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(pipePos0, "pipeHalf"));
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(pipePos1, "pipeHalf"));
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(wallPos, "wall"));
		room1.roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (midRoomTrigPos, "midRoomTrigger"));
		room1.roomAdj.Add (room2);
		room1.roomAdj.Add (room3);

		room2.roomTile = "purpleRoom";
		room2.roomSize = 100;
		nextRoomTrig0Pos =  new Vector3 (room2.roomSize, fourthCamHeight, 0);
		nextRoomTrig1Pos =  new Vector3 (room2.roomSize, -fourthCamHeight, 0);
		pipePos0 =  new Vector3 (room2.roomSize + halfCamWidth, fourthCamHeight, 0);
		pipePos1 =  new Vector3 (room2.roomSize + halfCamWidth, -fourthCamHeight, 0);
		wallPos =  new Vector3 (room2.roomSize + halfCamWidth, 0, 0);
		midRoomTrigPos =  new Vector3 (room2.roomSize/2, 0, 0);
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrig0Pos, "nextRoomTriggerHalf"));
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrig1Pos, "nextRoomTriggerHalf"));
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(pipePos0, "pipeHalf"));
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(pipePos1, "pipeHalf"));
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(wallPos, "wall"));
		room2.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(midRoomTrigPos, "midRoomTrigger"));
		room2.roomAdj.Add (room3);
		room2.roomAdj.Add (room1);

		room3.roomTile = "greenRoom";
		room3.roomSize = 100;
		nextRoomTrig0Pos =  new Vector3 (room3.roomSize, fourthCamHeight, 0);
		nextRoomTrig1Pos =  new Vector3 (room3.roomSize, -fourthCamHeight, 0);
		pipePos0 =  new Vector3 (room3.roomSize + halfCamWidth, fourthCamHeight, 0);
		pipePos1 =  new Vector3 (room3.roomSize + halfCamWidth, -fourthCamHeight, 0);
		wallPos =  new Vector3 (room3.roomSize + halfCamWidth, 0, 0);
		midRoomTrigPos =  new Vector3 (room3.roomSize/2, 0, 0);
		room3.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrig0Pos, "nextRoomTriggerHalf"));
		room3.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(nextRoomTrig1Pos, "nextRoomTriggerHalf"));
		room3.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(pipePos0, "pipeHalf"));
		room3.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(pipePos1, "pipeHalf"));
		room3.roomObjectsInfo.Add (new KeyValuePair<Vector3, string>(wallPos, "wall"));
		room3.roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (midRoomTrigPos, "midRoomTrigger"));
		room3.roomAdj.Add (room1);
		room3.roomAdj.Add (room2);

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

	void OnEnable()
	{
		PenguinController.onEnterRoom += enterRoom;
		PenguinController.onMidRoom += midRoom;
	}
	
	
	void OnDisable()
	{
		PenguinController.onEnterRoom -= enterRoom;
		PenguinController.onMidRoom -= midRoom;
	}

	/*~~~~~~ public functions ~~~~~~*/

	public void enterRoom(int nextRoomIndex, float newRoomPosX) {
		Debug.Log ("Entered next room");

		//Set current node to previous node
		prevNode = currentNode;
		//Set current node to next node
		currentNode = currentNode.roomAdj [nextRoomIndex];
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
		int roomTriggerIndex = 0;
		//For each object position and prefab pair in roomObjects
		foreach (KeyValuePair<Vector3, string> roomObjectPlan in currentNode.roomObjectsInfo) {
			//Get position of room object to be placed
			Vector3 roomObjectPos = new Vector3(roomObjectPlan.Key.x + roomStartPosX, roomObjectPlan.Key.y, roomObjectPlan.Key.z);
			//Remove an instance of the room object from the pool queue
			GameObject roomObject = (roomObjectPools[roomObjectPlan.Value]).Dequeue();
			//Move room object to its new position
			roomObject.transform.position = roomObjectPos;
			//If the room object is a nextRoomTrigger
			if(roomObject.name.Contains("nextRoomTrigger")) {
				//Create a new tag with the next room index (store which room this trigger takes you too)
				roomObject.tag = "nextRoomTrigger" + roomTriggerIndex;
				//Increment next room index in case there is another next room trigger
				roomTriggerIndex++;
			}
			currentNode.placedRoomObjects.Add (roomObject);
		}
	}
	private void removeOldRoomObjects(){
		//For every placed room object
		foreach (GameObject roomObject in prevNode.placedRoomObjects) {
			//Put the object back into the queue to be reused
			roomObjectPools[roomObject.name].Enqueue(roomObject);
		}
		//Clear out the placed room objecs list
		prevNode.placedRoomObjects.Clear ();
	}
}
