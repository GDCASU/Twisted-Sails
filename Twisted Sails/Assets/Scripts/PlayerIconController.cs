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

// Developer:   Kyle Aycock
// Date:        4/6/2017
// Description: Fixed players being able to join already-full teams
//              Fixed player names sometimes not showing up when picking a ship

//This is the input/player controller for a player's lobby representation
//Input is given to this script from LobbyManager.cs
public class PlayerIconController : NetworkBehaviour
{
    [SyncVar(hook = "OnNameChange")]
    public string playerName;
    [SyncVar]
    public short playerTeam;
    [SyncVar]
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
        DoChangeShip((Ship)playerShip);
        DoChangeTeam(playerTeam);
        OnNameChange(playerName);
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
        LobbyManager lobby = GameObject.Find("Canvas").GetComponent<LobbyManager>();
        if (lobby.currentState != LobbyManager.LobbyState.TeamSelect || MultiplayerManager.GetInstance().playerList.FindAll(p => p.team == team).Count >= 4)
            return; //team to switch to already has 4 players, abort
        MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId).team = team;
        playerTeam = team;
        RpcChangeTeam(team);
    }

    /// <summary>
    /// Called by server to notify clients of team change
    /// </summary>
    /// <param name="team"></param>
    [ClientRpc]
    public void RpcChangeTeam(short team)
    {
        DoChangeTeam(team);
    }

    public void DoChangeTeam(short team)
    {
        if (isLocalPlayer) MultiplayerManager.GetInstance().localPlayerTeam = team;
        LobbyManager lobby = GameObject.Find("Canvas").GetComponent<LobbyManager>();
        if (team == 0)
            transform.SetParent(lobby.redTeam, false);
        else
            transform.SetParent(lobby.blueTeam, false);
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
        RpcChangeShip(ship);
    }

    /// <summary>
    /// Called by server to notify clients of ship change
    /// </summary>
    /// <param name="ship"></param>
    [ClientRpc]
    public void RpcChangeShip(Ship ship)
    {
        DoChangeShip(ship);
    }

    public void DoChangeShip(Ship ship)
    {
        if(isLocalPlayer) MultiplayerManager.GetInstance().localShipType = ship;
        LobbyManager lobby = GameObject.Find("Canvas").GetComponent<LobbyManager>();
        if (lobby.currentState != LobbyManager.LobbyState.ShipSelect) return;
        transform.SetParent(lobby.ShipSelectNameContainers[(short)ship].transform, false);
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
