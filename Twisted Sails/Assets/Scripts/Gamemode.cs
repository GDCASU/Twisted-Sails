using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Initial code. This class should be extended when a new gamemode needs to be created.
//              See the documentation and the TeamDeathmatch class for usage info.

public class Gamemode
{
    public string name;
    public Team[] teams;

    /// <summary>
    /// Use this for whenever any form of timekeeping might be needed   
    /// </summary>
    public virtual void Update()
    {

    }

    /// <summary>
    /// Returns -1 if the game is not over, or the winning team's number if the game is over.
    /// Returns -2 if there was a tie.
    /// Called every update frame in order to check for all kinds of wins.
    /// </summary>
    /// <param name="teamScores">Current team scores array from MultiplayerManager</param>
    /// <param name="playerList">Current player list</param>
    /// <returns>Winning team # or -1 or -2</returns>
    public virtual short CheckEndCondition(int[] teamScores, List<Player> playerList)
    {
        return -1;
    }

    /// <summary>
    /// Returns the team a player who has not chosen a team yet should be assigned to
    /// </summary>
    /// <param name="playerList">Current list of players</param>
    /// <returns>Team for player to be assigned to</returns>
    public virtual short AutoAssignTeam(List<Player> playerList)
    {
        return -1;
    }

    public int NumTeams()
    {
        return teams.Length;
    }
}
