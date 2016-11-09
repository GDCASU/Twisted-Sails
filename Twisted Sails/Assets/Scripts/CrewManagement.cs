using UnityEngine;
using System.Collections;

/**
// This script handles the crew management stat allocation system. When one stat is raised, the 
// other two stats are lowered as a form of balance. Additionally, there is a cooldown 
// timer in place before the player can select another stat modification.
//
// Initial Author:  Erick Ramirez Cordero
// Initial Version: September 12, 2016
//
// Update:      Erick Ramirez Cordero
// Date:        October 2, 2016
// Description: Button and UI input reading have been moved to the new CrewUserInterface script.
//              This script is responsible for stat calculations and the cooldown timer. The code
//              that checked for stats remaining between the minimum and maximum is now located
//              under the new CheckStatLimits() function.
//
// Update:      Erick Ramirez Cordero
// Date:        November 9, 2016
// Description: First attempt at connecting the Crew Management feature to the boat. The three
//              stats are now multipliers that have four positive and four negative stages.
//              When the player allocates members to a crew, the stat chosen is augmented by
//              two stages while the other two stats fall by one stage. The CheckStatsLimits()
//              function has been replaced by the CheckStage() and StatUpdate() functions.
//
// NOTE:        Due to how stats are calculated, speed improves when its stage rises (larger
//              multiplier) while fire rate and defense improve when their stages fall (smaller
//              multipliers).
//
// Speed:       Influences the Acceleration of the ship. The higher the currentSpeed, the faster
//              the ship should move.
//              The BoatMovementNetworked script uses currentSpeed.
//
// Fire Rate:   Influences the Fire Delay of the ship's cannons. The lower the currentFireRate,
//              the faster the cannons should be able to fire.
//              The BroadsideCannonFireNetworked script uses currentFireRate.
//
// Damage:      Influences damage calculation. The lower the currentDamage, the less damage a
//              ship takes from the enemy.
//              The Health script uses currentDefense.
*/

public class CrewManagement : MonoBehaviour
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

    private const float stageMinusFour = 0.25f;
    private const float stageMinusThree = 0.33f;
    private const float stageMinusTwo = 0.5f;
    private const float stageMinusOne = 0.66f;
    private const float stageZero = 1.0f;
    private const float stagePlusOne = 1.5f;
    private const float stagePlusTwo = 2.0f;
    private const float stagePlusThree = 2.5f;
    private const float stagePlusFour = 3.0f;

    // Timer variables
    private float cooldown = 3.0f;
    public float cooldownTimer;

    private bool debugFlag = true;

    // Scripts influenced by Crew Management
    private Health healthScript;
    private BoatMovementNetworked boatScript;
    private BroadsideCannonFireNetworked[] fireScripts = new BroadsideCannonFireNetworked[8];

    // CrewManagement needs to use Awake (as opposed to Start) in order for the stats
    // to be instantiated before the UI reads them in CrewUserInterface.Start()

	// Use this for initialization - Variables
	void Awake ()
    {
        currentSpeedStage = 0;
        currentFireRateStage = 0;
        currentDefenseStage = 0;
        cooldownTimer = 0;
	}

    // Use this for initialization - Scripts
    void Start()
    {
        healthScript = this.GetComponentInChildren<Health>();
        boatScript = this.GetComponentInChildren<BoatMovementNetworked>();
        fireScripts = this.GetComponentsInChildren<BroadsideCannonFireNetworked>();
    }

    // In the Update event, the script checks if the cooldown timer has reached its
    // end. If not, the cooldown timer is incremented by Time.deltaTime.
    	
    // Update is called once per frame
	void Update ()
    {
        if (cooldownTimer < cooldown)
        { cooldownTimer += Time.deltaTime; }    
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
    // This function is called when a current stat needs to be updated. The stage of the stat
    // is checked first to see whether or not it has exceeded the maximum stage or fallen
    // below the minimum stage. A switch case for the stage provided then sets the multiplier
    // variable to the appropriate constant. When the multiplier is returned, the current stat
    // related to the stage used as the argument is set to that value.
    */

    public float CheckStage(int stage)
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
    // stats in the CrewManagement Script
    private void StatUpdate()
    {
        currentSpeed = CheckStage(currentSpeedStage);
        currentFireRate = CheckStage(currentFireRateStage);
        currentDefense = CheckStage(currentDefenseStage);

        boatScript.speedStat = currentSpeed;
        healthScript.defenseStat = currentDefense;

        for (int i = 0; i < 8; i++)
        { fireScripts[i].attackStat = currentFireRate; }

        if (debugFlag)
        {
            Debug.Log("Current Fire Rate: " + currentFireRate);
            Debug.Log("Current Defense Rate: " + currentDefense);
            Debug.Log("Current Speed: " + currentSpeed);
        }
    }
}