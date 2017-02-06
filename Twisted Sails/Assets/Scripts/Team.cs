using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Initial code. Simple object representing a team.
//              Everything works as you'd expect. teamNumber is the team's index
//              in the gamemode-managed team list.

public class Team
{
    public string teamName;
    public Color teamColor;
    public short teamNumber;

    /// <summary>
    /// Creates a new team for use in game.
    /// </summary>
    /// <param name="name">Name of the team</param>
    /// <param name="color">Color of the team</param>
    /// <param name="number">Number of the team in gamemode's team array</param>
    public Team(string name, Color color, short number)
    {
        teamName = name;
        teamColor = color;
        teamNumber = number;
    }

    public Team() : this("???", Color.white, -1) { }
}
