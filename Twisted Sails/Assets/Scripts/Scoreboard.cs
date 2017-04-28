using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour {

    public GameObject scoreboardUI;
    public GameObject[] scoreboardSlots;

    private bool isDisplaying;
    private MultiplayerManager manager;

    private void Start()
    {
        manager = MultiplayerManager.GetInstance();
        //manager.client.RegisterHandler(MultiplayerManager.ExtMsgType.State, UpdateScoreboard);
    }

	// Update is called once per frame
	void Update () {
		if(InputWrapper.GetKey(KeyCode.Tab) && !isDisplaying)
        {
            isDisplaying = true;
            scoreboardUI.SetActive(true);
            UpdateScoreboard(null);
        }
        else if (!InputWrapper.GetKey(KeyCode.Tab) && isDisplaying)
        {
            isDisplaying = false;
            scoreboardUI.SetActive(false);
        }
	}

    public void UpdateScoreboard(NetworkMessage netMsg)
    {
        List<Player> playerList = manager.playerList;
        int[] teamScores = manager.teamScores;
        int team = -1;
        List<Player> teamPlayers = null;
        int playerIndex = 0;
        for (int i=0; i<scoreboardSlots.Length; i++)
        {
            Transform parent = scoreboardSlots[i].transform;
            if (i % 5 == 0)
            {
                team++;
                parent.Find("Name").GetComponent<Text>().text = manager.currentGamemode.teams[team].teamName;
                parent.Find("Stats").GetComponent<Text>().text = "Kills/Deaths/Bounty";
                parent.Find("Score").GetComponent<Text>().text = teamScores[team].ToString();
                playerIndex = 0;
                teamPlayers = playerList.FindAll(p => p.team == team);
            } else
            {
                if (playerIndex < teamPlayers.Count)
                {
                    Player player = teamPlayers[playerIndex];
                    parent.Find("Name").GetComponent<Text>().text = player.name;
                    parent.Find("Stats").GetComponent<Text>().text = player.kills + " / " + player.deaths + " / " + player.GetBounty();
                    parent.Find("Score").GetComponent<Text>().text = player.score.ToString();
                    playerIndex++;
                }
                else
                {
                    parent.Find("Name").GetComponent<Text>().text = "";
                    parent.Find("Stats").GetComponent<Text>().text = "";
                    parent.Find("Score").GetComponent<Text>().text = "";
                }
            }
        }
    }
}
