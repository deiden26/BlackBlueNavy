using UnityEngine;
using System.Collections.Generic;


/*##########################################*/
/* roomNode data type                       */
/*##########################################*/


public class roomNode {

	/*~~~~~~ public variables~~~~~~*/
	public string roomTile;
	public float roomSize;
	public bool taken;
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
	private roomNode endNode;
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

		//generate room nodes
		int numRooms = Random.Range (4, 9);
		roomNode[] roomNodes = new roomNode[numRooms];
		for (int i=0; i<numRooms; i++) {
			//temperary roomTile assignment
			roomNodes[i]=new roomNode();
			switch (i) {
			case 0: roomNodes[i].roomTile="pinkRoom";
				break;
			case 1: roomNodes[i].roomTile="purpleRoom";
				break;
			case 2: roomNodes[i].roomTile="greenRoom";
				break;
			case 3: roomNodes[i].roomTile="blueRoom";
				break;
			case 4: roomNodes[i].roomTile="redRoom";
				break;
			case 5: roomNodes[i].roomTile="blackRoom";
				break;
			case 6: roomNodes[i].roomTile="orangeRoom";
				break;
			case 7: roomNodes[i].roomTile="redRoom";
				break;
			}
			roomNodes[i].roomSize=Random.Range (100, 400);
			roomNodes[i].taken = false;
		}

		currentNode=roomNodes[Random.Range (0, numRooms)];
		endNode = roomNodes [Random.Range (0, numRooms)];
		currentNode.taken = true;
		roomNode tempNode = currentNode;

		while (tempNode!=endNode) {
			int tempNum=Random.Range (0, numRooms);
			if (!roomNodes[tempNum].taken) {
				tempNode.roomAdj.Add (roomNodes[tempNum]);
				roomNodes[tempNum].roomAdj.Add (tempNode);
				roomNodes[tempNum].taken=true;
				tempNode=roomNodes[tempNum];
			}
		}

		//connect more nodes
		//for (int i=0; i<

		//add nextRoomTriggers
		for (int i=0; i<numRooms; i++) {
			Vector3 midRoomTrigPos = new Vector3 (roomNodes [i].roomSize / 2, 0, 0);
			roomNodes [i].roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (midRoomTrigPos, "midRoomTrigger"));
			if (roomNodes[i].roomAdj.Count==1) {
				Vector3 nextRoomTrig0Pos = new Vector3 (roomNodes [i].roomSize, 0, 0);
				roomNodes [i].roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (nextRoomTrig0Pos, "nextRoomTriggerWhole"));
			}
			else if (roomNodes[i].roomAdj.Count==2) {
				Vector3 nextRoomTrig0Pos = new Vector3 (roomNodes [i].roomSize, -Camera.main.orthographicSize / 2, 0);
				Vector3 nextRoomTrig1Pos = new Vector3 (roomNodes [i].roomSize, Camera.main.orthographicSize / 2, 0);
				roomNodes [i].roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (nextRoomTrig0Pos, "nextRoomTriggerHalf"));
				roomNodes [i].roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (nextRoomTrig1Pos, "nextRoomTriggerHalf"));
			}
			else if (roomNodes[i].roomAdj.Count==3) {
				Vector3 nextRoomTrig0Pos = new Vector3 (roomNodes [i].roomSize, -Camera.main.orthographicSize / 3, 0);
				Vector3 nextRoomTrig1Pos = new Vector3 (roomNodes [i].roomSize, Camera.main.orthographicSize / 3, 0);
				Vector3 nextRoomTrig2Pos = new Vector3 (roomNodes [i].roomSize, 0, 0);
				roomNodes [i].roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (nextRoomTrig0Pos, "nextRoomTriggerThird"));
				roomNodes [i].roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (nextRoomTrig1Pos, "nextRoomTriggerThird"));
				roomNodes [i].roomObjectsInfo.Add (new KeyValuePair<Vector3, string> (nextRoomTrig2Pos, "nextRoomTriggerThird"));
			}
		}

		//Initialize variables to start rendering/instantiating rooms
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
	}
}
