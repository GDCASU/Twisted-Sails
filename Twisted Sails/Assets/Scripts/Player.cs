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

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Adapted to use new team system
//              Added several events for use by other coders

// Class representing a player in game
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
    public short team
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
    private short _team;
    private Ship _ship;
    private NetworkInstanceId _objectId;
    private int _connectionId;
    private bool _ready;
    private short _kills;
    private short _deaths;
    private short _killstreak;
    private short _maxBounty;
    private short _score;

    #region Events

    //Player Killed event - happens when a player is killed for any reason, 'killer' may be null
    //This event only works serverside until I get a chance to rewrite the damage handling in the Health script
    public delegate void PlayerKilledEvent(Player victim, Player killer); //it yelled at me for making these private so they're public
    public static event PlayerKilledEvent PlayerKilled = delegate { };

    //Player Damaged event - happens when a player takes damage from an attacker
    //Damage can only be changed clientside for now until I get a chance to rewrite damage handling
    public delegate void PlayerDamagedEvent(Player victim, Player attacker, ref float damage);
    public static event PlayerDamagedEvent PlayerDamaged = delegate { };

    //Player Pickup event - happens when a player gets a pickup
    public delegate void PlayerPickupEvent(Player player, bool isHealthPack);
    public static event PlayerPickupEvent PlayerGotPickup = delegate { };

    //These methods allow classes other than the Player to trigger the events, as this is not normally possible
    //It was either this or put the events in the Health script instead, which didn't make sense
    public static void ActivateEventPlayerKilled(Player victim, Player killer)
    {
        PlayerKilled(victim, killer);
    }
    public static void ActivateEventPlayerDamaged(Player victim, Player attacker, ref float damage)
    {
        PlayerDamaged(victim, attacker, ref damage);
    }
    public static void ActivateEventPlayerPickup(Player player, bool isHealthPack)
    {
        PlayerGotPickup(player, isHealthPack);
    }

    #endregion

    public Player() : this("???", -1, NetworkInstanceId.Invalid, -1) { }

    public Player(string name, short team, NetworkInstanceId objectId, int connectionId)
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
        _team = reader.ReadByte();
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

    /// <summary>
    /// Returns this player's ship's health, or -1 if no ship found.
    /// </summary>
    /// <returns>Ship's current health or -1 if no ship was found.</returns>
    public float GetShipHealth()
    {
        if (MultiplayerManager.IsLobby() || GetPlayerObject() == null)
        {
            Debug.Log("ERROR: Ship health requested but no ship was found!");
            return -1;
        }
        return GetPlayerObject().GetComponent<Health>().health;
    }

    /// <summary>
    /// Returns the current GameObject assigned as the player controller.
    /// </summary>
    /// <returns>GameObject serving as player controller.</returns>
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
    /// Adds one kill to the player's killstreak.
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