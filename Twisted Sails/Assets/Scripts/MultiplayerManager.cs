using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

// Developer: Kyle Aycock
// Date: 11/3/2016
// This is a class inheriting from the default Unity NetworkManager that should be used
// to make changes to the way NetworkManager operates. It is extremely helpful to reference
// the code for Unity's NetworkManager which can be found open source at this link:
// https://bitbucket.org/Unity-Technologies/networking/
//
// This class is currently serving as a general multiplayer game controller. The functionality
// of the default NetworkManager has been extended to make it easier to work with.
// Also included are two enums and a container class that I didn't feel deserved their own file.

//Enum for all teams
public enum Team
{
    Red,
    Blue,
    Spectator
}

//Unnecessary right now but can be extended to include other gamemodes and time/stock options
public enum Gamemode
{
    TeamDeathmatch
}

//container class for all info for a single player
public class Player
{
    public string name;
    public Team team;
    public NetworkInstanceId shipId;
    public int connectionId;
    public int kills;
    public int deaths;
    public int bounty;
    public int score;

    public Player(string name, Team team, NetworkInstanceId shipId, int connectionId)
    {
        this.name = name;
        this.team = team;
        this.shipId = shipId;
        this.connectionId = connectionId;
        kills = 0;
        deaths = 0;
        bounty = 0;
        score = 0;
    }
}

public class MultiplayerManager : NetworkManager
{

    public Gamemode currentGamemode = Gamemode.TeamDeathmatch;
    public string localPlayerName;
    public Team localPlayerTeam;
    public List<Player> playerList;
    public Dictionary<Team, int> teamScores;
    public static Player localPlayer;
    public static MultiplayerManager instance;

    private float gameRestartTimer;

    //Initialization of variables
    public void Start()
    {
        localPlayerName = "???";
        localPlayerTeam = Team.Spectator;
        playerList = new List<Player>();
        instance = this;
        teamScores = new Dictionary<Team, int>();
        foreach (Team team in Enum.GetValues(typeof(Team)))
        {
            teamScores[team] = 0;
        }
    }

    //Only used for restarting the game
    public void Update()
    {
        if (gameRestartTimer > 0)
        {
            gameRestartTimer -= Time.deltaTime;
            if (gameRestartTimer <= 0)
            {
                foreach (Team team in Enum.GetValues(typeof(Team)))
                {
                    teamScores[team] = 0;
                }
                foreach (Player player in playerList)
                    NetworkServer.FindLocalObject(player.shipId).GetComponent<Health>().RpcRestartGame();
            }
        }
    }

    //Processes a player death, this method is called from the Health script
    public void PlayerKill(NetworkInstanceId killed, NetworkInstanceId by)
    {
        Player victim = playerList.Find(p => p.shipId == killed);
        Player killer = playerList.Find(p => p.shipId == by);
        victim.deaths++;
        if (killer != null && killer.team != victim.team)
        {
            killer.kills++;
            teamScores[killer.team] += 1;
            Debug.Log("Player " + killer.name + " killed player " + victim.name + "!");
        }
        else Debug.Log("Player " + victim.name + " suicided!");
        CheckEndGame();
    }

    //The functionality of this method can be expanded when multiple gamemodes are added
    public void CheckEndGame()
    {
        foreach (Team team in Enum.GetValues(typeof(Team)))
            if (teamScores[team] >= 10)
            {
                gameRestartTimer = 10;
                foreach (Player player in playerList)
                    NetworkServer.FindLocalObject(player.shipId).GetComponent<Health>().RpcEndGame(team, teamScores[Team.Red], teamScores[Team.Blue]);
            }
    }

    //This hook method is automatically called when the server is instructed to spawn a player
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        ServerAddPlayer(conn, playerControllerId, "???", Team.Spectator);
    }

    //This hook method is automatically called when the server is instructed to spawn a player (and provided extra info)
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        SpawnMessage msg = new SpawnMessage("", Team.Spectator);
        msg.Deserialize(extraMessageReader);
        ServerAddPlayer(conn, playerControllerId, msg.name, (Team)msg.team);

    }

    //most of the code for this taken from the internal ServerAddPlayer method used by the default NetworkManager
    //This method performs the actual spawning of a player
    void ServerAddPlayer(NetworkConnection conn, short playerControllerId, string playerName, Team playerTeam)
    {
        if (playerPrefab == null)
        {
            if (LogFilter.logError) { Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object."); }
            return;
        }

        if (playerPrefab.GetComponent<NetworkIdentity>() == null)
        {
            if (LogFilter.logError) { Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab."); }
            return;
        }

        if (playerControllerId < conn.playerControllers.Count && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
        {
            if (LogFilter.logError) { Debug.LogError("There is already a player at that playerControllerId for this connections."); }
            return;
        }

        GameObject player;
        Transform startPos = GetStartPosition();
        if (startPos != null)
        {
            player = (GameObject)Instantiate(playerPrefab, startPos.position, startPos.rotation);
        }
        else
        {
            player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        if(playerTeam == Team.Spectator)
        {
            int redCount = playerList.FindAll(p => p.team == Team.Red).Count;
            int blueCount = playerList.FindAll(p => p.team == Team.Blue).Count;
            if (redCount > blueCount)
                playerTeam = Team.Blue;
            else
                playerTeam = Team.Red;
        }
        player.GetComponent<Health>().team = playerTeam;
        player.GetComponent<Health>().playerName = playerName;

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    //Only purpose of overriding this is to make it do nothing
    //The base method readies the scene & spawns the player immediately upon connection
    public override void OnClientConnect(NetworkConnection conn)
    {
    }

    //Only purpose of overriding is to properly manage the playerList
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerList.Remove(playerList.Find(x => x.connectionId == conn.connectionId));
        base.OnServerRemovePlayer(conn, player);
    }

    //Called by NetworkHUD - tells the network client to tell the server to spawn a player
    //The result of this method is that OnServerAddPlayer is called
    public void SpawnClient()
    {
        ClientScene.AddPlayer(client.connection, 0, new SpawnMessage(localPlayerName, localPlayerTeam));
    }

    //This is a custom message to send name and team choices to the server
    class SpawnMessage : MessageBase
    {
        public string name;
        public short team;

        public SpawnMessage(string name, Team team)
        {
            this.name = name;
            this.team = (short)team;
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(name);
            writer.Write(team);
        }

        public override void Deserialize(NetworkReader reader)
        {
            name = reader.ReadString();
            team = reader.ReadInt16();
        }
    }
}