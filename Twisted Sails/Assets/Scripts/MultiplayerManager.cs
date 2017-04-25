using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;

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

// Developer:   Diego Wilde
// Date:        2/19/2017
// Description: Player data persistence functionality added. Player statistics are currently saved clientside 
//              using binary incription to the PlayerData.gd file. Save is called whenever the game scene 
//              changes clientside, and loading is performed once during OnClientConnect().
//              - Added method to get the local player

// Developer:   Kyle Aycock
// Date:        2/24/2017
// Description: Minor changes to work with new lobby.

// Developer:   Kyle Aycock
// Date:        3/23/2017
// Description: Fixed bug having to do with Unity networking keeping separate
//              connectionIds on client and server.

// Developer:   Kyle Aycock
// Date:        3/24/2017
// Description: Added functionality for team-based spawn points

// Developer:   Kyle Aycock
// Date:        4/6/2017
// Description: Fixed bug where a player sometimes wouldn't be spawned in because the server still detected their lobby object
//              Fixed bug where players would sometimes send spawn messages before the server collected spawn points, causing players to not spawn in
//              Fixed connectionId bug on ships, fix was only applied to lobby objects previously

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
    public int localConnectionId;

    public GameObject[] playableShips;

    private float gameRestartTimer;

    private int[] startPositionIndices;
    private static Dictionary<int, List<Transform>> teamStartPositions;

    //Initialization of variables
    void Start()
    {
        localPlayerName = "???";
        localPlayerTeam = -1;
        playerList = new List<Player>();

        SetupGamemode();

        startPositionIndices = new int[currentGamemode.NumTeams()];
        teamStartPositions = new Dictionary<int, List<Transform>>();

        teamScores = new int[currentGamemode.NumTeams()];
        for (int i = 0; i < currentGamemode.NumTeams(); i++)
        {
            teamScores[i] = 0;
        }
    }

    void Update()
    {
        if (networkSceneName.Equals(inGameScene)) {
            currentGamemode.Update();
            if (NetworkServer.active)
            {
                if (gameRestartTimer <= 0)
                {
                    CheckEndGame();
                }
                else
                    CheckRestartGame();
            }
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
            Debug.LogError("Error: GetLocalClient called with no active client!");
            return null;
        }
        return singleton.client;
    }

    //DW: Call this method to get the local player object
    /// <summary>
    /// Returns the Player object of the local client player.
    /// Can only be used on client.
    /// </summary>
    /// <returns>The local Player object.</returns>
    public static Player GetLocalPlayer()
    {
        NetworkClient client = GetLocalClient();
        if(client != null)
            return FindPlayer(client.connection.connectionId);
        return null;
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

    public static string GetLocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
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

    public static bool IsGameJoinable()
    {
        LobbyManager lobby = GameObject.Find("Canvas").GetComponent<LobbyManager>();
        return (IsLobby() && lobby != null && lobby.IsJoinable() && GetInstance().playerList.Count < 8);
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
    private void SetupGamemode()
    {
        //Todo: Add UI in lobby to configure chosen gamemode & settings
        //For now, going with defaults specified by design doc
        Team[] teams = new Team[2];
        teams[0] = new Team("Red", Color.red, 0);
        teams[1] = new Team("Blue", Color.blue, 1);
        currentGamemode = new TeamDeathmatch(teams, 30, 300);
        startPositionIndices = new int[currentGamemode.NumTeams()];
        for (int i = 0; i < currentGamemode.NumTeams(); i++)
            startPositionIndices[i] = 0;
    }

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
                StopHost();
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
            killer.AddKill();
            Debug.Log("Player " + killer.name + " killed player " + victim.name + "!");
        }
        else Debug.Log("Player " + victim.name + " suicided!");

        Player.ActivateEventPlayerKilled(victim, killer);
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
        Debug.Log(string.Format("Performing spawn of player {0} (controllerId: {1}, conn: {2}) in {3}", playerName, playerControllerId, conn.ToString(), IsLobby() ? "lobby" : ("game (team: " + playerTeam + " ship: " + playerShip + ")")));
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
            //This sometimes happens when moving from lobby to game - just destroy the lobby object
            Destroy(conn.playerControllers[playerControllerId].gameObject);
            //if (LogFilter.logError) { Debug.LogError("There is already a player at that playerControllerId for this connections."); }
            //return;
        }

        GameObject playerObj;
        
        if (IsLobby())
        {
            //We are spawning a lobby representation of the player
            playerTeam = currentGamemode.AutoAssignTeam(playerList);
            playerShip = 0;
            Player player = new Player(playerName, playerTeam, NetworkInstanceId.Invalid, conn.connectionId);
            playerList.Add(player);
            playerObj = Instantiate(lobbyPrefab, Vector3.zero, Quaternion.identity);
            playerObj.GetComponent<PlayerIconController>().playerName = playerName;
            playerObj.GetComponent<PlayerIconController>().playerTeam = playerTeam;
            playerObj.GetComponent<PlayerIconController>().playerShip = playerShip;
            playerObj.GetComponent<PlayerIconController>().connectionId = conn.connectionId;
        }
        else {
            //We are spawning a ship in game
            Transform startPos = GetSpawnPoint(playerTeam);
            if (startPos != null)
            {
                playerObj = Instantiate(playableShips[playerShip], startPos.position, startPos.rotation);
            }
            else
            {
                //Try again - most likely scenario is that the player requested a spawn before the server found the spawn points
                Debug.Log("Spawn point not found - reattempting.");
                StartCoroutine(RetryAddPlayer(conn, playerControllerId, playerName, playerTeam, playerShip));
                return;
            }

            playerObj.GetComponent<Health>().team = playerTeam;
            playerObj.GetComponent<Health>().playerName = playerName;
            playerObj.GetComponent<Health>().connectionId = conn.connectionId;
        }

        PlayerConnected(FindPlayer(conn.connectionId));
        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
    }

    IEnumerator RetryAddPlayer(NetworkConnection conn, short playerControllerId, string playerName, short playerTeam, short playerShip)
    {
        yield return new WaitForSeconds(0.5f);
        ServerAddPlayer(conn, playerControllerId, playerName, playerTeam, playerShip);
    }

    public static void RegisterTeamStartPosition(Transform pos, int team)
    {
        if(!teamStartPositions.ContainsKey(team))
        {
            teamStartPositions.Add(team, new List<Transform>());
        }
        teamStartPositions[team].Add(pos);
    }

    public Transform GetSpawnPoint(int team)
    {
        //get the list of start positions corresponding to the given team,
        //and get from that list the next unused start position, incrementing afterwards
        if (!teamStartPositions.ContainsKey(team)) return null;
        if (startPositionIndices[team] >= teamStartPositions[team].Count) return null;
        return teamStartPositions[team][(startPositionIndices[team]++)%4];
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(!IsGameJoinable())
            conn.Disconnect();
        base.OnServerConnect(conn);
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

    public override void OnStartHost()
    {
        SetupGamemode();
        base.OnStartHost();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.Contains(inGameScene))
            GameStart();
        base.OnServerSceneChanged(sceneName);
    }

    //Called when the lobby transitions to the game
    public void EnterGame()
    {
        teamStartPositions.Clear(); //in case leftover data from last game
        ServerChangeScene(inGameScene);
    }

    //Convenience method for sending the current game state to all clients
    public void SendGameState()
    {
        NetworkServer.SendToAll(ExtMsgType.State, new GameStateMessage(playerList, teamScores, ((TeamDeathmatch)GetCurrentGamemode()).timeRemaining));
    }
    #endregion

    //Methods only for use clientside
    #region Client
    //Overridden to register handlers and prevent spawning at this stage
    //DW: Added a call to load save data, if it exists
    public override void OnClientConnect(NetworkConnection conn)
    {
        SaveLoad.Load();
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
        SaveLoad.SaveGame();
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

    public static void ClientNotifyGameEnd(short team)
    {
        GameEnd(team);
    }

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
        public float gamemodeTimeLeft;

        public GameStateMessage()
        {
        }

        public GameStateMessage(List<Player> plyList, int[] scores, float currentTime)
        {
            playerList = plyList;
            teamScores = scores;
            gamemodeTimeLeft = currentTime;
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
            writer.Write(gamemodeTimeLeft);
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
            gamemodeTimeLeft = reader.ReadSingle();
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
    Human,
    Trireme,
    Bramble,
    Dragon
}
