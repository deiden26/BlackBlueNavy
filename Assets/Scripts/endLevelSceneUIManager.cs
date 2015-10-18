using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class endLevelSceneUIManager : MonoBehaviour {

	/*~~~~~~ private variables ~~~~~~*/


	/*~~~~~~ unity functions ~~~~~~*/

	// Use this for initialization
	void Start () {
		//Get new user score data
		int health = PlayerPrefs.GetInt("health");
		int coinCount = PlayerPrefs.GetInt("coins");
		int time = PlayerPrefs.GetInt("time");
		int didWin = PlayerPrefs.GetInt ("didWin");

		//Chose multiplyer values
		int cointMult, healthMult, timeMult;
		if (didWin == 0) {
			cointMult = 50;
			healthMult = 10;
			timeMult = 10;
		}
		else {
			cointMult = 100;
			healthMult = 10;
			timeMult = 10;
		}

		//Create Score
		int score = (int)( (coinCount * cointMult) + (health * healthMult) - (time * timeMult) );
		
		if (score < 0)
			score = 0;

		//Get user score text element
		Text scoreText = this.transform.Find ("textHolder").transform.Find ("scoreHolder").transform.Find ("score").GetComponent<Text> ();

		//Get high score text element
		Text highScoresText = this.transform.Find ("textHolder").transform.Find ("highScoresHolder").transform.Find ("highScores").GetComponent<Text> ();

		//Get score breakdown text element
		Text breakDownText = this.transform.Find ("textHolder").transform.Find ("breakDownHolder").transform.Find ("breakDown").GetComponent<Text> ();

		//Fill user score text element
		string scoreString = string.Format ("Score: {0}", score);
		scoreText.text = scoreString;

		//Fill in user score breakdown
		string breakDownString = string.Format ("+{0}x{3}\n+{1}x{4}\n-{2}x{5}", coinCount, health, time, cointMult, healthMult, timeMult);
		breakDownText.text = breakDownString;

		//Fill in high score table
		int[] highScores = new int[10];
		bool scoreNotSet = true;
		string highScoresString = "";

		//For each high score slot
		for (int i=0; i < 10; i++) {
			//Create a lookup string
			string highScoreLookupString = string.Concat("highScore", i);
			//Get the old high score
			highScores [i] = PlayerPrefs.GetInt (highScoreLookupString);
			//If the current score is better than the high score...
			if (score > highScores[i] && scoreNotSet) {
				//Replace the high score with the current score in the array
				highScores[i] = score;
				scoreNotSet = false;
				//Replace the high score with the current score in playerprefs and shift everything down one slot
				int insertScore = score;
				int removeScore;
				for (int j=i; j<10; j++) {
					highScoreLookupString = string.Concat("highScore", j);
					//Remove the old value
					removeScore = PlayerPrefs.GetInt (highScoreLookupString);
					//Insert the new values
					PlayerPrefs.SetInt(highScoreLookupString, insertScore);
					//Prepare the old value to become the new value for the next slot down
					insertScore = removeScore;
				}
			}
			highScoresString += string.Format("{0}\n", highScores[i]);
		}

		highScoresText.text = highScoresString;

	}
}
