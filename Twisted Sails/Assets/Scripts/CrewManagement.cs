using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

/**
// This script handles the crew management stat allocation system. When one stat is raised, the 
// other two stats are lowered as a form of balance. Additionally, there is a cooldown 
// timer in place before the player can select another stat modification. This script also updates
// the Crew Management User Interface for the player to visually see the state of their ship.

// Initial Author:  Erick Ramirez Cordero
// Initial Version: September 12, 2016

// Update:      Erick Ramirez Cordero
// Date:        October 2, 2016
// Description: Button and UI input reading have been moved to the new CrewUserInterface script.
//              This script is responsible for stat calculations and the cooldown timer. The code
//              that checked for stats remaining between the minimum and maximum is now located
//              under the new CheckStatLimits() function.

// Update:      Erick Ramirez Cordero
// Date:        November 9, 2016
// Description: First attempt at connecting the Crew Management feature to the boat. The three
//              stats are now multipliers that have four positive and four negative stages.
//              When the player allocates members to a crew, the stat chosen is augmented by
//              two stages while the other two stats fall by one stage. The CheckStatsLimits()
//              function has been replaced by the CheckStage() and StatUpdate() functions.

// NOTE (UI):   This script now handles calculations AND the User Interface again.
//              Due to this, this script is now networked and the CrewManagementUserInterface 
//              script should NOT be used.

// NOTE:        Due to how stats are calculated, speed improves when its stage rises (larger
//              multiplier) while fire rate and defense improve when their stages fall (smaller
//              multipliers).

// Speed:       Influences the Acceleration of the ship. The higher the currentSpeed, the faster
//              the ship should move.
//              The BoatMovementNetworked script uses currentSpeed.

// Fire Rate:   Influences the Fire Delay of the ship's cannons. The lower the currentFireRate,
//              the faster the cannons should be able to fire.
//              The BroadsideCannonFireNetworked script uses currentFireRate.

// Defense:     Influences damage calculation. The lower the currentDefense, the less damage a
//              ship takes from the enemy.
//              The Health script uses currentDefense.
*/

public class CrewManagement : NetworkBehaviour
{
    // Current Stat Variables
    public float currentSpeed;
    public float currentFireRate;
    public float currentDefense;

    public int currentSpeedStage;
    public int currentFireRateStage;
    public int currentDefenseStage;

    // Stage Variables
    private const int stageMin = -4;
    private const int stageMax = 4;

    private const float stageMinusFour = 0.33f;
    private const float stageMinusThree = 0.5f;
    private const float stageMinusTwo = 0.66f;
    private const float stageMinusOne = 0.75f;
    private const float stageZero = 1.0f;
    private const float stagePlusOne = 1.25f;
    private const float stagePlusTwo = 1.5f;
    private const float stagePlusThree = 1.75f;
    private const float stagePlusFour = 2.0f;

    // Timer variables
    private float cooldown = 3.0f;
    private float cooldownTimer;

    // User Interface Variables
    public string attackButton = "Attack Crew"; // At the moment set to '1'
    public string defenseButton = "Defense Crew"; // At the moment set to '2'
    public string speedButton = "Speed Crew"; // At the moment set to '3'

    private Slider attackBar;
    private Slider defenseBar;
    private Slider speedBar;

    Text attackText;
    Text defenseText;
    Text speedText;

    // Scripts influenced by Crew Management
    private Health healthScript;
    private BoatMovementNetworked boatScript;

    // Set to the number of cannons on the ship (Currently 8)
    private const int cannonCount = 8;
    private BroadsideCannonFireNetworked[] fireScripts = new BroadsideCannonFireNetworked[cannonCount];

    private bool debugFlag = true;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            currentSpeedStage = 0;
            currentFireRateStage = 0;
            currentDefenseStage = 0;
            cooldownTimer = 0;

            healthScript = GetComponentInChildren<Health>();
            boatScript = GetComponentInChildren<BoatMovementNetworked>();
            fireScripts = GetComponentsInChildren<BroadsideCannonFireNetworked>();

            // Locate & initialize UI and script components
            Transform crewUI = GameObject.FindGameObjectWithTag("CrewManagementUI").transform;
            Transform attackUI = crewUI.FindChild("AttackCrewUI");
            Transform defenseUI = crewUI.FindChild("DefenseCrewUI");
            Transform speedUI = crewUI.FindChild("SpeedCrewUI");

            attackBar = attackUI.GetComponentInChildren<Slider>();
            defenseBar = defenseUI.GetComponentInChildren<Slider>();
            speedBar = speedUI.GetComponentInChildren<Slider>();

            attackText = attackUI.GetComponentInChildren<Text>();
            defenseText = defenseUI.GetComponentInChildren<Text>();
            speedText = speedUI.GetComponentInChildren<Text>();

            // Initialize UI bar min / max values
            attackBar.maxValue = stageMax;
            attackBar.minValue = stageMin;

            defenseBar.maxValue = stageMax;
            defenseBar.minValue = stageMin;

            speedBar.maxValue = stageMax;
            speedBar.minValue = stageMin;

