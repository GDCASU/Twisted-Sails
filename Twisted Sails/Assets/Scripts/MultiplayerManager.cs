﻿using UnityEngine;
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
// Also included are two enums and several message classes that I didn't feel deserved their own file.
// Added code for broadcasting current game state (score, player stats, etc) to all players whenever
// a change is made. This may be inefficient, but it's okay for now. At the very least it's needed
// for when a new player joins, to synchronize the current game state with that player and tell everyone
// else that a new player has joined. Also added stuff to help with the scoreboard and score feed.

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Expanded to include lobby functionality
//              Fixed a bunch of stuff to allow more control over when/what the player object spawns in

// Developer:   Nizar Kury
// Date:        11/17/2016
// Description: Expanded to include ship selection
//              - Modified SpawnMessage
//              - Added localShipType
//              

// Developer:   Kyle Aycock
// Date:        1/11/2016
// Description: Added a selection of convenience methods that should be used by other scripts
//              to interact with the MultiplayerManager. The intention is that if another programmer
//              needs to do something with networking, they may simply refer to the static methods either here
//              or in the autocomplete and find the one that does what they need to do. Will most likely need to
//              be expanded upon after designers start doing more stuff and the need for more methods arises.
//              Documentation will be written to aid with this.

// Developer:   Kyle Aycock
// Date:        1/26/2017
// Description: Adapted this class to work with new Gamemode and Team classes
//              Fixed a bug having to do with starting a session after ending one without restarting the client
//              Fixed a bug having to do with hosting a game after previously joining one
//              Added several events for use by other coders
//              Added code to trigger Player's events at appropriate times
//              Fixed a bug preventing player ship choice from being sent to the server

public class MultiplayerManager : NetworkManager
{
    public Gamemode currentGamemode;
    public string localPlayerName;
    public short localPlayerTeam;
    public Ship localShipType;
    public int pointsToWin;
    public List<Player> playerList;
    public int[] teamScores;
    public GameObject lobbyPrefab;
    public string inGameScene;

    public GameObject[] playableShips;

    private float gameRestartTimer;

    //Initialization of variables
    void Start()
    {
        localPlayerName = "???";
        localPlayerTeam = -1;
        playerList = new List<Player>();

        //Todo: Add UI in lobby to configure chosen gamemode & settings
        //For now, going with defaults specified by design doc
        Team[] teams = new Team[2];
        teams[0] = new Team("Red", Color.red, 0);
        teams[1] = new Team("Blue", Color.blue, 1);
        currentGamemode = new TeamDeathmatch(teams, 30, 300);

        teamScores = new int[currentGamemode.NumTeams()];
        for (int i = 0; i < currentGamemode.NumTeams(); i++)
        {
            teamScores[i] = 0;
        }
    }

    void Update()
    {
        if (NetworkServer.active)
        {
            CheckRestartGame();
        }
        if (networkSceneName.Equals(inGameScene))
        {
            currentGamemode.Update();
            CheckEndGame();
        }
    }

    #region Static Methods

    /// <summary>
    /// Checks if game is currently in lobby phase.
    /// </summary>
    /// <returns>True if in lobby, false if otherwise.</returns>
    public static bool IsLobby()
    {
        return networkSceneName.Equals(singleton.onlineScene);
    }

    /// <summary>
    /// Checks if current code is being run on the client.
    /// </summary>
    /// <returns>True if on client, false otherwise.</returns>
    public static bool IsClient()
    {
        return NetworkClient.active;
    }

    /// <summary>
    /// Checks if current code is being run on the server.
    /// </summary>
    /// <returns>True if on server, false otherwise.</returns>
    public static bool IsServer()
    {
        return NetworkServer.active;
    }

    /// <summary>
    /// Checks if current code is being run on the host.
    /// </summary>
    /// <returns>True if code is being run on the host, false otherwise</returns>
    public static bool IsHost()
    {
        return NetworkClient.active && NetworkServer.active;
    }

    /// <summary>
    /// Returns the NetworkClient object of the local player connection.
    /// Can only be used on client.
    /// </summary>
    /// <returns>The local NetworkClient</returns>
    public static NetworkClient GetLocalClient()
    {
        if (!NetworkClient.active)
        {
            Debug.LogError("Error: GetLocalClient called on server!");
            return null;
        }
        return singleton.client;
    }

