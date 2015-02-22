using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {


	public void startScene() {
		Application.LoadLevel ("startScene");
	}

	public void runScene() {
		Application.LoadLevel ("runScene");
	}

}
