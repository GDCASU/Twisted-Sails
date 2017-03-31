#if ENABLE_UNET

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Developer: Kyle Aycock
// Date: 11/3/2016
// This is a heavily modified version of the original 'NetworkManagerHUD'. The code for
// NetworkManagerHUD is open source at: https://bitbucket.org/Unity-Technologies/networking/
//
// Added new GUI controls to input name and preferred team
// Added scoreboard & recent score gain feed.

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Adapted to new team system

// Developer:   Kyle Aycock
// Date:        3/23/2017
// Description: Allow TAB playerlist only in game
//              Disabled unnecessary debug function
//              Hide redundant info display in top left

// Developer:   Kyle Aycock
// Date:        3/24/2017
// Description: Removed some unnecessary code

[RequireComponent(typeof(NetworkManager))]
public class NetworkHUD : MonoBehaviour
{
    public bool showGUI = true;
    public int offsetX;
    public int offsetY;
    public float messageDisplayDuration;

    bool showScoreboard;
    List<string> messageStack;
    float messageTimer;

    private MultiplayerManager manager;
    private GUIStyle scoreboardStyle;
    private GUIStyle scoreFeedStyle;
    private Color defaultColor;

    private bool showDebug;

    void Awake()
    {
        manager = GetComponent<MultiplayerManager>();
        showScoreboard = false;
        showDebug = false;
        messageStack = new List<string>();
    }

