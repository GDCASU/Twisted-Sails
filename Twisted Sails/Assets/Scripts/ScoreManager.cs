using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Created by Ryan Black - 11/30/2016
//Just updates the scoreboard at the top of the screen, this could probably be put in another script to consolidate everything but it's finals week and my brain is tired :(

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Adapted to use new MultiplayerManager interfact w/new Team system

public class ScoreManager : MonoBehaviour
{
    public Text redScoreText;
    public Text blueScoreText;

    //This should be adapted to allow different numbers/colors of teams ~ Kyle A
    void Update()
    {
        redScoreText.text = MultiplayerManager.GetTeamScore(0).ToString();
        blueScoreText.text = MultiplayerManager.GetTeamScore(1).ToString();
    }
}