#if ENABLE_UNET

using UnityEngine;
using UnityEngine.Networking;

// Developer: Kyle Aycock
// Date: 11/3/2016
// This is a heavily modified version of the original 'NetworkManagerHUD'. The code for
// NetworkManagerHUD is open source at: https://bitbucket.org/Unity-Technologies/networking/
//
// Added new GUI controls to input name and preferred team

[RequireComponent(typeof(NetworkManager))]
public class NetworkHUD : MonoBehaviour
{
    private MultiplayerManager manager;
    [SerializeField]
    public bool showGUI = true;
    [SerializeField]
    public int offsetX;
    [SerializeField]
    public int offsetY;

    int teamSelection;

    void Awake()
    {
        manager = GetComponent<MultiplayerManager>();
        teamSelection = Random.Range(0, 2);
    }

    void Update()
    {
        if (!showGUI)
            return;
        if (NetworkServer.active || manager.IsClientConnected())
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                manager.StopHost();
            }
        }
    }

    void OnGUI()
    {
        if (!showGUI)
            return;

        int xpos = 10 + offsetX;
        int ypos = 40 + offsetY;
        int spacing = 24;

        if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
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
        {
            if (NetworkServer.active)
            {
                GUI.Label(new Rect(xpos, ypos, 300, 20), "Server: port=" + manager.networkPort);
                ypos += spacing;
            }
            if (NetworkClient.active)
            {
                GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                ypos += spacing;
            }
        }

        //New stuff starts here
        if (NetworkClient.active && !ClientScene.ready)
        {
            GUI.Label(new Rect(xpos, ypos, 50, 20), "Name: ");
            manager.localPlayerName = GUI.TextField(new Rect(xpos + 60, ypos, 140, 20), manager.localPlayerName);
            ypos += spacing;

            GUI.Label(new Rect(xpos, ypos, 50, 20), "Team: ");
            teamSelection = GUI.SelectionGrid(new Rect(xpos + 60, ypos, 140, 20), teamSelection, new string[] { "Red", "Blue" }, 2);
            manager.localPlayerTeam = (Team)teamSelection;
            ypos += spacing;
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Join"))
            {
                ClientScene.Ready(manager.client.connection);

                if (ClientScene.localPlayers.Count == 0)
                {
                    //Instructs the manager to spawn the client with the given name and team
                    manager.SpawnClient();
                }
            }
            ypos += spacing;
        }
        

        if (NetworkServer.active || manager.IsClientConnected())
        {
            if(ClientScene.ready)
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
        }
        //New stuff ends here
    }
}
#endif //ENABLE_UNET
