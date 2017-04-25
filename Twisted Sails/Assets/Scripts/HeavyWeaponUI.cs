using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

//Programmer:Pablo Camacho
//Date: 02/22/17
//Update: 03/02/17


/*
 * Attach this script to player ship.
 * Make sure the player ship this script is attached to contains a heavy weapon script as a component!
 * For example, add this script in TriremeShipPlayer which should have HeavyWeaponSunShip.cs as of 03/02/17.
 * Use UIDumbHeavyWeapon.cs if you want to do testing with UI.
*/
public class HeavyWeaponUI : NetworkBehaviour
{
    //variable reference to text item that displays amount of ammo in game 
    public Text ammoCountValueText;

    //variable reference to text item that displays max amount of ammo in game
    public Text ammoMaxValueText;

    //variable reference to text item that displays cool down timer in game
    public Text coolDownTimerText;

    //variable reference to HeavyWeapon item that player uses
    public HeavyWeapon playerHeavyWeapon;

    // Use this for initialization
    void Start()
    {

        //Setting up Ammo and Cooldown Timer Text UI
        if (isLocalPlayer) //This is the local player's ship -- use the HUD heavyweapon UI elements
        {
            GameObject UI;
            //locate text for current ammo in scene
            UI = GameObject.FindGameObjectWithTag("CurrentAmmoUI");
            ammoCountValueText = UI.GetComponentInChildren<Text>();
            //locate text for max ammo in scene
            UI = GameObject.FindGameObjectWithTag("MaxAmmoUI");
            ammoMaxValueText = UI.GetComponentInChildren<Text>();
            //locate text for cool down in scene
            UI = GameObject.FindGameObjectWithTag("CooldownUI");
            coolDownTimerText = UI.GetComponentInChildren<Text>();

            //Get reference to HeavyWeapon Component the player ship is using
            playerHeavyWeapon = GetComponent<HeavyWeapon>();

            //initialize text UI elements with infor from playerHeavyWeapon
            SetAmmoCountValueText();
            SetAmmoMaxValueText();
            SetCoolDownTimerText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Kyle Aycock 4/6/2017 - prevent this code from being run on other clients
        if (!isLocalPlayer) return;

        //Update ammo count text item with ammo count value in playerHeavyWeapon 
        SetAmmoCountValueText();
        //Update ammo max value text item with max ammo count value in playerHeavyWeapon 
        SetAmmoMaxValueText();
        //Update cool down timer text item with cooldown timer value in playerHeavyWeapon 
        SetCoolDownTimerText();
    }

    //function to set ammoCountText to ammo count from playerHeavyWeapon
    void SetAmmoCountValueText()
    {
        ammoCountValueText.text = playerHeavyWeapon.AmmoCount.ToString();
    }

    //function to set ammoMaxText to ammo max value from playerHeavyWeapon
    void SetAmmoMaxValueText()
    {
        ammoMaxValueText.text = playerHeavyWeapon.ammoCapacity.ToString();
    }

    //function to set cool down timer count to cool down timer value from playreHeavyWeapon
    void SetCoolDownTimerText()
    {
        coolDownTimerText.text = (playerHeavyWeapon.CoolDownTimer > 0 ? playerHeavyWeapon.CoolDownTimer.ToString("0.#") : "0.0");
    }
}
