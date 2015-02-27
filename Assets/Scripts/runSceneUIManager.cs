using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class runSceneUIManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/
	float hurtOverlayAlpha;
	Slider healthSlider;
	Image hurtOverlay;
	Text coinCointText;

	/*~~~~~~ unity functions ~~~~~~*/

	void Start() {
		healthSlider = this.transform.Find ("healthBar").GetComponent<Slider> ();
		hurtOverlay = this.transform.Find ("hurtOverlay").GetComponent<Image> ();
		coinCointText = this.transform.Find ("scoreHolder").transform.Find("score").GetComponent<Text> ();
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
		PenguinController.onCoinCollect += changeCoins;
	}
	
	
	void OnDisable()
	{
		PenguinController.onHealthChange -= changeHealth;
		PenguinController.onCoinCollect -= changeCoins;
	}

	/*~~~~~~ private functions ~~~~~~*/

	void changeHealth(float newHealth, bool nextLevel) {
		if ((newHealth < healthSlider.value) && !nextLevel)
			hurtOverlayAlpha = 1;
		healthSlider.value = newHealth;
	}

	void changeCoins(int coinCount, string coinName) {
		coinCointText.text = string.Concat ("Coins: ", coinCount);
	}

}
