using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Created by Ryan Black - 11/30/2016
//Just updates the scoreboard at the top of the screen, this could probably be put in another script to consolidate everything but it's finals week and my brain is tired :(

public class ScoreManager : MonoBehaviour {

    public Text redScoreText;
    public Text blueScoreText;

	void Update () {
	    redScoreText.text = MultiplayerManager.GetTeamScore(Team.Red).ToString();
        blueScoreText.text = MultiplayerManager.GetTeamScore(Team.Blue).ToString();
    }
}