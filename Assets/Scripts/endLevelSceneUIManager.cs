using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class endLevelSceneUIManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/


	/*~~~~~~ unity functions ~~~~~~*/

	// Use this for initialization
	void Start () {
		int score = PlayerPrefs.GetInt ("score");
		Text scoreText = this.transform.Find ("textHolder").transform.Find ("scoreHolder").transform.Find ("score").GetComponent<Text> ();

		string scoreString = string.Format ("Score: {0}", score);
		scoreText.text = scoreString;
	}
}
