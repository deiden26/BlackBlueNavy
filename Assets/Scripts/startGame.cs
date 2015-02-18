using UnityEngine;
using System.Collections;

public class startGame : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	void LoadLevel() {
		Application.LoadLevel ("runScene");
	}

	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown () {
		LoadLevel ();
	}

}