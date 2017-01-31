using UnityEngine;
using System.Collections;

// Developer:   Kyle Aycock
// Date:        11/10/2016
// Description: Handles input for the title screen, most of which
//              is currently network related. All buttons hook to this

public class TitleScreenInput : MonoBehaviour
{
    private MultiplayerManager manager;

    void Start()
    {
        manager = MultiplayerManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartHost()
    {
        manager.StartHost();
    }

    public void StartClient()
    {
        manager.StartClient();
    }

    public void SetIP(string ip)
    {
        manager.networkAddress = ip;
    }

    public void SetName(string name)
    {
        manager.localPlayerName = name;
    }

    public void SetPort(string port)
    {
        manager.networkPort = int.Parse(port);
    }
}
