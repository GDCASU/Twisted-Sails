using UnityEngine;
using System.Collections;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Handles input for the title screen, most of which
//              is currently network related. All buttons hook to this

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Fixed strange behavior when joining/hosting a server for the second time in the same session
//              Added more documentation

// Developer:   Diego Wilde
// Date:        2/19/2017
// Description: Added Awake override to load game data, if it exists

public class TitleScreenInput : MonoBehaviour
{

    //DW
    //
    void Awake()
    {
        SaveLoad.Load();
    }

    // Allows quitting by pressing ESC.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }

    // This is hooked to the Quit button
    public void QuitGame()
    {
        Application.Quit();
    }

    //This is hooked to the Host Server button
    public void StartHost()
    {
        MultiplayerManager.GetInstance().StartHost();
    }

    //This is hooked to the Join Server button
    public void StartClient()
    {
        MultiplayerManager.GetInstance().StartClient();
    }

    //This is called when changes to the Server IP field are submitted
    public void SetIP(string ip)
    {
        MultiplayerManager.GetInstance().networkAddress = ip;
        Game.current.IPaddress = ip;
        SaveLoad.SaveGame();
    }

    //Called when the name field is changed
    public void SetName(string name)
    {
        MultiplayerManager.GetInstance().localPlayerName = name;
    }

    //Called when the port field is changed
    public void SetPort(string port)
    {
        MultiplayerManager.GetInstance().networkPort = int.Parse(port);
    }
}