    void Update()
    {
        if (!showGUI)
            return;
        if (NetworkServer.active || manager.IsClientConnected())
        {
            if (InputWrapper.GetKeyDown(KeyCode.X))
            {
                if (NetworkServer.active)
                    manager.StopHost();
                else
                    manager.StopClient();
            }
            if (InputWrapper.GetKey(KeyCode.Tab) && !MultiplayerManager.IsLobby())
                showScoreboard = true;
            else
                showScoreboard = false;
        }
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0)
            {
                messageStack.RemoveAt(0);
                if (messageStack.Count > 0)
                    messageTimer = messageDisplayDuration;
                else
                    messageTimer = 0;
            }
        }
        /*if (InputWrapper.GetKey(KeyCode.P))
        {
            showDebug = true;
        }*/
    }

    public void OnKill(NetworkMessage netMsg)
    {
        messageStack.Add("Kill! +1");
        MultiplayerManager.KillMessage msg = netMsg.ReadMessage<MultiplayerManager.KillMessage>();
        if (msg.bountyGained > 0)
            messageStack.Add("Bounty! +" + msg.bountyGained);
        if (messageTimer == 0)
            messageTimer = messageDisplayDuration;
    }

    void OnGUI()
    {
        if (!showGUI)
            return;

        int xpos = 10 + offsetX;
        int ypos = 40 + offsetY;
        int spacing = 24;

        //This disabled code is what makes the standard buttons for starting/joining a server appear
        //Uncomment this block if that is desired
        /*if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
        {
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Host"))
            {
                manager.StartHost();
            }
            ypos += spacing;

            if (GUI.Button(new Rect(xpos, ypos, 95, 20), "LAN Client"))
            {
                manager.StartClient();
            }
            manager.networkAddress = GUI.TextField(new Rect(xpos + 105, ypos, 95, 20), manager.networkAddress);
            ypos += spacing;

            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Server Only"))
            {
                manager.StartServer();
            }
            ypos += spacing;
        }
        else
        {*/


        //Uncomment this code to display the server address and port in the top left
        /*if (NetworkServer.active)
        {
            GUI.Label(new Rect(xpos, ypos, 300, 20), "Server: port=" + manager.networkPort);
            ypos += spacing;
        }
        if (NetworkClient.active)
        {
            GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
            ypos += spacing;
        }*/
        //}

        //This disabled code was where the player specified name/team in the HUD
        //Uncomment if that behaviour is desired
        /*if (NetworkClient.active && !ClientScene.ready)
        {
            GUI.Label(new Rect(xpos, ypos, 50, 20), "Name: ");
            manager.localPlayerName = GUI.TextField(new Rect(xpos + 60, ypos, 140, 20), manager.localPlayerName);
            ypos += spacing;

            GUI.Label(new Rect(xpos, ypos, 50, 20), "Team: ");
            manager.localPlayerTeam = (Team) GUI.SelectionGrid(new Rect(xpos + 60, ypos, 140, 20), (int)manager.localPlayerTeam, new string[] { "Red", "Blue" }, 2);
            ypos += spacing;
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Join"))
            {
                manager.SpawnClient();
            }
            ypos += spacing;
        }*/


        //Uncomment this code to show basic player info and stop button in top left
        /*if (NetworkServer.active || manager.IsClientConnected())
        {
            if (ClientScene.ready)
            {
                GUI.Label(new Rect(xpos, ypos, 300, 20), "Name: " + manager.localPlayerName);
                ypos += spacing;
                GUI.Label(new Rect(xpos, ypos, 300, 20), "Team: " + manager.localPlayerTeam.ToString());
                ypos += spacing;
            }
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Stop (X)"))
            {
                manager.StopHost();
            }
            ypos += spacing;
        }*/

        if (showDebug)
        {
            GUI.Label(new Rect(xpos, ypos, 300, 20), "Client?: " + MultiplayerManager.IsClient());
            ypos += spacing;
            GUI.Label(new Rect(xpos, ypos, 300, 20), "Host?: " + MultiplayerManager.IsHost());
            ypos += spacing;
            GUI.Label(new Rect(xpos, ypos, 300, 20), "Server?: " + MultiplayerManager.IsServer());
            ypos += spacing;
        }

        //***SCORE FEED***
        GUI.color = Color.black;
        if (messageStack.Count > 0)
        {
            if (scoreFeedStyle == null)
            {
                scoreFeedStyle = new GUIStyle(GUI.skin.label);
                scoreFeedStyle.fontSize = 20;
            }
            ypos = Screen.height / 2;
            xpos = Screen.width / 2 - 30;
            foreach (string msg in messageStack)
            {
                GUI.Label(new Rect(xpos, ypos, 200, 30), msg, scoreFeedStyle);
                ypos += 33;
            }
        }
        GUI.color = Color.white;

        //***SCOREBOARD***
        if (showScoreboard)
        {
            if (scoreboardStyle == null)
            {
                scoreboardStyle = new GUIStyle(GUI.skin.box);
                scoreboardStyle.normal.background = Texture2D.whiteTexture;
            }
            int cellCount = manager.playerList.Count + 2;
            int cellHeight = Mathf.Clamp(Screen.height / cellCount, 1, Mathf.RoundToInt(Screen.height * 0.1f));
            int cellWidth = Mathf.RoundToInt(Screen.width * 0.75f);

            xpos = Screen.width / 2 - cellWidth / 2;
            if (cellCount < 20)
                ypos = Screen.height / 2 - (cellCount * cellHeight) / 2;
            else
                ypos = 0;

            defaultColor = GUI.backgroundColor;
            GUI.contentColor = Color.black;
            for (short i = 0; i < MultiplayerManager.GetCurrentGamemode().NumTeams(); i++)
            {
                Team currentTeam = MultiplayerManager.GetTeam(i);
                Color teamColor = currentTeam.teamColor;
                GUI.backgroundColor = teamColor;
                DrawScoreboardRow(xpos, ypos, cellWidth, cellHeight, "Team " + currentTeam.teamName, "Kills/Deaths/TopBounty", manager.teamScores[i].ToString());
                ypos += cellHeight + 1;
                foreach (Player player in manager.playerList)
                {
                    if (player.team == i)
                    {
                        if (player.connectionId == manager.client.connection.connectionId)
                        {
                            GUI.Box(new Rect(xpos, ypos, cellWidth, cellHeight), GUIContent.none, scoreboardStyle);
                        }
                        DrawScoreboardRow(xpos, ypos, cellWidth, cellHeight, player.name, player.kills + "/" + player.deaths + "/" + player.maxBounty, player.score.ToString());
                        ypos += cellHeight + 1;
                    }
                }
            }
            GUI.backgroundColor = defaultColor;
            GUI.contentColor = Color.white;
        }
    }

    public void DrawScoreboardRow(int xpos, int ypos, int cellWidth, int cellHeight, string name, string stats, string score)
    {
        GUI.Box(new Rect(xpos, ypos, cellWidth, cellHeight), stats, scoreboardStyle);
        GUI.Label(new Rect(xpos + 10, ypos + 5, cellWidth, cellHeight), name);
        GUI.Label(new Rect(xpos + cellWidth - 20, ypos + 5, cellWidth, cellHeight), score);
    }
}
#endif //ENABLE_UNET
