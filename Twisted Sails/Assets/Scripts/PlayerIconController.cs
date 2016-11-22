using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Initial creation. This script takes input and controls
//              the player's icon in the lobby.

// Developer:   Nizar Kury
// Date:        11/17/2016
// Description: Added CmdChangeShip for ship selection
//              Added RpcShipSelect for ship selection
//              Added a ship icons instance variable for storage of sprites


//This is the input/player controller for a player's lobby icon
public class PlayerIconController : NetworkBehaviour {

    [SyncVar(hook = "OnReadyStateChange")] public bool ready;
    [SyncVar(hook = "OnNameChange")] public string playerName;
    [SyncVar] public bool host;

    private Sprite[] shipIcons; // list of ship icons taken from LobbyManager for RPC use
    private Text nameText;
    private Vector2 startPos;
    private Vector2 targetPos;
    private float progress;
    private RectTransform rectTransform;
    private GameObject canvas;
    public Color defaultColor;
    public Color readyColor;
    

	// Use this for initialization
	void Start () {
        ready = false;
        if (host) MarkHost();
        canvas = GameObject.Find("Canvas");
        transform.SetParent(canvas.transform.Find("TeamSelect"), false);
        nameText = transform.Find("PlayerName").GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();
        targetPos = rectTransform.anchoredPosition;
        
        if(isLocalPlayer)
        {
            defaultColor.a = 255;
            readyColor.a = 255;
            CmdPlayerInit(MultiplayerManager.instance.client.connection.connectionId);  
        }
        OnReadyStateChange(ready);
        OnNameChange(playerName);
    }
	
	// Update is called once per frame
	void Update () {
	    if(rectTransform.anchoredPosition != targetPos)
        {
            progress += Time.deltaTime*2;
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, progress);
            rectTransform.anchoredPosition = newPos;
        }
	}

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void MarkHost()
    {
        defaultColor.b = 0;
        readyColor.r = 255;
        OnReadyStateChange(ready);
    }

    #region Commands
    /// <summary>
    /// To be called to hook this object to the player controlling it
    /// </summary>
    /// <param name="connId">Connection ID of the player to hook this object to</param>
    [Command]
    public void CmdPlayerInit(int connId)
    {
        MultiplayerManager.instance.RegisterPlayer(connId, GetComponent<NetworkIdentity>().netId);
    }

    /// <summary>
    /// Called by the client to send a request to switch teams
    /// </summary>
    /// <param name="team">Team to switch to</param>
    /// <param name="parentName">Name of the container for the team</param>
    /// <param name="localTarget">Position within the container to move to</param>
    [Command]
    public void CmdMoveReq(Team team, string parentName, Vector2 localTarget)
    {
        MultiplayerManager.instance.ChangePlayerTeam(GetComponent<NetworkIdentity>().netId, team);
        RpcDoMove(team, parentName, localTarget);
    }

    [Command]
    public void CmdChangeShip(Ship ship, Sprite[] shipPics)
    {
        MultiplayerManager.instance.ChangePlayerShip(GetComponent<NetworkIdentity>().netId, ship);
        shipIcons = shipPics;
        RpcShipSelect(ship);
    }

    /// <summary>
    /// Toggles readiness for match to start
    /// </summary>
    [Command]
    public void CmdReady()
    {
        MultiplayerManager.instance.SetPlayerReady(GetComponent<NetworkIdentity>().netId, !ready);
        ready = !ready;
    }
    #endregion

    #region ClientRpc
    /// <summary>
    /// Called by server to mark this icon as belonging to the host player
    /// </summary>
    [ClientRpc]
    public void RpcMarkHost()
    {
        host = true;
        MarkHost();
    }

    /// <summary>
    /// Called by server to perform a team switch on this icon
    /// </summary>
    /// <param name="team">Team to switch to</param>
    /// <param name="parentName">Name of the container for the team</param>
    /// <param name="localTarget">Position within the container to move to</param>
    [ClientRpc]
    public void RpcDoMove(Team team, string parentName, Vector2 localTarget)
    {
        transform.SetParent(canvas.transform.Find("TeamSelect").Find(parentName));
        startPos = rectTransform.anchoredPosition;
        targetPos = localTarget;
        progress = 0;
    }

    // NK
    /// <summary>
    /// Called by server to perform a ship switch for the player
    /// </summary>
    /// <param name="ship">Ship to switch to</param>
    [ClientRpc]
    public void RpcShipSelect(Ship ship)
    {
        transform.Find("ShipIcon").GetComponent<Image>().sprite = shipIcons[(int)ship];     
    }
    #endregion

    #region Hooks
    /// <summary>
    /// Automatically called when this icon is readied or unreadied
    /// </summary>
    /// <param name="newState"></param>
    public void OnReadyStateChange(bool newState)
    {
        ready = newState;
        GetComponent<Image>().color = (ready ? readyColor : defaultColor);
    }

    /// <summary>
    /// Automatically called when playerName is changed on this icon serverside
    /// </summary>
    /// <param name="newName"></param>
    public void OnNameChange(string newName)
    {
        playerName = newName;
        transform.Find("PlayerName").GetComponent<Text>().text = newName;
    }
    #endregion
}
