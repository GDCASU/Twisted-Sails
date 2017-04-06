using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Initial creation. This script takes input and controls
//              the player's icon in the lobby.
//              I implemented this the simplest and most effective way
//              I could imagine, but the "icon" system can be changed 
//              easily if need be

// Developer:   Nizar Kury
// Date:        11/17/2016
// Description: Added CmdChangeShip for ship selection
//              Added RpcShipSelect for ship selection
//              Added a ship icons instance variable for storage of sprites

// Developer:   Kyle Aycock
// Date:        11/17/2016
// Description: Code cleanup
//              Fixed newly connected clients spawning icons off-screen
//              Fixed host seeing "ready" button when not all clients ready

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Adapted to use new team system

// Developer:   Kyle Aycock
// Date:        2/24/2017
// Description: Revamped lobby flow and usage of this script

// Developer:   Kyle Aycock
// Date:        3/23/2017
// Description: Fixed bug having to do with Unity keeping separate connectionIds on client and server

//This is the input/player controller for a player's lobby representation
//Input is given to this script from LobbyManager.cs
public class PlayerIconController : NetworkBehaviour
{
    [SyncVar(hook = "OnNameChange")]
    public string playerName;
    [SyncVar(hook = "OnTeamChange")]
    public short playerTeam;
    [SyncVar(hook = "OnShipChange")]
    public short playerShip;
    [SyncVar]
    public bool host;
    [SyncVar]
    public int connectionId;

    private Text nameText;
    private RectTransform rectTransform;
    private GameObject canvas;

    // Use this for initialization
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        nameText = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();

        if (isLocalPlayer)
        {
            CmdPlayerInit(connectionId);
        }

        OnTeamChange(playerTeam);
        OnNameChange(playerName);
        OnShipChange(playerShip);
    }

    /// <summary>
    /// Automatically called when playerName is changed on this script serverside
    /// </summary>
    /// <param name="newName"></param>
    public void OnNameChange(string newName)
    {
        playerName = newName;
        GetComponent<Text>().text = newName;
    }

    /// <summary>
    /// Automatically called when playerTeam is changed on this script serverside
    /// </summary>
    /// <param name="newTeam"></param>
    public void OnTeamChange(short newTeam)
    {
        playerTeam = newTeam;
        if(isLocalPlayer) MultiplayerManager.GetInstance().localPlayerTeam = newTeam;
        LobbyManager lobby = GameObject.Find("Canvas").GetComponent<LobbyManager>();
        if (lobby.currentState != LobbyManager.LobbyState.TeamSelect) return;
        if (playerTeam == 0)
            transform.SetParent(lobby.redTeam, false);
        else
            transform.SetParent(lobby.blueTeam, false);
    }

    /// <summary>
    /// Automatically called when playerShip is changed on this script serverside
    /// </summary>
    /// <param name="newShip"></param>
    public void OnShipChange(short newShip)
    {
        playerShip = newShip;
        LobbyManager lobby = GameObject.Find("Canvas").GetComponent<LobbyManager>();
        if (lobby.currentState != LobbyManager.LobbyState.ShipSelect) return;
        transform.SetParent(lobby.ShipSelectNameContainers[newShip].transform, false);
    }


    /// <summary>
    /// To be called to hook this object to the player controlling it
    /// </summary>
    /// <param name="connId">Connection ID of the player to hook this object to</param>
    [Command]
    public void CmdPlayerInit(int connId)
    {
        MultiplayerManager.FindPlayer(connId).objectId = GetComponent<NetworkIdentity>().netId;
    }

    /// <summary>
    /// Called by LobbyManager to notify this player of a change in team
    /// </summary>
    /// <param name="team"></param>
    [Command]
    public void CmdChangeTeam(short team)
    {
        if (MultiplayerManager.GetInstance().playerList.FindAll(p => p.team == team).Count >= 4)
            return; //team to switch to already has 4 players, abort
        MultiplayerManager.GetInstance().localPlayerTeam = team;
        MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId).team = team;
        playerTeam = team;
    }

    /// <summary>
    /// Called by LobbyManager to notify this player of a change in ship
    /// </summary>
    /// <param name="ship"></param>
    [Command]
    public void CmdChangeShip(Ship ship)
    {
        MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId).ship = ship;
        playerShip = (short)ship;
    }
    
    /// <summary>
    /// Called by LobbyManager to bounce a Lock In request to the server
    /// </summary>
    [Command]
    public void CmdLockIn()
    {
        MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId).ready = true;
        List<Player> playerList = MultiplayerManager.GetInstance().playerList;
        if (playerList.FindAll(p => p.ready).Count == playerList.Count)
        {
            LobbyManager lobby = canvas.GetComponent<LobbyManager>();
            lobby.LockShips();
            lobby.RpcLockShips();
        }
    }

    //Changes the name displayed on this icon
    public void SetName(string name)
    {
        nameText.text = name;
    }
}
