using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titleMusic : MonoBehaviour {

// This Script is used  to keep MainTheme Music  active, through scene changes
 
	void Awake () {

		// create an array to store music tagged  TitleMusic
		GameObject[] bgm = GameObject.FindGameObjectsWithTag ("TitleMusic");
		if (bgm.Length > 1)
			//Makes sure no more than 1 instance of the music is active
			Destroy(this.gameObject);

		DontDestroyOnLoad (this.gameObject);
		
	}
	
}