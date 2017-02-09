﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Initial code. This class serves as both the default gamemode
//              and a template for any future gamemodes.

class TeamDeathmatch : Gamemode
{
    public int pointsToWin;
    public float timeRemaining;

    //Gamemode-specific variables and functionality is configured through its constructor
    public TeamDeathmatch(Team[] teams, int pointsToWin, float matchTime)
    {
        name = "Team Deathmatch";
        this.teams = teams;
        this.pointsToWin = pointsToWin;
        timeRemaining = matchTime;
    }

    //Called every update frame as usual, so Time.deltaTime still valid
    public override void Update()
    {
        timeRemaining -= Time.deltaTime;
    }

    //This is called every update frame as well, every kind of end condition should be checked here
    //A return value of -1 indicates the game is not over, a return value of -2 indicates a tie.
    public override short CheckEndCondition(int[] teamScores, List<Player> playerList)
    {
        for (short i = 0; i < teams.Length; i++)
            if (teamScores[i] >= pointsToWin)
            {
                return i;
            }
        if (timeRemaining <= 0)
        {
            int winningTeam = -2;
            int winningTeamPoints = 0;
            for (short i = 0; i < teams.Length; i++)
            {
                if (teamScores[i] > winningTeamPoints)
                {
                    winningTeam = i;
                    winningTeamPoints = teamScores[i];
                }
                else if (teamScores[i] == winningTeamPoints)
                {
                    winningTeam = -2;
                }
            }
        }
        return -1;
    }

    //If the player enters the game without having chosen a team, their team is autoassigned based on this code
    public override short AutoAssignTeam(List<Player> playerList)
    {
        short playerTeam;
        int team1Count = playerList.FindAll(p => p.team == 1).Count;
        int team2Count = playerList.FindAll(p => p.team == 2).Count;
        if (team1Count > team2Count)
            playerTeam = 1;
        else
            playerTeam = 2;
        return playerTeam;
    }
}
