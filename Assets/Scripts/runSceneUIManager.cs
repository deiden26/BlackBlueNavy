using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class runSceneUIManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/
	private float hurtOverlayAlpha;
	private Slider healthSlider;
	private Image hurtOverlay;
	private Text coinCointText;
	private Text timeText;
	private float time;

	/*~~~~~~ unity functions ~~~~~~*/

	void Start() {
		healthSlider = this.transform.Find ("healthBar").GetComponent<Slider> ();
		hurtOverlay = this.transform.Find ("hurtOverlay").GetComponent<Image> ();
		coinCointText = this.transform.Find ("scoreHolder").transform.Find("score").GetComponent<Text> ();
		timeText = this.transform.Find ("timeHolder").transform.Find("time").GetComponent<Text> ();
		hurtOverlayAlpha = 0;
		time = 0;
	}

	void Update() {
		if (hurtOverlayAlpha != 0) {
			Color newColor = hurtOverlay.color;
			newColor.a = hurtOverlayAlpha;
			hurtOverlay.color = newColor;
			hurtOverlayAlpha = hurtOverlayAlpha - 0.02f;
		}
		time += Time.deltaTime;
		int minutes = (int)time / 60;
		int seconds = (int)time % 60;
		int milliseconds = (int)(time * 100) % 100;
		timeText.text = string.Format ("{0}:{1:00}:{2:00}", minutes, seconds, milliseconds);
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

	public float getTime() {
		return time;
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
