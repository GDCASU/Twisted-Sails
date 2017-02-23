using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI.Text UI

//Programmer:Pablo Camacho
//Date: 02/22/17

//Duplicated from HeavyWeaponTemplate

public class HeavyWeaponUI : MonoBehaviour 
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
	void Start () 
	{
		SetAmmoCountValueText();
		SetAmmoMaxValueText();
		SetCoolDownTimerText();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Update ammo count text item with ammo count value in Heavy Weapon 
		SetAmmoCountValueText();
		SetAmmoMaxValueText();
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
		coolDownTimerText.text = playerHeavyWeapon.CoolDownTimer.ToString();
	}
}
