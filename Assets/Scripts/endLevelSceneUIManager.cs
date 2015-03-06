using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class endLevelSceneUIManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/


	/*~~~~~~ unity functions ~~~~~~*/

	// Use this for initialization
	void Start () {
		//Get new user score
		int score = PlayerPrefs.GetInt ("score");

		//Get user score text element
		Text scoreText = this.transform.Find ("textHolder").transform.Find ("scoreHolder").transform.Find ("score").GetComponent<Text> ();

		//Get high score text element
		Text highScoresText = this.transform.Find ("textHolder").transform.Find ("highScoresHolder").transform.Find ("highScores").GetComponent<Text> ();

		//Fill user score text element
		string scoreString = string.Format ("Score: {0}", score);
		scoreText.text = scoreString;

		//Get all high scores
		int[] highScores = new int[10];


		bool scoreNotSet = true;
		string highScoresString = "";
		for (int i=0; i < 10; i++) {
			string highScoreLookupString = string.Concat("highScore", i);
			highScores [i] = PlayerPrefs.GetInt (highScoreLookupString);
			if (score > highScores[i] && scoreNotSet) {
				PlayerPrefs.SetInt(highScoreLookupString, score);
				highScores[i] = score;
				scoreNotSet = false;
			}
			highScoresString += string.Format("{0}\n", highScores[i]);
		}

		highScoresText.text = highScoresString;

	}
}
