using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class penguinHealthManager : MonoBehaviour {

	/*~~~~~~ unity functions ~~~~~~*/

	void OnEnable()
	{
		PenguinController.onHealthChange += changeHealth;
	}
	
	
	void OnDisable()
	{
		PenguinController.onHealthChange -= changeHealth;
	}

	/*~~~~~~ private functions ~~~~~~*/

	void changeHealth(float newHealth) {
		Slider healthSlider = this.GetComponent<Slider> ();
		healthSlider.value = newHealth;
	}

}
