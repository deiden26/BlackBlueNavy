using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/
	PenguinController penguinManager;
	runSceneUIManager runSceneUIController;
	GameObject instrBox;


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
		instrBox = GameObject.FindWithTag("instrBox");
		instrBox.transform.localScale = new Vector3 (0, 1, 1);
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
		Application.LoadLevel ("loseScene");
	}

	public void hideInstructions () {
		instrBox.transform.localScale = new Vector3 (0, 1, 1);
	}

	public void showInstructions () {
		instrBox.transform.localScale = new Vector3 (1, 1, 1);
	}

	public void endLevelScene() {
		float health = penguinManager.getHealth();
		int coinCount = penguinManager.getCoinCount();
		float time = runSceneUIController.getTime ();

		int score = (int)(coinCount * 100 + health * 10 - time * 10);

		if (score < 0)
			score = 0;

		Debug.Log (score);

		PlayerPrefs.SetInt("score", score);

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

}
