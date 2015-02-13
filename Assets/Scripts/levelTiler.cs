using UnityEngine;
using System.Collections.Generic;

public class levelTiler : MonoBehaviour {

	/*~~~~~~ public variables~~~~~~*/

	public GameObject[] availableRoomTiles;

	/*~~~~~~ public variables ~~~~~~*/

	private Dictionary<string,Queue<GameObject>> roomTilePools;
	private Vector3 nextTilePosition;
	private Transform cameraTransform;
	private string currentRoomTile;


	/*~~~~~~ unity functions ~~~~~~*/

	// Use this for initialization
	void Start () {
		//Initialize next position
		nextTilePosition = new Vector3 (5, 0, 0);
		//Cache reference to penguin transform
		GameObject camera = GameObject.Find ("camera");
		cameraTransform = camera.transform;
		//Initialize the dictionary of tile pools
		roomTilePools = new Dictionary<string, Queue<GameObject>>();
		//For each type of tile...
		foreach (GameObject tile in availableRoomTiles) {
			//Create a queue in the dictionary
			roomTilePools.Add(tile.name, new Queue<GameObject>());
			//And instantiate 2 copies of the tile type into the queue
			for(int i=0; i<2; i++)
				roomTilePools[tile.name].Enqueue((GameObject)Instantiate(tile, new Vector3(-100,0,0), Quaternion.identity));
		}
		//Load the first room
		recycleBackgrounds (currentRoomTile);
	}
	
	// Update is called once per frame
	void Update () {
		//Get the width of the tile we're currently 
		float spriteWidth = roomTilePools[currentRoomTile].Peek().renderer.bounds.size.x;
		//If the penguin is halfway through a tile...
		if (cameraTransform.position.x + spriteWidth >= nextTilePosition.x)
			//Take the til that just went out of view and put it infront of the penguin
			recycleBackgrounds (currentRoomTile);
	}

	void OnEnable()
	{
		roomManager.onRoomChange += changeRoomTile;
	}
	
	
	void OnDisable()
	{
		roomManager.onRoomChange -= changeRoomTile;
	}

	/*~~~~~~ private functions ~~~~~~*/
	
	private void changeRoomTile(string newRoomTile) {
		currentRoomTile = newRoomTile;
	}

	private void recycleBackgrounds(string roomTileType) {
		//Get next til from pool
		GameObject nextRoomTile = (roomTilePools[roomTileType]).Dequeue();
		//Set position of next tile to next position
		nextRoomTile.transform.position = nextTilePosition;
		//Update next position to next tile's position + next tile's width
		nextTilePosition.x = nextRoomTile.transform.position.x + nextRoomTile.renderer.bounds.size.x;
		//Add back to the queue
		roomTilePools [roomTileType].Enqueue (nextRoomTile);
	}
}
