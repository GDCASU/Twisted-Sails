using UnityEngine;
using System.Collections;

/**
// This script handles the crew management stat allocation. When one stat is raised, the 
// other two stats are lowered as a form of balance. Additionally, there is a cooldown 
// timer in place before the player can select another stat modification.
//
// Initial Author: Erick Ramirez Cordero
// Initial Version: September 12, 2016
// Recent Author: Erick Ramirez Cordero
// Recent Version: October 2, 2016
//
// Recent Version Update: Button and UI input reading have been moved to the new 
// CrewUserInterface script. This script is responsible for stat calculations and the
// cooldown timer. The code that checked for stats remaining between the minimum and
// maximum is now located under the new CheckStatLimits() function. 
*/

public class CrewManagement : MonoBehaviour
{
    private float baseSpeed = 5.0f;
    private float baseFireRate = 5.0f;
    private float baseDefense = 5.0f;

    private float maxSpeed = 10.0f;
    private float maxFireRate = 10.0f;
    private float maxDefense = 10.0f;

    private float minSpeed = 1.0f;
    private float minFireRate = 1.0f;
    private float minDefense = 1.0f;

    private float modifySpeed = 0.5f;
    private float modifyFireRate = 0.5f;
    private float modifyDefense = 0.5f;
    private float increment = 2.0f;

    public float currentSpeed;
    public float currentFireRate;
    public float currentDefense;

    private float cooldown = 3.0f;
    public float cooldownTimer;

    private bool debugFlag = true;

    // CrewManagement needs to use Awake (as opposed to Start) in order for the stats
    // to be instantiated before the UI reads them in CrewUserInterface.Start()

	// Use this for initialization
	void Awake ()
    {
        currentSpeed = baseSpeed;
        currentFireRate = baseFireRate;
        currentDefense = baseDefense;
        cooldownTimer = 0;
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
    // the current speed has not exceeded the max speed.
    // If the checks are true, then the current speed is raised by its modification rate
    // times the increment rate, the other stats are lowered by their modification rates,
    // and the cooldown timer is reset.
    */

    public void SpeedCrew()
    {
        if (cooldownTimer >= cooldown && currentSpeed < maxSpeed)
        {
            currentSpeed += modifySpeed * increment;
            currentFireRate -= modifyFireRate;
            currentDefense -= modifyDefense;
            cooldownTimer = 0;
            CheckStatLimits();

            if (debugFlag)
            {
                Debug.Log("Speed has increased to: " + currentSpeed);
                Debug.Log("Attack has decreased to: " + currentFireRate);
                Debug.Log("Defense has decreased to: " + currentDefense);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    /*
    // When AttackCrew is called, the script checks if the cooldown timer has ended and
    // the current attack has not exceeded the max attack.
    // If the checks are true, then the current fire rate is raised by its modification
    // rate times the increment rate, the other stats are lowered by their modification
    // rates, and the cooldown timer is reset.
    */

    public void AttackCrew()
    {
        if (cooldownTimer >= cooldown && currentFireRate < maxFireRate)
        {
            currentSpeed -= modifySpeed;
            currentFireRate += modifyFireRate * increment;
            currentDefense -= modifyDefense;
            cooldownTimer = 0;
            CheckStatLimits();

            if (debugFlag)
            {
                Debug.Log("Speed has decreased to: " + currentSpeed);
                Debug.Log("Attack has increased to: " + currentFireRate);
                Debug.Log("Defense has decreased to: " + currentDefense);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    /*
    // When DefenseCrew is called, the script checks if the cooldown timer has ended and
    // the current defense has not exceeded the max defense.
    // If the checks are true, then the current defense is raised by its modification rate
    // times the increment rate, the other stats are lowered by their modification rates,
    // and the cooldown timer is reset.
    */

    public void DefenseCrew()
    {
        if (cooldownTimer >= cooldown && currentDefense < maxDefense)
        {
            currentSpeed -= modifySpeed;
            currentFireRate -= modifyFireRate;
            currentDefense += modifyDefense * increment;
            cooldownTimer = 0;
            CheckStatLimits();

            if (debugFlag)
            {
                Debug.Log("Speed has decreased to: " + currentSpeed);
                Debug.Log("Attack has decreased to: " + currentFireRate);
                Debug.Log("Defense has increased to: " + currentDefense);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    // This function checks to see if the stats have not exceeded their maximum or
    // dropped below their minimum.
    void CheckStatLimits()
    {
        if (currentSpeed > maxSpeed) { currentSpeed = maxSpeed; }
        if (currentSpeed < minSpeed) { currentSpeed = minSpeed; }

        if (currentFireRate > maxFireRate) { currentFireRate = maxFireRate; }
        if (currentFireRate < minFireRate) { currentFireRate = minFireRate; }

        if (currentDefense > maxDefense) { currentDefense = maxDefense; }
        if (currentDefense < minDefense) { currentDefense = minDefense; }
    }
}