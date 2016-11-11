using UnityEngine;
using System.Collections;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Handles input for the title screen, most of which
//              is currently network related. All buttons hook to this

public class TitleScreenInput : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
	}

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartHost()
    {
        MultiplayerManager.instance.StartHost();
    }

    public void StartClient()
    {
        MultiplayerManager.instance.StartClient();
    }

    public void SetIP(string ip)
    {
        MultiplayerManager.instance.networkAddress = ip;
    }

    public void SetName(string name)
    {
        MultiplayerManager.instance.localPlayerName = name;
    }

    public void SetPort(string port)
    {
        MultiplayerManager.instance.networkPort = int.Parse(port);
    }
}
