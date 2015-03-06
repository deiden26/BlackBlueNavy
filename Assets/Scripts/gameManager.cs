using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {


	/*~~~~~~ unity functions ~~~~~~*/
	
	void OnEnable()
	{
		PenguinController.onHealthChange += checkForDeath;
	}
	
	
	void OnDisable()
	{
		PenguinController.onHealthChange -= checkForDeath;
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

	/*~~~~~~ private functions ~~~~~~*/

	private void checkForDeath(float newHealth, bool nextLevel) {
		if (newHealth <= 0) {
			loseScene ();
		}
	}

	void Update() {
		if (Application.loadedLevelName!="runScene")
			if (Input.GetButton ("Jump"))
						runScene ();
	}
}
