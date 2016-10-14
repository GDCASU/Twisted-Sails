using UnityEngine;
using System.Collections;

/**
// This script handles the crew management stat allocation. During the middle of battle,
// the player can press one of three buttons (on the keyboard or a UI) to modify one of
// three stats for their ship: Speed of the ship, Fire Rate of the weapons, and
// Durability/Defense of the ship. When one stat is raised, the other two stats are lowered
// as a form of balance. Additionally, there is a cooldown timer in place before the player
// can select another stat modification.
//
// Initial Author: Erick Ramirez Cordero
// First Version: September 12, 2016
// Recent Author: Erick Ramirez Cordero
// Recent Version: September 25, 2016
//
// Recent Version Update: The code that modifies the stats has been split into three
// separate public functions (SpeedCrew, FireRateCrew, and DefenseCrew). This was
// done to allow the UI to call these functions when the player clicks one of the UI
// buttons for stat modification.
*/

public class CrewManagement : MonoBehaviour
{
    public string speedButton = "Speed Crew"; // At the moment set to 'C'
    public string fireRateButton = "Attack Crew"; // At the moment set to 'Z'
    public string defenseButton = "Defense Crew"; // At the moment set to 'X'

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

	// Use this for initialization
	void Start ()
    {
        currentSpeed = baseSpeed;
        currentFireRate = baseFireRate;
        currentDefense = baseDefense;
        cooldownTimer = 0;
	}
	
    /*
    // In the Update event, the script checks if the cooldown timer has reached its
    // end. If so, the script checks if the player has allowcated crew members
    // and calls the function for the appropriate stat.
    */
		
    // Update is called once per frame
	void Update ()
    {
        if (cooldownTimer >= cooldown)
        {
            if (Input.GetButtonDown(speedButton))
            { SpeedCrew(); }
            else if (Input.GetButtonDown(fireRateButton))
            { FireRateCrew(); }

            else if (Input.GetButtonDown(defenseButton))
            { DefenseCrew(); }
        }

        else
        { cooldownTimer += Time.deltaTime; }

        // This part checks to see if the stats have not exceeded their maximum or
        // dropped below their minimum.

        if (currentSpeed > maxSpeed) { currentSpeed = maxSpeed; }
        if (currentSpeed < minSpeed) { currentSpeed = minSpeed; }

        if (currentFireRate > maxFireRate) { currentFireRate = maxFireRate; }
        if (currentFireRate < minFireRate) { currentFireRate = minFireRate; }

        if (currentDefense > maxDefense) { currentDefense = maxDefense; }
        if (currentDefense < minDefense) { currentDefense = minDefense; }
	}

    /*
    // When SpeedCrew is called, the script checks if the current speed has not exceeded
    // the max speed. The cooldown timer is checked once more in the case the function is
    // accessed via UI button click.
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

            if (debugFlag)
            {
                Debug.Log("Speed has increased to: " + currentSpeed);
                Debug.Log("Fire Rate has decreased to: " + currentFireRate);
                Debug.Log("Defense has decreased to: " + currentDefense);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    /*
    // When FireRateCrew is called, the script checks if the current fire rate has not 
    // exceeded the max fire rate. The cooldown timer is checked once more in the case
    // the function is accessed via UI button click.
    // If the checks are true, then the current fire rate is raised by its modification
    // rate times the increment rate, the other stats are lowered by their modification
    // rates, and the cooldown timer is reset.
    */

    public void FireRateCrew()
    {
        if (cooldownTimer >= cooldown && currentFireRate < maxFireRate)
        {
            currentSpeed -= modifySpeed;
            currentFireRate += modifyFireRate * increment;
            currentDefense -= modifyDefense;
            cooldownTimer = 0;

            if (debugFlag)
            {
                Debug.Log("Speed has decreased to: " + currentSpeed);
                Debug.Log("Fire Rate has increased to: " + currentFireRate);
                Debug.Log("Defense has decreased to: " + currentDefense);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }

    /*
    // When DefenseCrew is called, the script checks if the current defense has not exceeded
    // the max defense. The cooldown timer is checked once more in the case the function is
    // accessed via UI button click.
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

            if (debugFlag)
            {
                Debug.Log("Speed has decreased to: " + currentSpeed);
                Debug.Log("Fire Rate has decreased to: " + currentFireRate);
                Debug.Log("Defense has increased to: " + currentDefense);
            }
        }

        else
        { Debug.Log("Crew Unavailable!"); }
    }
}