    /// <summary>
    /// Returns the combined score of all players on a given team.
    /// Can be used on client and server.
    /// </summary>
    /// <param name="team">Team for which the total score will be returned</param>
    /// <returns></returns>
    public static int GetTeamScore(short team)
    {
        return GetInstance().teamScores[team];
    }

    /// <summary>
    /// Returns the combined score of all players on a given team.
    /// Can be used on client and server.
    /// </summary>
    /// <param name="team">Team for which the total score will be returned</param>
    /// <returns></returns>
    public static int GetTeamScore(Team team)
    {
        return GetTeamScore(team.teamNumber);
    }

    /// <summary>
    /// Returns the player whose name matches the given name.
    /// Can be used on client and server.
    /// If two players have the same name, the one that connected first is returned.
    /// It is recommended to use an overload of this method.
    /// </summary>
    /// <param name="name">The name of the player to find</param>
    /// <returns></returns>
    public static Player FindPlayer(string name)
    {
        return GetInstance().playerList.Find(p => p.name.Equals(name));
    }

    /// <summary>
    /// Returns the player whose connectionId matches the given one.
    /// Can be used on client and server.
    /// </summary>
    /// <param name="connectionId">The connectionId of the player to find</param>
    /// <returns></returns>
    public static Player FindPlayer(int connectionId)
    {
        return GetInstance().playerList.Find(p => p.connectionId == connectionId);
    }

    /// <summary>
    /// Returns the player whose ship has the given NetworkInstanceId.
    /// Can be used on client and server.
    /// </summary>
    /// <param name="shipId">The NetworkInstanceId of the ship whose owner will be found</param>
    /// <returns></returns>
    public static Player FindPlayer(NetworkInstanceId shipId)
    {
        return GetInstance().playerList.Find(p => p.objectId == shipId);
    }

    /// <summary>
    /// Returns the current gamemode object.
    /// </summary>
    /// <returns>The current gamemode object.</returns>
    public static Gamemode GetCurrentGamemode()
    {
        return GetInstance().currentGamemode;
    }

    /// <summary>
    /// Returns the team with given team number.
    /// </summary>
    /// <param name="index">Team with given team number.</param>
    /// <returns></returns>
    public static Team GetTeam(short number)
    {
        return GetInstance().currentGamemode.teams[number];
    }

    /// <summary>
    /// Only use this if you need access to something within the MultiplayerManager not given by other methods.
    /// </summary>
    /// <returns>Active instance of MultiplayerManager</returns>
    public static MultiplayerManager GetInstance()
    {
        return singleton as MultiplayerManager;
    }
    #endregion

    #region Events
    public static event Action GameStart = delegate { };

    public delegate void GameEndEvent(short winningTeam);
    public static event GameEndEvent GameEnd = delegate { };

    public delegate void PlayerEvent(Player player);
    public static event PlayerEvent PlayerConnected = delegate { };
    public static event PlayerEvent PlayerDisconnected = delegate { };

    #endregion

    //Methods only for use serverside
    #region Server
    //Timer to restart game
    private void CheckRestartGame()
    {
        if (gameRestartTimer > 0)
        {
            gameRestartTimer -= Time.deltaTime;
            if (gameRestartTimer <= 0)
            {
                for (int i = 0; i < currentGamemode.NumTeams(); i++)
                {
                    teamScores[i] = 0;
                }
                foreach (Player player in playerList)
                    NetworkServer.FindLocalObject(player.objectId).GetComponent<Health>().RpcRestartGame();
            }
        }
    }

    //Processes a player death, this method is called from the Health script
    public void PlayerKill(Player killed, NetworkInstanceId by)
    {
        Player victim = killed;
        Player killer = playerList.Find(p => p.objectId == by);
        victim.deaths++;
        if (killer != null && killer.team != victim.team)
        {
            killer.kills++;
            int scoreGain = victim.GetBounty() + 1;
            teamScores[killer.team] += scoreGain;
            killer.score += (short)scoreGain;
            KillMessage msg = new KillMessage();
            msg.bountyGained = victim.GetBounty();
            NetworkServer.SendToClient(killer.connectionId, ExtMsgType.Kill, msg);
            victim.killstreak = 0;
            killer.killstreak++;
            Debug.Log("Player " + killer.name + " killed player " + victim.name + "!");
        }
        else Debug.Log("Player " + victim.name + " suicided!");

        Player.SendPlayerKilled(victim, killer);
        SendGameState();
        CheckEndGame();
    }

