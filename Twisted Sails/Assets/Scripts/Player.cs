using UnityEngine;
using UnityEngine.Networking;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Exported this class from the MultiplayerManager as it was getting too big.

// Developer:   Nizar Kury
// Date:        11/17/2016
// Description: Added  a ship instance variable for ship selection

// Developer:   Kyle Aycock
// Date:        1/20/2017
// Description: Changed to allow coders to directly modify the player object and have it
//              automatically update the game state in the server

// Container class for all info for a single player
public class Player
{
    #region Properties
    //there's probably a more elegant way of doing this
    public string name
    {
        get
        {
            return _name;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _name = value;
            NotifyServerStateChanged();
        }
    }
    public Team team
    {
        get
        {
            return _team;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _team = value;
            NotifyServerStateChanged();
        }
    }
    public Ship ship
    {
        get
        {
            return _ship;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _ship = value;
            NotifyServerStateChanged();
        }
    }
    public NetworkInstanceId objectId
    {
        get
        {
            return _objectId;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _objectId = value;
            NotifyServerStateChanged();
        }
    }
    public int connectionId
    {
        get
        {
            return _connectionId;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _connectionId = value;
            NotifyServerStateChanged();
        }
    }
    public bool ready
    {
        get
        {
            return _ready;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _ready = value;
            NotifyServerStateChanged();
        }
    }
    public short kills
    {
        get
        {
            return _kills;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _kills = value;
            NotifyServerStateChanged();
        }
    }
    public short deaths
    {
        get
        {
            return _deaths;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _deaths = value;
            NotifyServerStateChanged();
        }
    }
    public short killstreak
    {
        get
        {
            return _killstreak;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _killstreak = value;
            NotifyServerStateChanged();
        }
    }
    public short maxBounty
    {
        get
        {
            return _maxBounty;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _maxBounty = value;
            NotifyServerStateChanged();
        }
    }
    public short score
    {
        get
        {
            return _score;
        }
        set
        {
            if (!NetworkServer.active)
                return;
            _score = value;
            NotifyServerStateChanged();
        }
    }
    #endregion

    private string _name;
    private Team _team;
    private Ship _ship;
    private NetworkInstanceId _objectId;
    private int _connectionId;
    private bool _ready;
    private short _kills;
    private short _deaths;
    private short _killstreak;
    private short _maxBounty;
    private short _score;

    public Player() : this("???", Team.Spectator, NetworkInstanceId.Invalid, -1) { }

    public Player(string name, Team team, NetworkInstanceId objectId, int connectionId)
    {
        _name = name;
        _team = team;
        _objectId = objectId;
        _connectionId = connectionId;
        _ready = false;
        _kills = 0;
        _deaths = 0;
        _killstreak = 0;
        _maxBounty = 0;
        _score = 0;
    }

    //This method needs to write all of the player's information to the given NetworkWriter
    public void Serialize(NetworkWriter writer)
    {
        writer.Write(_name);
        writer.Write((byte)_team);
        writer.Write((byte)_ship);
        writer.Write(_objectId);
        writer.Write(_connectionId);
        writer.Write(_ready);
        writer.Write(_kills);
        writer.Write(_deaths);
        writer.Write(_killstreak);
        writer.Write(_maxBounty);
        writer.Write(_score);
    }

    //This method is called on a player object to fill it with information from a NetworkReader
    public void Deserialize(NetworkReader reader)
    {
        _name = reader.ReadString();
        _team = (Team)reader.ReadByte();
        _ship = (Ship)reader.ReadByte();
        _objectId = reader.ReadNetworkId();
        _connectionId = reader.ReadInt32();
        _ready = reader.ReadBoolean();
        _kills = reader.ReadInt16();
        _deaths = reader.ReadInt16();
        _killstreak = reader.ReadInt16();
        _maxBounty = reader.ReadInt16();
        _score = reader.ReadInt16();
    }

    /// <summary>
    /// Returns bounty based on killstreak
    /// </summary>
    /// <returns>Points of bounty</returns>
    public short GetBounty()
    {
        return (short)Mathf.Clamp(_killstreak, 2, 5);
    }

    public float GetShipHealth()
    {
        if (MultiplayerManager.IsLobby())
        {
            Debug.Log("ERROR: Ship health requested while still in lobby!");
            return -1;
        }
        return GetPlayerObject().GetComponent<Health>().health;
    }

    public GameObject GetPlayerObject()
    {
        if (_objectId == NetworkInstanceId.Invalid)
        {
            Debug.Log("GetPlayerObject() called on player with invalid object!");
            return null;
        }
        if (MultiplayerManager.IsServer())
        {
            return NetworkServer.FindLocalObject(_objectId);
        }
        else
        {
            return ClientScene.FindLocalObject(_objectId);
        }
    }

    /// <summary>
    /// Use this to add to a player's killstreak
    /// </summary>
    public void AddKill()
    {
        _killstreak++;
        if (GetBounty() > _maxBounty) maxBounty = GetBounty();
    }

    //tells server about a player variable changing
    private void NotifyServerStateChanged()
    {
        MultiplayerManager.GetInstance().SendGameState();
    }
}