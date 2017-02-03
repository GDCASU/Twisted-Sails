using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

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

//This is the input/player controller for a player's lobby icon
//Input is given to this script from LobbyManager.cs currently
public class PlayerIconController : NetworkBehaviour
{
    [SyncVar(hook = "OnReadyStateChange")]
    public bool ready;
    [SyncVar(hook = "OnNameChange")]
    public string playerName;
    [SyncVar]
    public bool host;
    [SyncVar]
    public Vector2 effectivePosition;
    public Color defaultColor;
    public Color readyColor;

    private Sprite[] shipIcons; // list of ship icons taken from LobbyManager for RPC use
    private Text nameText;
    private Vector2 startPos;
    private Vector2 targetPos;
    private float progress;
    private RectTransform rectTransform;
    private GameObject canvas;

    #region Hooks
    // Use this for initialization
    void Start()
    {
        ready = false;
        canvas = GameObject.Find("Canvas");
        transform.SetParent(canvas.transform.Find("TeamSelect"), false);
        nameText = transform.Find("PlayerName").GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = effectivePosition;
        targetPos = effectivePosition;

        shipIcons = canvas.GetComponent<LobbyManager>().shipIcons;

        if (host)
            MarkHost();
        else if (MultiplayerManager.IsHost() && isLocalPlayer) //temporary solution
            CmdMarkHost();

        if (isLocalPlayer)
        {
            defaultColor.a = 255;
            readyColor.a = 255;
            CmdPlayerInit(MultiplayerManager.GetLocalClient().connection.connectionId);
            CmdReady(ready);
        }
        OnReadyStateChange(ready);
        OnNameChange(playerName);
    }

    // Update is called once per frame
    void Update()
    {
        //Smooth movement
        if (rectTransform.anchoredPosition != targetPos)
        {
            progress += Time.deltaTime * 2;
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, progress);
            rectTransform.anchoredPosition = newPos;
            effectivePosition = newPos;
        }
    }

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

    #region Commands
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
    /// Called by the client to send a request to switch teams
    /// </summary>
    /// <param name="team">Team to switch to</param>
    /// <param name="parentName">Name of the container for the team</param>
    /// <param name="localTarget">Position within the container to move to</param>
    [Command]
    public void CmdMoveReq(short team, string parentName, Vector2 localTarget)
    {
        MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId).team = team;
        RpcDoMove(team, parentName, localTarget);
    }

    [Command]
    public void CmdChangeShip(Ship ship)
    {
        MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId).ship = ship;
        RpcShipSelect(ship);
    }

    [Command]
    public void CmdMarkHost()
    {
        host = true;
        RpcMarkHost();
    }

    /// <summary>
    /// Toggles readiness for match to start
    /// </summary>
    [Command]
    public void CmdReady(bool ready)
    {
        MultiplayerManager.GetInstance().SetPlayerReady(GetComponent<NetworkIdentity>().netId, ready);
        this.ready = ready;
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
    public void RpcDoMove(short team, string parentName, Vector2 localTarget)
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

    #region Misc
    //Changes the name displayed on this icon
    public void SetName(string name)
    {
        nameText.text = name;
    }
    #endregion

    //This icon is the host's icon
    public void MarkHost()
    {
        defaultColor.b = 0;
        readyColor.r = 255;
        OnReadyStateChange(ready);
    }
    #endregion
}
