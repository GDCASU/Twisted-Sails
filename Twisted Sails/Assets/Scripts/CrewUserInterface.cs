using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
// During the middle of battle, the player can press one of three buttons 
// (on the keyboard or a UI) to modify one of three stats for their ship: Speed of the ship, 
// Fire Rate/Attack of the weapons, and Durability/Defense of the ship. 
//
// Initial Author: Erick Ramirez Cordero
// Initial Version: October 2, 2016
// Recent Author:
// Recent Version:
//
// Recent Version Update:
*/

public class CrewUserInterface : MonoBehaviour
{
    // The min and max of each stat should match those in the CrewManagement Script
    private float attackMax = 10.0f;
    private float attackMin = 0.0f;
    private float defenseMax = 10.0f;
    private float defenseMin = 0.0f;
    private float speedMax = 10.0f;
    private float speedMin = 0.0f;

    public string speedButton = "Speed Crew"; // At the moment set to 'C'
    public string attackButton = "Attack Crew"; // At the moment set to 'Z'
    public string defenseButton = "Defense Crew"; // At the moment set to 'X'

    Slider attackBar;
    Slider defenseBar;
    Slider speedBar;

    Text attackText;
    Text defenseText;
    Text speedText;

    GameObject boat;
    CrewManagement crewManagement;
    
	// Use this for initialization
	void Start ()
    {
        attackBar = GameObject.Find("AttackBar").GetComponent<Slider>();
        defenseBar = GameObject.Find("DefenseBar").GetComponent<Slider>();
        speedBar = GameObject.Find("SpeedBar").GetComponent<Slider>();

        attackText = GameObject.Find("AttackText").GetComponent<Text>();
        defenseText = GameObject.Find("DefenseText").GetComponent<Text>();
        speedText = GameObject.Find("SpeedText").GetComponent<Text>();

        boat = GameObject.Find("Boat");
        crewManagement = boat.GetComponent<CrewManagement>();

        attackBar.maxValue = attackMax;
        attackBar.minValue = attackMin;
        defenseBar.maxValue = defenseMax;
        defenseBar.minValue = defenseMin;
        speedBar.maxValue = speedMax;
        speedBar.minValue = speedMin;

        attackBar.value = crewManagement.currentFireRate;
        defenseBar.value = crewManagement.currentDefense;
        speedBar.value = crewManagement.currentSpeed;

        attackText.text = "" + crewManagement.currentFireRate / attackMax * 100 + "%";
        defenseText.text = "" + crewManagement.currentDefense / defenseMax * 100 + "%";
        speedText.text = "" + crewManagement.currentSpeed / speedMax * 100 + "%";
    }
	
    // The Update event checks for button inputs.

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown(speedButton))
        { CallSpeedCrew(); }

        else if (Input.GetButtonDown(attackButton))
        { CallAttackCrew(); }

        else if (Input.GetButtonDown(defenseButton))
        { CallDefenseCrew(); }
    }

    /*
    // When CallAttackCrew() is called, AttackCrew() is called in the CrewManagement
    // Script. After stats are calculated, the bars and text displaying the stats to
    // the player are updated.
    */

    public void CallAttackCrew()
    {
        crewManagement.AttackCrew();

        attackBar.value = crewManagement.currentFireRate;
        defenseBar.value = crewManagement.currentDefense;
        speedBar.value = crewManagement.currentSpeed;

        attackText.text = "" + crewManagement.currentFireRate / attackMax * 100 + "%";
        defenseText.text = "" + crewManagement.currentDefense / defenseMax * 100 + "%";
        speedText.text = "" + crewManagement.currentSpeed / speedMax * 100 + "%";
    }

    /*
    // When CallDefenseCrew() is called, DefenseCrew() is called in the CrewManagement
    // Script. After stats are calculated, the bars and text displaying the stats to
    // the player are updated.
    */

    public void CallDefenseCrew()
    {
        crewManagement.DefenseCrew();

        attackBar.value = crewManagement.currentFireRate;
        defenseBar.value = crewManagement.currentDefense;
        speedBar.value = crewManagement.currentSpeed;

        attackText.text = "" + crewManagement.currentFireRate / attackMax * 100 + "%";
        defenseText.text = "" + crewManagement.currentDefense / defenseMax * 100 + "%";
        speedText.text = "" + crewManagement.currentSpeed / speedMax * 100 + "%";
    }

    /*
    // When CallSpeedCrew() is called, SpeedCrew() is called in the CrewManagement
    // Script. After stats are calculated, the bars and text displaying the stats to
    // the player are updated.
    */

    public void CallSpeedCrew()
    {
        crewManagement.SpeedCrew();

        attackBar.value = crewManagement.currentFireRate;
        defenseBar.value = crewManagement.currentDefense;
        speedBar.value = crewManagement.currentSpeed;

        attackText.text = "" + crewManagement.currentFireRate / attackMax * 100 + "%";
        defenseText.text = "" + crewManagement.currentDefense / defenseMax * 100 + "%";
        speedText.text = "" + crewManagement.currentSpeed / speedMax * 100 + "%";
    }
}
