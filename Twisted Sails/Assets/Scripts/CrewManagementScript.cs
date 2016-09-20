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
// Recent Version: September 18, 2016
*/

public class CrewManagementScript : MonoBehaviour
{
    public string speedButton = "Fire1";
    public string fireRateButton = "Fire2";
    public string defenseButton = "Fire3";

    public float baseSpeed = 5.0f;
    public float baseFireRate = 5.0f;
    public float baseDefense = 5.0f;

    public float maxSpeed = 10.0f;
    public float maxFireRate = 10.0f;
    public float maxDefense = 10.0f;

    public float minSpeed = 1.0f;
    public float minFireRate = 1.0f;
    public float minDefense = 1.0f;

    public float modifySpeed = 0.5f;
    public float modifyFireRate = 0.5f;
    public float modifyDefense = 0.5f;
    public float increment = 2.0f;

    public float currentSpeed;
    public float currentFireRate;
    public float currentDefense;

    public float cooldown = 3.0f;
    public float cooldownTimer;

    public bool debugFlag = true;

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
    // and modifies stats accordingly.
    */
    
    // Update is called once per frame
	void Update ()
    {
        if (cooldownTimer >= cooldown)
        {
            if (Input.GetButtonDown(speedButton) && currentSpeed < maxSpeed)
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

            else if (Input.GetButtonDown(fireRateButton) && currentFireRate < maxFireRate)
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

            else if (Input.GetButtonDown(defenseButton) && currentDefense < maxDefense)
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
}
