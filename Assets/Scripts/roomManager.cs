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
	public Dictionary<int, KeyValuePair<Vector3, string>> roomObjectsInfo;
	public List<GameObject> placedRoomObjects;
	public int roomObjectsCount;

	/*~~~~~~ public functions ~~~~~~*/
	public roomNode() {
		roomAdj = new List<roomNode> ();
		roomObjectsInfo = new Dictionary<int, KeyValuePair<Vector3, string>> ();
		placedRoomObjects = new List<GameObject> ();
		roomObjectsCount = 0;
	}

	public void addObjectEntry(Vector3 position, string name){
		KeyValuePair<Vector3, string> entry = new KeyValuePair<Vector3, string> (position, name);
		roomObjectsInfo.Add (roomObjectsCount++, entry);
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
	public int maxRoomSize;
	public int minRoomSize;

	/*~~~~~~ private variables ~~~~~~*/

	private roomNode currentNode;
	private roomNode prevNode;
	private roomNode levelEnd;
	private float roomStartPosX;
	private Dictionary<string,Queue<GameObject>> roomObjectPools;

	/*~~~~~~ unity functions ~~~~~~*/

	// Use this for initialization
	void Start () {
		//Initialize levelEnd Node
		levelEnd = new roomNode ();
		levelEnd.roomTile = "pinkRoom";
		levelEnd.roomSize = 100;

		//Create pools of room objects
		createRoomObjectPools ();
		
		//generate room nodes that make up the level
		currentNode = createRooms ();

		//get floor position
		GameObject camera = GameObject.Find ("camera");
		GameObject floor = camera.transform.Find("floor").gameObject;

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
		PenguinController.onEndRoom += endRoom;
	}
	
	
	void OnDisable()
	{
		PenguinController.onEnterRoom -= enterRoom;
		PenguinController.onEndRoom -= endRoom;
	}

	/*~~~~~~ public functions ~~~~~~*/



	/*~~~~~~ private functions ~~~~~~*/

	private void enterRoom(int nextRoomIndex, float pipeStartPosition) {
		//See if end of level
		if (currentNode.roomAdj [nextRoomIndex] == levelEnd) {
			Application.LoadLevel ("endLevelScene");
		}
		//Set current node to previous node
		prevNode = currentNode;
		//Set current node to next node
		currentNode = currentNode.roomAdj [nextRoomIndex];
		//Update roomStartPos
		float pipeLength = roomObjectPools["pipeWhole"].Peek().renderer.bounds.size.x;
		roomStartPosX = pipeStartPosition + pipeLength;
		//Instantiate objects for new room
		placeNewRoomObjects ();
		//Alert subscribers that you have changed the room
		onRoomChange(currentNode.roomTile);
	}
	
	public void endRoom() {
		//Destroy all of the instantiated objects from the old room
		removeOldRoomObjects();
	}

	private void placeNewRoomObjects(){
		int roomTriggerIndex = 0;
		//For each object position and prefab pair in roomObjects
		foreach (var dictEntry in currentNode.roomObjectsInfo) {
			KeyValuePair<Vector3, string> roomObjectPlan = dictEntry.Value;
			//Get position of room object to be placeddictEntry
			Vector3 roomObjectPos = new Vector3(roomObjectPlan.Key.x + roomStartPosX, roomObjectPlan.Key.y, roomObjectPlan.Key.z);
			//Remove an instance of the room object from the pool queue
			GameObject roomObject = (roomObjectPools[roomObjectPlan.Value]).Dequeue();
			//Move room object to its new position
			roomObject.transform.position = roomObjectPos;
			//If the room object is a nextRoomTrigger
			if(roomObject.name.Contains("pipe")) {
				//Create a new tag with the next room index (store which room this trigger takes you too)
				GameObject pipeStart = roomObject.transform.Find("pipeStart").gameObject;
				pipeStart.tag = "nextRoomTrigger" + roomTriggerIndex;
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

	private void createRoomObjectPools() {
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
	}

	private roomNode createRooms() {

		//Create 4 to 9 room nodes of random sizes and colors (with no 2 colors being the same)
		roomNode[] roomNodes = generateRoomNodes();

		//Choose a start node and an end node
		int currentNodeIndex = Random.Range (0, roomNodes.Length);
		int endNodeIndex = Random.Range (0, roomNodes.Length-1);
		if (endNodeIndex == currentNodeIndex)
			endNodeIndex++;

		Debug.Log ("End pipe is bottom one in " + roomNodes[endNodeIndex].roomTile);

		roomNodes [endNodeIndex].roomAdj.Add (levelEnd);

		//Link the room nodes together in a directed graph thats completely traversable from any node
		linkRoomNodes (currentNodeIndex, roomNodes);

		//Add triggers to get from one room to the next
		addNextRoomTriggers (roomNodes);

		//add objects to rooms
		addObjects (roomNodes);

		return roomNodes [currentNodeIndex];
	}

	private void addObjects(roomNode[] roomNodes) {
		//For each room
		for (int i=0; i<roomNodes.Length; i++) {
			//Add a random number of spikes that is related to room size
			int numObjects = (int) roomNodes[i].roomSize/20;
			numObjects = (int) Random.Range ((int)numObjects/2f, (int)numObjects*1.5f);
			float minPosition=10;
			float maxPosition=roomNodes[i].roomSize-10;
			for (int j=0; j<numObjects; j++) {
				float xposition=Random.Range (minPosition, maxPosition);
				roomNodes [i].addObjectEntry(new Vector3(xposition,-4.5f,0), "spikes");
			}
			//Add a random number of spike balls that is related to room size
			numObjects = (int) roomNodes[i].roomSize/20;
			numObjects = (int) Random.Range ((int)numObjects/2f, (int)numObjects*1.5f);
			minPosition=10;
			maxPosition=roomNodes[i].roomSize-10;
			for (int j=0; j<numObjects; j++) {
				float xposition=Random.Range (minPosition, maxPosition);
				float yposition=Random.Range (-3.5f, 3.5f);
				roomNodes [i].addObjectEntry(new Vector3(xposition,yposition,0), "spikeBall");
			}
			//Add a random number of platforms that is related to room size
			numObjects = (int) roomNodes[i].roomSize/20;
			numObjects = (int) Random.Range ((int)numObjects/2f, (int)numObjects*1.5f);
			minPosition=10;
			maxPosition=roomNodes[i].roomSize-10;
			roomNodes [i].addObjectEntry(new Vector3(maxPosition+5,-1f,0), "platform");
			for (int j=0; j<(numObjects-1); j++) {
				float xposition=Random.Range (minPosition, maxPosition);
				float yposition=Random.Range (-3.5f, 0f);
				roomNodes [i].addObjectEntry(new Vector3(xposition,yposition,0), "platform");
			}
			//Add health balls
			int healthProb = Random.Range (0,2);
			if (healthProb==1) {
				float xposition=Random.Range (minPosition, maxPosition);
				float yposition=Random.Range (-3.5f, 0f);
				roomNodes [i].addObjectEntry(new Vector3(xposition,yposition,0), "pinkBall");
			}
			//Add a random number of coins that is related to room size
			numObjects = (int) roomNodes[i].roomSize/5;
			numObjects = (int) Random.Range ((int)numObjects/2f, (int)numObjects*1.5f);
			minPosition=10;
			maxPosition=roomNodes[i].roomSize-10;
			for (int j=0; j<(numObjects-1); j++) {
				float xposition=Random.Range (minPosition, maxPosition);
				float yposition=Random.Range (-3.5f, 3.5f);
				roomNodes [i].addObjectEntry(new Vector3(xposition,yposition,0), "coin");
			}
		}
	}
	
	private roomNode[] generateRoomNodes () {
		//Create 4 to 8 room nodes
		int numRooms = Random.Range (4, 9);
		roomNode[] roomNodes = new roomNode[numRooms];
		//Create temporary taken array
		for (int i=0; i<numRooms; i++) {
			//Create the room node object
			roomNodes[i]=new roomNode();
			//Assign each room node a different color
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
			case 7: roomNodes[i].roomTile="yellowRoom";
				break;
			}
			// Assign each room a size
			roomNodes[i].roomSize=Random.Range (minRoomSize, maxRoomSize);
		}

		return roomNodes;
	}

	private void linkRoomNodes(int startNodeIndex, roomNode[] roomNodes) {
		int numRooms = roomNodes.Length;

		//Create a random path starting from start and touching all nodes exactly once
		int index = startNodeIndex;
		bool[] visited = new bool[numRooms];
		int numVisited = 0;
		while (numVisited < numRooms - 1) {
			//Mark current room as visited
			visited[index] = true;
			numVisited++;
			//Pick a random room
			int nextRoomIndex = Random.Range(0,numRooms);
			//If the room has already been visited
			while(visited[nextRoomIndex])
				//Try the next room
				nextRoomIndex = (nextRoomIndex + 1) % numRooms;
			//Add the unvisted room to the current room adjacentcy list
			roomNodes[index].roomAdj.Add(roomNodes[nextRoomIndex]);
			//Set the next room index as the index
			index = nextRoomIndex;
		}
		
		//Reset visited array to false
		for (int i = 0; i < visited.Length; i++)
			visited[i] = false;
		
		//Create a random path from the last node in the current path to the beginning touching each node at most once
		while (index != startNodeIndex) {
			//Mark current room as visited
			visited[index] = true;
			//Pick a random room
			int nextRoomIndex = Random.Range(0,numRooms);
			//If the room has already been visited
			while(visited[nextRoomIndex])
				//Try the next room
				nextRoomIndex = (nextRoomIndex + 1) % numRooms;
			//Add the unvisted room to the current room adjacentcy list (if not already present)
			if(!roomNodes[index].roomAdj.Contains(roomNodes[nextRoomIndex]))
				roomNodes[index].roomAdj.Add(roomNodes[nextRoomIndex]);
			//Set the next room index as the index
			index = nextRoomIndex;
		}
		
		//At this point we have a graph with a random loop that touches every node at least once
		
		//Add more connects (max 3 per room)
		for (index = 0; index < numRooms; index++) {
			//Get number of new rooms to connect to
			int numConnectedRoom = roomNodes[index].roomAdj.Count; //Will be 1 or 2
			int maxNewConnections = 4 - numConnectedRoom ;
			int numNewAdjRooms = Random.Range(0, maxNewConnections);
			
			//Connect to at most the number of new adjacent rooms
			for(int i=0; i<numNewAdjRooms; i++) {
				//Get the index of a random room
				int nextRoomIndex = Random.Range(0,numRooms);
				//If the index is for the current room or for a room already in the roomAdj array
				if (nextRoomIndex == index || roomNodes[index].roomAdj.Contains(roomNodes[nextRoomIndex]))
					//Try the next room
					nextRoomIndex = (nextRoomIndex + 1) % numRooms;
				//Add the room to the current room adjacentcy list
				roomNodes[index].roomAdj.Add(roomNodes[nextRoomIndex]);
			}
		}
	}

	private void addNextRoomTriggers(roomNode[] roomNodes) {
		int numRooms = roomNodes.Length;
		float pipeLength = roomObjectPools["pipeWhole"].Peek().renderer.bounds.size.x;
		for (int i=0; i<numRooms; i++) {
			float posX = roomNodes [i].roomSize + pipeLength/2f;
			//Add wall
			Vector3 wallPos = new Vector3 (posX, 0, 0);
			roomNodes [i].addObjectEntry(wallPos, "wall");
			//add pipes
			if (roomNodes[i].roomAdj.Count==1) {
				Vector3 nextRoomTrig0Pos = new Vector3 (posX, 0, 0);
				roomNodes [i].addObjectEntry(nextRoomTrig0Pos, "pipeWhole");
			}
			else if (roomNodes[i].roomAdj.Count==2) {
				Vector3 nextRoomTrig0Pos = new Vector3 (posX, -Camera.main.orthographicSize / 2, 0);
				Vector3 nextRoomTrig1Pos = new Vector3 (posX, Camera.main.orthographicSize / 2, 0);
				roomNodes [i].addObjectEntry(nextRoomTrig0Pos, "pipeHalf");
				roomNodes [i].addObjectEntry(nextRoomTrig1Pos, "pipeHalf");
			}
			else if (roomNodes[i].roomAdj.Count==3) {
				Vector3 nextRoomTrig0Pos = new Vector3 (posX, -Camera.main.orthographicSize * 2 / 3, 0);
				Vector3 nextRoomTrig1Pos = new Vector3 (posX, Camera.main.orthographicSize * 2 / 3, 0);
				Vector3 nextRoomTrig2Pos = new Vector3 (posX, 0, 0);
				roomNodes [i].addObjectEntry(nextRoomTrig0Pos, "pipeThird");
				roomNodes [i].addObjectEntry(nextRoomTrig1Pos, "pipeThird");
				roomNodes [i].addObjectEntry(nextRoomTrig2Pos, "pipeThird");
			}
		}
	}
}
