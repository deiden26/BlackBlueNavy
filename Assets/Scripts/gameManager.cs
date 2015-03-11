using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/
	PenguinController penguinManager;
	runSceneUIManager runSceneUIController;


	/*~~~~~~ unity functions ~~~~~~*/
	
	void OnEnable()
	{
		PenguinController.onHealthChange += checkForDeath;
		roomManager.onRoomChange += checkForExit;
	}
	
	
	void OnDisable()
	{
		PenguinController.onHealthChange -= checkForDeath;
		roomManager.onRoomChange -= checkForExit;
	}

	void Start() {
		penguinManager = (PenguinController)GameObject.Find ("penguin").GetComponent(typeof(PenguinController));
		runSceneUIController = (runSceneUIManager)GameObject.Find ("canvas").GetComponent(typeof(runSceneUIManager));
	}

	void Update() {
		if (Application.loadedLevelName!="runScene")
			if (Input.GetKey ("up") || Input.GetKey ("down") || Input.GetKey ("left") || Input.GetKey ("right") || Input.GetButton ("Jump"))
				runScene ();
	}

	/*~~~~~~ public functions ~~~~~~*/

	public void startScene() {
		Application.LoadLevel ("startScene");
	}

	public void runScene() {
		Application.LoadLevel ("runScene");
	}

	public void loseScene() {
		storeGameValues (false);

		Application.LoadLevel ("loseScene");
	}

	public void endLevelScene() {
		storeGameValues (true);

		Application.LoadLevel ("endLevelScene");
	}

	/*~~~~~~ private functions ~~~~~~*/

	private void checkForDeath(float newHealth, bool nextLevel) {
		if (newHealth <= 0) {
			loseScene ();
		}
	}

	private void checkForExit(string newRoomTile) {
		if (newRoomTile == "endRoom")
			endLevelScene ();
	}

	private void storeGameValues(bool didWin) {
		float health = penguinManager.getHealth();
		int coinCount = penguinManager.getCoinCount();
		float time = runSceneUIController.getTime ();

		int didWinInt = didWin ? 1 : 0;
		
		PlayerPrefs.SetInt("health", (int)health);
		PlayerPrefs.SetInt("coins", coinCount);
		PlayerPrefs.SetInt("time", (int)time);
		PlayerPrefs.SetInt ("didWin", didWinInt);
	}

}