            DisplayUpdate();
        }
    }

    /*
    // In the Update event, the script checks if the cooldown timer has reached its
    // end. If not, the cooldown timer is incremented by Time.deltaTime. Afterwards,
    // the script checks for player input and calls a crew function if necessary.
    */

    // Update is called once per frame
	void Update ()
    {
        if (!isLocalPlayer)
        { return; }

        if (cooldownTimer < cooldown)
        { cooldownTimer += Time.deltaTime; }

        if (InputWrapper.GetButtonDown(attackButton))
        {
            AttackCrew();
            DisplayUpdate();
        }

        else if (InputWrapper.GetButtonDown(defenseButton))
        {
            DefenseCrew();
            DisplayUpdate();
        }

        else if (InputWrapper.GetButtonDown(speedButton))
        {
            SpeedCrew();
            DisplayUpdate();
        }
    }

    /*
    // When SpeedCrew is called, the script checks if the cooldown timer has ended and
    // the current speed stage has not exceeded the maximum stage.
    // If the checks are true, then the current speed is raised by two stages, the
    // other stats are raised by one stage, and the cooldown timer is reset.
    */

    public void SpeedCrew()
    {
        if (cooldownTimer >= cooldown && currentSpeedStage < stageMax)
        {
            currentSpeedStage += 2;
            currentFireRateStage++;
            currentDefenseStage++;
            cooldownTimer = 0;

            StatUpdate();

            if (debugFlag)
            {
                Debug.Log("Speed Stage: " + currentSpeedStage);
                Debug.Log("Fire Rate Stage: " + currentFireRateStage);
                Debug.Log("Defense Stage: " + currentDefenseStage);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    /*
    // When AttackCrew is called, the script checks if the cooldown timer has ended and
    // the current fire rate stage has not exceeded the minimum stage.
    // If the checks are true, then the current fire rate is lowered by two stages,
    // current speed is lowered by one stage, current damage taken is raised by
    // one stage, and the cooldown timer is reset.
    */

    public void AttackCrew()
    {
        if (cooldownTimer >= cooldown && currentFireRateStage > stageMin)
        {
            currentSpeedStage--;
            currentFireRateStage -= 2;
            currentDefenseStage++;
            cooldownTimer = 0;

            StatUpdate();

            if (debugFlag)
            {
                Debug.Log("Speed Stage: " + currentSpeedStage);
                Debug.Log("Fire Rate Stage: " + currentFireRateStage);
                Debug.Log("Defense Stage: " + currentDefenseStage);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    /*
    // When DefenseCrew is called, the script checks if the cooldown timer has ended and
    // the current defense stage has not exceeded the minimum stage.
    // If the checks are true, then current defense is lowered by two stages, current
    // speed is lowered by a stage, and current fire rate is raised by a stage.
    */

    public void DefenseCrew()
    {
        if (cooldownTimer >= cooldown && currentDefenseStage > stageMin)
        {
            currentSpeedStage--;
            currentFireRateStage++;
            currentDefenseStage -= 2;
            cooldownTimer = 0;

            StatUpdate();

            if(debugFlag)
            {
                Debug.Log("Speed Stage: " + currentSpeedStage);
                Debug.Log("Fire Rate Stage: " + currentFireRateStage);
                Debug.Log("Defense Stage: " + currentDefenseStage);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    /*
    // CheckStage first checks to make sure the stage has not exceeded the max or fallen
    // under the min. Due to this check, the stage is passed by reference in case a change
    // is made. Afterwards, the switch case checks the stage for the correct multiplier
    // and should return that multiplier to the stat that is related to the stage given.
    */

    public float CheckStage(ref int stage)
    {
        float multiplier = stageZero;

        if (stage > stageMax) { stage = stageMax; }
        if (stage < stageMin) { stage = stageMin; }

        switch (stage)
        {
            case -4:
                multiplier = stageMinusFour;
                break;
            case -3:
                multiplier = stageMinusThree;
                break;
            case -2:
                multiplier = stageMinusTwo;
                break;
            case -1:
                multiplier = stageMinusOne;
                break;
            case 0:
                multiplier = stageZero;
                break;
            case 1:
                multiplier = stagePlusOne;
                break;
            case 2:
                multiplier = stagePlusTwo;
                break;
            case 3:
                multiplier = stagePlusThree;
                break;
            case 4:
                multiplier = stagePlusFour;
                break;
        }

        return multiplier;
    }

    // StatUpdate will update the stats of other scripts to match the "current"
    // stats in the Crew Management Script
    private void StatUpdate()
    {
        currentSpeed = CheckStage(ref currentSpeedStage);
        currentFireRate = CheckStage(ref currentFireRateStage);
        currentDefense = CheckStage(ref currentDefenseStage);

        boatScript.speedStat = currentSpeed;
        healthScript.defenseStat = currentDefense;

        for (int i = 0; i < cannonCount; i++)
        { fireScripts[i].attackStat = currentFireRate; }

        if (debugFlag)
        {
            Debug.Log("Current Fire Rate: " + currentFireRate);
            Debug.Log("Current Defense Rate: " + currentDefense);
            Debug.Log("Current Speed: " + currentSpeed);
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

        attackBar.value = currentFireRateStage * -1.0f;
        defenseBar.value = currentDefenseStage * -1.0f;
        speedBar.value = currentSpeedStage;

        attackText.text = "Stage: " + (int)attackBar.value;
        defenseText.text = "Stage: " + (int)defenseBar.value;
        speedText.text = "Stage: " + (int)speedBar.value;
    }
}