    //The functionality of this method can be expanded when multiple gamemodes are added
    private void CheckEndGame()
    {
        short res = currentGamemode.CheckEndCondition(teamScores, playerList);
        if (res >= 0)
        {
            GameEnd(res);
            gameRestartTimer = 10;
            foreach (Player player in playerList)
                NetworkServer.FindLocalObject(player.objectId).GetComponent<Health>().RpcEndGame(res, teamScores);
        }
    }

    //This hook method is automatically called when the server is instructed to spawn a player
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        ServerAddPlayer(conn, playerControllerId, "???", -1, 0);
    }

    //This hook method is automatically called when the server is instructed to spawn a player (and provided extra info)
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        SpawnMessage msg = new SpawnMessage("", -1, 0);
        msg.Deserialize(extraMessageReader);
        ServerAddPlayer(conn, playerControllerId, msg.name, msg.team, msg.ship);

    }

    //most of the code for this taken from the internal ServerAddPlayer method used by the default NetworkManager
    //This method performs the actual spawning of a player
    void ServerAddPlayer(NetworkConnection conn, short playerControllerId, string playerName, short playerTeam, short playerShip)
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

        GameObject playerObj;
        Transform startPos = GetStartPosition();
        if (IsLobby())
        {
            //We are spawning a lobby icon representation of the player
            Player player = new Player(playerName, playerTeam, NetworkInstanceId.Invalid, conn.connectionId);
            playerList.Add(player);
            if (startPos != null)
            {
                RectTransform rectTransform = (RectTransform)startPos;
                playerObj = (GameObject)Instantiate(lobbyPrefab, Vector3.zero, startPos.rotation);
                playerObj.transform.SetParent(rectTransform.parent, false);
                ((RectTransform)playerObj.transform).anchoredPosition = rectTransform.anchoredPosition;
                playerObj.GetComponent<PlayerIconController>().effectivePosition = rectTransform.anchoredPosition;
            }
            else
            {
                //shouldn't happen - needs failsafe
                playerObj = (GameObject)Instantiate(lobbyPrefab, Vector3.zero, Quaternion.identity);
            }
            playerObj.GetComponent<PlayerIconController>().playerName = playerName;
        }
        else {
            //We are spawning a ship in game
            if (startPos != null)
            {
                playerObj = (GameObject)Instantiate(playableShips[playerShip], startPos.position, startPos.rotation);
            }
            else
            {
                playerObj = (GameObject)Instantiate(playableShips[playerShip], Vector3.zero, Quaternion.identity);
            }

            //Player team not chosen - ask gamemode to autoassign
            if (playerTeam == -1)
            {
                playerTeam = currentGamemode.AutoAssignTeam(playerList);
            }
            playerObj.GetComponent<Health>().team = playerTeam;
            playerObj.GetComponent<Health>().playerName = playerName;
        }

        PlayerConnected(FindPlayer(conn.connectionId));
        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
    }

    //Overridden for purposes of maintaining playerlist
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        playerList.Remove(playerList.Find(x => x.connectionId == conn.connectionId));
        PlayerDisconnected(FindPlayer(conn.connectionId));
        SendGameState();
        base.OnServerDisconnect(conn);
    }

    //Overridden for purposes of maintaining playerlist
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerList.Remove(playerList.Find(x => x.connectionId == conn.connectionId));
        PlayerDisconnected(FindPlayer(conn.connectionId));
        SendGameState();
        base.OnServerRemovePlayer(conn, player);
    }

    //Overridden for purposes of maintaining playerlist
    public override void OnStopServer()
    {
        playerList.Clear();
        base.OnStopServer();
    }

    public override void OnStopHost()
    {
        playerList.Clear();
        base.OnStopHost();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == inGameScene)
            GameStart();
        base.OnServerSceneChanged(sceneName);
    }

    //Called when a player clicks 'ready' in the lobby
    public void SetPlayerReady(NetworkInstanceId source, bool ready)
    {
        playerList.Find(p => p.objectId == source).ready = ready;
        if (IsLobby())
        {
            LobbyManager manager = GameObject.Find("Canvas").GetComponent<LobbyManager>();
            int readyCount = playerList.FindAll(p => p.ready).Count;
            if (readyCount == playerList.Count - 1)
                manager.SetAllReady(true);
            else if (readyCount == playerList.Count)
                ServerChangeScene(inGameScene);
            else
                manager.SetAllReady(false);
        }
    }

    //Convenience method for sending the current game state to all clients
    public void SendGameState()
    {
        NetworkServer.SendToAll(ExtMsgType.State, new GameStateMessage(playerList, teamScores));
    }
    #endregion

    //Methods only for use clientside
    #region Client
    //Overridden to register handlers and prevent spawning at this stage
    public override void OnClientConnect(NetworkConnection conn)
    {
        client.RegisterHandler(ExtMsgType.State, OnStateUpdate);
        if (!ClientScene.ready) ClientScene.Ready(client.connection);
    }

    //Overridden to customize player spawning
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        SpawnClient();
        if (!ClientScene.ready) ClientScene.Ready(client.connection);
    }

    //Hook to receive & update game state
    public void OnStateUpdate(NetworkMessage netMsg)
    {
        GameStateMessage msg = netMsg.ReadMessage<GameStateMessage>();
        playerList = msg.playerList;
        teamScores = msg.teamScores;
    }

    //Tells the network client to tell the server to spawn a player
    //The result of this method is that OnServerAddPlayer is called
    public void SpawnClient()
    {
        //Most of this code taken from original NetworkManager
        bool addPlayer = (ClientScene.localPlayers.Count == 0);
        bool foundPlayer = false;
        foreach (var playerController in ClientScene.localPlayers)
        {
            if (playerController.gameObject != null)
            {
                foundPlayer = true;
                break;
            }
        }
        if (!foundPlayer)
        {
            // there are players, but their game objects have all been deleted
            addPlayer = true;
        }
        if (addPlayer)
        {
            ClientScene.AddPlayer(client.connection, 0, new SpawnMessage(localPlayerName, localPlayerTeam, localShipType));
        }
        client.RegisterHandler(ExtMsgType.Kill, GetComponent<NetworkHUD>().OnKill);
    }

    //Need to clear the client's player list too
    public override void OnStopClient()
    {
        playerList.Clear();
        base.OnStopClient();
    }
    #endregion

    //Custom messages for sending packets of information between client and server
    #region Messages
    //All new message types must be registered here using this format with the exception of SpawnMessage.
    public class ExtMsgType
    {
        public static short State = MsgType.Highest + 1;
        public static short Kill = MsgType.Highest + 2;
        public static short Ready = MsgType.Highest + 3;
    }

    //This message is sent from client to server to tell the server what the client chose for name & team.
    //It needs to be a message because name information is chosen before the server is started and before
    //the player has authority over any object with which to send commands.
    class SpawnMessage : MessageBase
    {
        public string name;
        public short team;
        public short ship;

        public SpawnMessage(string name, short team, Ship ship)
        {
            this.name = name;
            this.team = team;
            this.ship = (short)ship;
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(name);
            writer.Write(team);
            writer.Write(ship);
        }

        public override void Deserialize(NetworkReader reader)
        {
            name = reader.ReadString();
            team = reader.ReadInt16();
            ship = reader.ReadInt16();
        }
    }

    //This message is broadcasted to clients whenever the state of the game is changed.
    //For example, player joins, player leaves, team scores, player scores, etc.
    class GameStateMessage : MessageBase
    {
        public List<Player> playerList;
        public int[] teamScores;

        public GameStateMessage()
        {
        }

        public GameStateMessage(List<Player> plyList, int[] scores)
        {
            playerList = plyList;
            teamScores = scores;
        }

        public override void Serialize(NetworkWriter writer)
        {
            for (int i = 0; i < GetCurrentGamemode().NumTeams(); i++)
            {
                writer.Write(teamScores[i]);
            }
            writer.Write(playerList.Count);
            foreach (Player player in playerList)
            {
                player.Serialize(writer);
            }
        }

        public override void Deserialize(NetworkReader reader)
        {
            teamScores = new int[GetCurrentGamemode().NumTeams()];
            playerList = new List<Player>();
            for (int i = 0; i < GetCurrentGamemode().NumTeams(); i++)
            {
                teamScores[i] = reader.ReadInt32();
            }
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Player player = new Player();
                player.Deserialize(reader);
                playerList.Add(player);
            }
        }
    }

    //This message is sent from server to the client that just made a kill.
    //The client merely receiving this message lets it know it just made a kill,
    //and variables within the message notifies it of any additional score it gained.
    public class KillMessage : MessageBase
    {
        public int bountyGained;
    }

    //Sent from client to server when a client clicks the "Ready" button in the lobby.
    public class ReadyMessage : MessageBase
    {
        public bool ready;
    }
    #endregion
}

// NK: Enums for type of ship
public enum Ship
{
    Trireme,
    Human,
    Bramble
}
