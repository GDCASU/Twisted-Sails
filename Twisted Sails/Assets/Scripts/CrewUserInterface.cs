using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
// During the middle of battle, the player can press one of three buttons 
// (on the keyboard or a UI) to modify one of three stats for their ship: Speed of the ship, 
// Fire Rate/Attack of the weapons, and Durability/Defense of the ship. 
//
// Initial Author:  Erick Ramirez Cordero
// Initial Version: October 2, 2016
//
// Update:      Erick Ramirez Cordero
// Date:        November 9, 2016
// Description: Variables were changed to reflect the new multiplier stat system implemented
//              in the Crew Management Script. Furthermore, the text above the bars now
//              display the current stage of each stat rather than percentages.
*/

public class CrewUserInterface : MonoBehaviour
{
    // Limit Variables
    private float minStat = -4.0f;
    private float maxStat = 4.0f;
    
    public string attackButton = "Attack Crew"; // At the moment set to '1'
    public string defenseButton = "Defense Crew"; // At the moment set to '2'
    public string speedButton = "Speed Crew"; // At the moment set to '3'

    public GameObject boat;

    Slider attackBar;
    Slider defenseBar;
    Slider speedBar;

    Text attackText;
    Text defenseText;
    Text speedText;
    
    CrewManagement crewManagement;
    
	// Use this for initialization
	void Start ()
    {
        // Locate UI and script components
        attackBar = GameObject.Find("AttackBar").GetComponent<Slider>();
        defenseBar = GameObject.Find("DefenseBar").GetComponent<Slider>();
        speedBar = GameObject.Find("SpeedBar").GetComponent<Slider>();

        attackText = GameObject.Find("AttackText").GetComponent<Text>();
        defenseText = GameObject.Find("DefenseText").GetComponent<Text>();
        speedText = GameObject.Find("SpeedText").GetComponent<Text>();

        crewManagement = boat.GetComponent<CrewManagement>();

        // Initialize UI bar min / max values
        attackBar.maxValue = maxStat;
        attackBar.minValue = minStat;

        defenseBar.maxValue = maxStat;
        defenseBar.minValue = minStat;

        speedBar.maxValue = maxStat;
        speedBar.minValue = minStat;

        DisplayUpdate();
    }
	
    /*
    // The Update event checks for button inputs. When a stat buff is chosen, the
    // appropriate stat function from the CrewManagement script is called. After
    // stat calculations are done, DisplayUpdate() is called to update the UI.
    */

	// Update is called once per frame
	void Update ()
    {
        if (InputWrapper.GetButtonDown(attackButton))
        {
            //crewManagement.AttackCrew();
            DisplayUpdate();
        }

        else if (InputWrapper.GetButtonDown(defenseButton))
        {
            //crewManagement.DefenseCrew();
            DisplayUpdate();
        }

        else if (InputWrapper.GetButtonDown(speedButton))
        {
            //crewManagement.SpeedCrew();
            DisplayUpdate();
        }
    }

    // This function updates the UI bars and text
    private void DisplayUpdate()
    {
        /*
        // Since Attack & Defense stats in the Crew Management Script improve with a lower
        // multiplier, their displayed stages are multiplied by -1 to show the player a
        // positive stage
        */

        attackBar.value = crewManagement.currentFireRateStage * -1.0f;
        defenseBar.value = crewManagement.currentDefenseStage * -1.0f;
        speedBar.value = crewManagement.currentSpeedStage;

        attackText.text = "Stage: " + (int) attackBar.value;
        defenseText.text = "Stage: " + (int) defenseBar.value;
        speedText.text = "Stage: " + (int) speedBar.value;
    }
}
