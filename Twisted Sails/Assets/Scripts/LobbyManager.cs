﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: This class takes care of the "administrative" work of running the lobby.
//              It is also what all the buttons, etc. are hooked up to by default.
//              It passes input to the local icon, which bounces it to the server.

// Developer:   Nizar Kury
// Date:        11/17/2016
// Description: Added the SwitchShip() class which deals with a player picking a ship
//              they want to use. Also added shipIcons instance variable for the 
//              representation of a person's selection.

// This is mainly a clientside class
public class LobbyManager : MonoBehaviour
{
    public RectTransform[] teams; //assigned in inspector
    public GameObject readyButton; //assigned in inspector

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
        manager = MultiplayerManager.instance;
        allReady = false;
        if (NetworkServer.active)
        {
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
            readyButton.GetComponent<Button>().interactable = false;
            SetAllReady(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Called by server when all players except host have readied up, or when
    /// a new player joins
    /// </summary>
    /// <param name="ready"></param>
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
        GetLocalPlayer().GetComponent<PlayerIconController>().CmdReady();
    }

    /// <summary>
    /// Called when a player clicks a team region to switch to that team
    /// </summary>
    /// <param name="team">Index in array of teams to switch to</param>
    public void SwitchTeam(int team)
    {
        manager.localPlayerTeam = (Team)team;
        GetLocalPlayer().GetComponent<PlayerIconController>().CmdMoveReq((Team)team, teams[team].gameObject.name, new Vector2(Random.Range(-75, 75), Random.Range(-75, 75)));
    }

    // NK
    /// <summary>
    /// Called when a player clicks a ship button to switch to that ship
    /// </summary>
    /// <param name="ship">Index in array of ships to switch to</param>
    public void SwitchShip(int ship)
    {
        manager.localShipType = (Ship)ship;
        GetLocalPlayer().GetComponent<PlayerIconController>().CmdChangeShip((Ship)ship, shipIcons);
    }

    //Convenience method to fetch local player object (only works clientside)
    private GameObject GetLocalPlayer()
    {
        return manager.client.connection.playerControllers[0].gameObject;
    }
}
