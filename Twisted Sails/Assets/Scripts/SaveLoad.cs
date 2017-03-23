
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;

// Developer:   Diego Wilde 
// Date:        I wrote this months ago, but now its 1/20/2017 
// Description: This class should enable player data persistence. Right now it should 
//              called when a player disconnects in order to get the player  
//              disconnecting and save its local stats, then restore it on reconnect. 

[System.Serializable]
public class Game
{
    public static Game current;
    public string name;
    public float timePlayed;
    public string IPaddress;
    public float kills;
    public float deaths;
}

public static class SaveLoad
{
    public static List<Game> savedGames = new List<Game>();

    public static void SaveGame()
    {
		int playerID = MultiplayerManager.GetLocalClient().connection.connectionId;
		Player player = MultiplayerManager.FindPlayer(playerID);

		if (player == null)
			return;

		Game.current = new Game();
        savedGames.Insert(0, Game.current);

		Game.current.name = player.name;
        Game.current.kills = player.kills;
        Game.current.deaths = player.deaths;

        BinaryFormatter bFormat = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/PlayerData.gd");

        bFormat.Serialize(file, Game.current);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerData.gd"))
        {

            BinaryFormatter bFormat = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerData.gd", FileMode.Open);
            Game data = (Game)bFormat.Deserialize(file);
            file.Close();
        }
    }
}