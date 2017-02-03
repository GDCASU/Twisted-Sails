using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: This class is responsible for bouncing input to the
//              object representing the player, which bounces it to the server.
//              It is currently also responsible for controlling the UI.
//              It is sufficient for now, but probably should networked later.

// Developer:   Nizar Kury
// Date:        11/17/2016
// Description: Added the SwitchShip() class which deals with a player picking a ship
//              they want to use. Also added shipIcons instance variable for the 
//              representation of a person's selection.

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Adapted to work with new team system

public class LobbyManager : MonoBehaviour
{
    public RectTransform[] teams;
    public GameObject readyButton;

    // index corresponds to ship type. In this case,
    // 0 = trireme
    // 1 = human
    // 2 = living wood
    public Sprite[] shipIcons;

    private MultiplayerManager manager;
    private bool allReady;

    // Use this for initialization
    void Start()
    {
        //Variable initialization
        manager = MultiplayerManager.GetInstance();
        allReady = false;

        //This if statement statement checks if it's running on the host
        if (NetworkServer.active)
        {
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
            readyButton.GetComponent<Button>().interactable = false;
            SetAllReady(true);
        }
    }

    /// <summary>
    /// Called by server when all players except host have readied up, or when
    /// a new player joins
    /// </summary>
    /// <param name="ready">Are all players except the host ready?</param>
    public void SetAllReady(bool ready)
    {
        allReady = ready;
        readyButton.GetComponent<Button>().interactable = allReady;
    }

    /// <summary>
    /// Called when a player clicks the ready/start button
    /// </summary>
    public void Ready()
    {
        PlayerIconController controller = GetLocalPlayer().GetComponent<PlayerIconController>();
        controller.CmdReady(!controller.ready);
    }

    /// <summary>
    /// Called when a player clicks a team region to switch to that team
    /// </summary>
    /// <param name="team">Index in array of teams to switch to</param>
    public void SwitchTeam(int team)
    {
        manager.localPlayerTeam = (short)team;
        GetLocalPlayer().GetComponent<PlayerIconController>().CmdMoveReq((short)team, teams[team].gameObject.name, new Vector2(Random.Range(-75, 75), Random.Range(-75, 75)));
    }

    // NK
    /// <summary>
    /// Called when a player clicks a ship button to switch to that ship
    /// </summary>
    /// <param name="ship">Index in array of ships to switch to</param>
    public void SwitchShip(int ship)
    {
        manager.localShipType = (Ship)ship;
        GetLocalPlayer().GetComponent<PlayerIconController>().CmdChangeShip((Ship)ship);
    }

    /// <summary>
    /// Convenience method to fetch local player object (only works clientside)
    /// </summary>
    private GameObject GetLocalPlayer()
    {
        return manager.client.connection.playerControllers[0].gameObject;
    }
}
