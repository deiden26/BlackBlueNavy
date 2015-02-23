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

	/*~~~~~~ private functions ~~~~~~*/

	private void checkForDeath(float newHealth) {
		if (newHealth <= 0)
			startScene();
	}

}
