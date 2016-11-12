using UnityEngine;
using UnityEngine.Networking;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Exported this class from the MultiplayerManager as it was getting too big.

/// <summary>
/// Container class for all info for a single player
/// </summary>
public class Player
{
    public string name;
    public Team team;
    public NetworkInstanceId objectId;
    public int connectionId;
    public bool ready;
    public short kills;
    public short deaths;
    public short killstreak;
    public short maxBounty;
    public short score;

    public Player() { }

    public Player(string name, Team team, NetworkInstanceId objectId, int connectionId)
    {
        this.name = name;
        this.team = team;
        this.objectId = objectId;
        this.connectionId = connectionId;
        ready = false;
        kills = 0;
        deaths = 0;
        killstreak = 0;
        maxBounty = 0;
        score = 0;
    }

    //This method needs to write all of the player's information to the given NetworkWriter
    public void Serialize(NetworkWriter writer)
    {
        writer.Write(name);
        writer.Write((byte)team);
        writer.Write(objectId);
        writer.Write(connectionId);
        writer.Write(ready);
        writer.Write(kills);
        writer.Write(deaths);
        writer.Write(killstreak);
        writer.Write(maxBounty);
        writer.Write(score);
    }

    //This method is called on a player object to fill it with information from a NetworkReader
    public void Deserialize(NetworkReader reader)
    {
        name = reader.ReadString();
        team = (Team)reader.ReadByte();
        objectId = reader.ReadNetworkId();
        connectionId = reader.ReadInt32();
        ready = reader.ReadBoolean();
        kills = reader.ReadInt16();
        deaths = reader.ReadInt16();
        killstreak = reader.ReadInt16();
        maxBounty = reader.ReadInt16();
        score = reader.ReadInt16();
    }

    /// <summary>
    /// Returns bounty based on killstreak
    /// </summary>
    /// <returns>Points of bounty</returns>
    public short GetBounty()
    {
        return (short)Mathf.Clamp(killstreak, 2, 5);
    }

    /// <summary>
    /// Use this to add to a player's killstreak
    /// </summary>
    public void AddKill()
    {
        killstreak++;
        if (GetBounty() > maxBounty) maxBounty = GetBounty();
    }
}