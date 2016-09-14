using UnityEngine;
using System.Collections;

public class Master : MonoBehaviour {

	public int numbOfTeams = 2;
	public int numbOfLives = 3;
	public static int [] arrayOfLives = new int [2];

	// Use this for initialization
	void Start () {
		for (int i = 0; i < arrayOfLives.Length; i++) {
			arrayOfLives [i] = numbOfLives;
		} 
	}

	// Update is called once per frame
	void Update () {
		if (arrayOfLives[0] <= 0) {
			Debug.Log ("Team 2 wins!");
		}
		if (arrayOfLives[1] <= 0) {
			Debug.Log ("Team 1 wins!");
		}

		Debug.Log ("Team 1 Lives: " + arrayOfLives[0]);
		Debug.Log ("Team 2 Lives: " + arrayOfLives[1]);
	}
}
