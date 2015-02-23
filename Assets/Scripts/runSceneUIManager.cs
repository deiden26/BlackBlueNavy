using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class runSceneUIManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/
	float hurtOverlayAlpha;
	Slider healthSlider;
	Image hurtOverlay;

	/*~~~~~~ unity functions ~~~~~~*/

	void Start() {
		healthSlider = this.transform.Find ("healthBar").GetComponent<Slider> ();
		hurtOverlay = this.transform.Find ("hurtOverlay").GetComponent<Image> ();
		hurtOverlayAlpha = 0;
	}

	void Update() {
		if (hurtOverlayAlpha != 0) {
			Color newColor = hurtOverlay.color;
			newColor.a = hurtOverlayAlpha;
			hurtOverlay.color = newColor;
			hurtOverlayAlpha = hurtOverlayAlpha - 0.02f;
		}
	}

	void OnEnable()
	{
		PenguinController.onHealthChange += changeHealth;
	}
	
	
	void OnDisable()
	{
		PenguinController.onHealthChange -= changeHealth;
	}

	/*~~~~~~ private functions ~~~~~~*/

	void changeHealth(float newHealth, bool nextLevel) {
		if ((newHealth < healthSlider.value) && !nextLevel)
			hurtOverlayAlpha = 1;
		healthSlider.value = newHealth;
	}

}
