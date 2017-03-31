using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

// Developer:   Kyle Aycock
// Date:        3/24/2017
// Description: Some fixes to saving and loading and filling contents of text fields with loaded data

public class TitleScreenInput : MonoBehaviour
{
    public InputField HostNameField;
    public InputField JoinNameField;
    public InputField IPField;

    //DW
    //
    void Awake()
    {
        MultiplayerManager mm = MultiplayerManager.GetInstance();
        if (mm != null)
        {
            HostNameField.text = mm.localPlayerName;
            JoinNameField.text = mm.localPlayerName;
            IPField.text = mm.networkAddress;
        }
        Game.current = new Game();
        SaveLoad.Load();
        IPField.text = Game.current.IPaddress;
        JoinNameField.text = Game.current.name;
        HostNameField.text = Game.current.name;
    }

    // Allows quitting by pressing ESC.
    void Update()
    {
        if (InputWrapper.GetKeyDown(KeyCode.Escape))
            QuitGame();

        //The following code was taken from http://answers.unity3d.com/questions/784526/46-ugui-select-next-inputfield-with-entertab.html
        //and exists solely to allow tabbing between input fields on the title screen. Whew!
        if (InputWrapper.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;
            Selectable current = null;
            EventSystem eventSystem = EventSystem.current;

            // Figure out if we have a valid current selected gameobject
            if (eventSystem.currentSelectedGameObject != null)
            {
                // Unity doesn't seem to "deselect" an object that is made inactive
                if (eventSystem.currentSelectedGameObject.activeInHierarchy)
                {
                    current = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
                }
            }

            if (current != null)
            {
                // When SHIFT is held along with tab, go backwards instead of forwards
                if (InputWrapper.GetKey(KeyCode.LeftShift) || InputWrapper.GetKey(KeyCode.RightShift))
                {
                    next = current.FindSelectableOnLeft();
                    if (next == null)
                    {
                        next = current.FindSelectableOnUp();
                    }
                }
                else {
                    next = current.FindSelectableOnRight();
                    if (next == null)
                    {
                        next = current.FindSelectableOnDown();
                    }
                }
            }
            else {
                // If there is no current selected gameobject, select the first one
                if (Selectable.allSelectables.Count > 0)
                {
                    next = Selectable.allSelectables[0];
                }
            }

            if (next != null)
            {
                next.Select();
            }
        }
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
        MultiplayerManager.GetInstance().localPlayerName = (name.Equals("") ? "???" : name);
        Game.current.name = name;
        SaveLoad.SaveGame();
    }

    //Called when the port field is changed
    public void SetPort(string port)
    {
        MultiplayerManager.GetInstance().networkPort = int.Parse(port);
    }
}
