using UnityEngine;
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

public class SaveLoader {
    public static List<Player> playerDisconnectList = new List<Player>();

    public static void SavePlayerStats(Player player)
    {
        playerDisconnectList.Add(player);
        BinaryFormatter bFormat = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath 
            + "/savedPlayers.gd");
        bFormat.Serialize(file, SaveLoader.playerDisconnectList);
        file.Close();

    }
    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedPlayers/gd"))
        {
            BinaryFormatter bFormat = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath 
                + "/savedPlayers.gd", FileMode.Open);
            SaveLoader.playerDisconnectList = (List<Player>)bFormat.Deserialize(file);
            file.Close();
        }
    }

}
