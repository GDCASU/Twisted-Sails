using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer:   Kyle Aycock
// Date:        3/24/2017
// Description: This class should be attached to an empty gameobject to indicate that
//              players should spawn there. Team number must be set.

public class TeamSpawnPoint : MonoBehaviour {

    public int teamNumber;

    //Register this spawn point
    public void Awake()
    {
        MultiplayerManager.RegisterTeamStartPosition(transform, teamNumber);
    }
}
