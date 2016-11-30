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

public class MultiplayerManager : NetworkManager
{
    public Gamemode currentGamemode = Gamemode.TeamDeathmatch;
    public string localPlayerName;
    public Team localPlayerTeam;
    public Ship localShipType;
    public int pointsToWin;
    public List<Player> playerList;
    public Dictionary<Team, int> teamScores;
    public static MultiplayerManager instance;
    public GameObject lobbyPrefab;
    public string inGameScene;

    private float gameRestartTimer;

    //Initialization of variables
    void Start()
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

    void Update()
    {
        if (NetworkServer.active)
        {
            CheckRestartGame();
        }
    }

    //Checks if the current scene is lobby
    public bool IsLobby()
    {
        return networkSceneName.Equals(onlineScene);
    }

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
                foreach (Team team in Enum.GetValues(typeof(Team)))
                {
                    teamScores[team] = 0;
                }
                foreach (Player player in playerList)
                    NetworkServer.FindLocalObject(player.objectId).GetComponent<Health>().RpcRestartGame();
            }
        }
    }

    //Processes a player death, this method is called from the Health script
    public void PlayerKill(NetworkInstanceId killed, NetworkInstanceId by)
    {
        Player victim = playerList.Find(p => p.objectId == killed);
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

        SendGameState();
        CheckEndGame();
    }

    //The functionality of this method can be expanded when multiple gamemodes are added
    private void CheckEndGame()
    {
        foreach (Team team in Enum.GetValues(typeof(Team)))
            if (teamScores[team] >= pointsToWin)
            {
                gameRestartTimer = 10;
                foreach (Player player in playerList)
                    NetworkServer.FindLocalObject(player.objectId).GetComponent<Health>().RpcEndGame(team, teamScores[Team.Red], teamScores[Team.Blue]);
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
        SpawnMessage msg = new SpawnMessage("", Team.Spectator, 0); 
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
                playerObj = (GameObject)Instantiate(playerPrefab, startPos.position, startPos.rotation);
            }
            else
            {
                playerObj = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            }

            //Actual spectator functionality can be added in later, but for now it's basically "unknown"
            //Autoassign team based on whichever has less players
            if (playerTeam == Team.Spectator)
            {
                int redCount = playerList.FindAll(p => p.team == Team.Red).Count;
                int blueCount = playerList.FindAll(p => p.team == Team.Blue).Count;
                if (redCount > blueCount)
                    playerTeam = Team.Blue;
                else
                    playerTeam = Team.Red;
            }
            playerObj.GetComponent<Health>().team = playerTeam;
            playerObj.GetComponent<Health>().playerName = playerName;
        }

        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
    }

    //Overridden for purposes of maintaining playerlist
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        playerList.Remove(playerList.Find(x => x.connectionId == conn.connectionId));
        SendGameState();
        base.OnServerDisconnect(conn);
    }

    //Overridden for purposes of maintaining playerlist
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerList.Remove(playerList.Find(x => x.connectionId == conn.connectionId));
        SendGameState();
        base.OnServerRemovePlayer(conn, player);
    }

    //Overridden for purposes of maintaining playerlist
    public override void OnStopServer()
    {
        playerList.Clear();
        base.OnStopServer();
    }

    //Hooks an object to a Player
    public void RegisterPlayer(int connId, NetworkInstanceId objId)
    {
        playerList.Find(p => p.connectionId == connId).objectId = objId;
        if (IsLobby() && playerList.Count == 1) //bad code
            NetworkServer.FindLocalObject(objId).GetComponent<PlayerIconController>().RpcMarkHost();
        SendGameState();
    }

    //Changes a player's team
    public void ChangePlayerTeam(NetworkInstanceId source, Team newTeam)
    {
        playerList.Find(p => p.objectId == source).team = newTeam;
        SendGameState();
    }

    //NK
    //Changes a player's ship to their selected one and sends the changes out
    public void ChangePlayerShip(NetworkInstanceId source, Ship newShip)
    {
        playerList.Find(p => p.objectId == source).ship = newShip;
        SendGameState();
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
    }

    //Overridden to customize player spawning
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (!ClientScene.ready) ClientScene.Ready(client.connection);
        SpawnClient();
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

        public SpawnMessage(string name, Team team, Ship ship)
        {
            this.name = name;
            this.team = (short)team;
            this.ship = (short)ship;
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

    //This message is broadcasted to clients whenever the state of the game is changed.
    //For example, player joins, player leaves, team scores, player scores, etc.
    class GameStateMessage : MessageBase
    {
        public List<Player> playerList;
        public Dictionary<Team, int> teamScores;

        public GameStateMessage()
        {
            playerList = new List<Player>();
            teamScores = new Dictionary<Team, int>();
        }

        public GameStateMessage(List<Player> plyList, Dictionary<Team, int> scores)
        {
            playerList = plyList;
            teamScores = scores;
        }

        public override void Serialize(NetworkWriter writer)
        {
            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                writer.Write(teamScores[team]);
            }
            writer.Write(playerList.Count);
            foreach (Player player in playerList)
            {
                player.Serialize(writer);
            }
        }

        public override void Deserialize(NetworkReader reader)
        {
            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                teamScores[team] = reader.ReadInt32();
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

//Enum for all teams
public enum Team
{
    Red,
    Blue,
    Spectator
}

// NK: Enums for type of ship
public enum Ship
{
    Trireme,
    Human,
    Bramble
}

//Unnecessary right now but can be extended to include other gamemodes and time/stock options
public enum Gamemode
{
    TeamDeathmatch
}