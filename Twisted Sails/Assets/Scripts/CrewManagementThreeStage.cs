/**
// This version of the Crew Management system uses a three-stage system as opposed to the
// plus-minus system originally created. The player will have three points (crew members)
// to allocate to their ship at any point in the match. When all three points are allocated,
// points need to be removed before new points can be allocated. The player can either select
// a stat to remove a point from, or the player can press a "Reset" button to remove all points
// at once.

// Note:        The base stat represents a multiplier of one, and every point added to a stat
//              raises the multiplier by one. Due to this, stats should not have a negative
//              effect (drop below one).

// Attack:      Influences the fire delay of the ship's cannons.
// Defense:     Influences damage calculation.
// Speed:       Influences the acceleration of the ship.

// CrewCount:   Keeps track of the current amount of crew members (points) available.
//              Starting amount and maximum are three points, and points can be regained by
//              removing a point from a stat.

// Author:      Erick Ramirez Cordero
// Date:        November 21, 2016

// Update:      Erick Ramirez Cordero
// Date:        December 26, 2016
// Description: Cleaning up code for easier readability (i.e. adding headers, adding summaries)
//              Also fixed an error with the "reset stats" section of the Update function

// Update:      Erick Ramirez Cordero
// Date:        January 6, 2017
// Description: Added ResetStats function.

// Update:      Erick Ramirez Cordero
// Date:        January 30, 2017
// Description: Refactored the code to send stat modifications to the new StatSystem script
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CrewManagementThreeStage : NetworkBehaviour
{
    // Current Stage Variables
    [Header ("Stage Variables")]
    public int attackStage;
    public int defenseStage;
    public int speedStage;
    public int startCrew;

    private int statChange;
    private int crewCount;

    private const float STAGE_MULTIPLIER = 0.25f;
    private const int CREW_MIN = 1;
    private const int CREW_MAX = 6;

    // Timer variables
    private float cooldownTimer;
    private const float COOLDOWN_LIMIT = 3.0f;

    // User Interface Variables
    [Header ("Input Variables")]
    public string attackButton = "Attack Crew"; // At the moment set to '1'
    public string defenseButton = "Defense Crew"; // At the moment set to '2'
    public string speedButton = "Speed Crew"; // At the moment set to '3'
    public string resetButton = "Reset Crew"; // At the moment set to '4'
    public Text crewText;
    public Image cooldownImage;

    private StatSystem statSystem;
    private bool debugFlag = true;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            statSystem = GetComponentInChildren<StatSystem>();
            crewText = GameObject.FindGameObjectWithTag("CrewManagementUI").GetComponentInChildren<Text>();
            cooldownImage = GameObject.FindGameObjectWithTag("CrewManagementUI").GetComponentInChildren<Image>();
            cooldownImage.fillAmount = 0f;

            attackStage = CREW_MIN;
            defenseStage = CREW_MIN;
            speedStage = CREW_MIN;

            crewCount = startCrew;
            crewText.text = "Available Crew: " + crewCount;
            cooldownTimer = 0;
        }
    }

    /// <summary>
    /// In the Update event, the script checks if the cooldown timer has reached its
    /// end. If not, the cooldown timer is incremented by Time.deltaTime.
    /// Otherwise, the script checks for player input. If a crew button is selected,
    /// then the function StatUpdate is called to update the stat in relation to the
    /// number of crew members available. If any change does occur, the modification
    /// goes to the StatSystem.
    /// </summary>

    // Update is called once per frame
    void Update ()
    {
        if (isLocalPlayer)
        {
            if (cooldownTimer < COOLDOWN_LIMIT)
            {
                cooldownTimer += Time.deltaTime;
                cooldownImage.fillAmount = cooldownTimer / COOLDOWN_LIMIT;
            }

            else
            {
                if (InputWrapper.GetButtonDown(attackButton))
                {
                    statChange = StatUpdate(ref attackStage);

                    if (statChange != 0)
                    {
                        statSystem.AlterAttack(STAGE_MULTIPLIER * statChange);
                        cooldownTimer = 0;
                    }
                }

                else if (InputWrapper.GetButtonDown(defenseButton))
                {
                    statChange = StatUpdate(ref defenseStage);

                    if (statChange != 0)
                    {
                        statSystem.AlterDefense(STAGE_MULTIPLIER * statChange);
                        cooldownTimer = 0;
                    }
                }

                else if (InputWrapper.GetButtonDown(speedButton))
                {
                    statChange = StatUpdate(ref speedStage);

                    if (statChange != 0)
                    {
                        statSystem.AlterSpeed(STAGE_MULTIPLIER * statChange);
                        cooldownTimer = 0;
                    }
                }

                else if (InputWrapper.GetButtonDown(resetButton))
                { ResetStages(); }

                //if some action was performed, play the sound
                if(cooldownTimer == 0)
                {
                    transform.Find("ShipSounds").Find("CrewManagement").GetComponent<AudioSource>().Play();
                }
            }
        }
	}

    /// <summary>
    /// The function first checks the current number of available crew members (currentCrew).
    /// If currentCrew is above the minimum, the stat gains one point and currentCrew loses
    /// one point. If currentCrew is at or below the minimum, then currentCrew is reset to the
    /// minimum plus one point that is taken away from the stat.
    /// </summary>
    /// <param name="stage"> The stat's stage to be updated </param>

    int StatUpdate(ref int stage)
    {
        if (crewCount >= CREW_MIN && stage < CREW_MAX)
        {
            stage++;
            crewCount--;
            crewText.text = "Available Crew: " + crewCount;
            return 1;
        }

        else if (stage > CREW_MIN)
        {
            stage--;
            crewCount++;
            crewText.text = "Available Crew: " + crewCount;
            return -1;
        }

        else
        { return 0; }
    }

    /// <summary>
    /// ResetStages is called when when the reset key is pressed. 
    /// The stat system is first updated with the modification of:
    ///     -(stage - 1) * multiplier
    /// Then the stages, crew count, and timer are reset.
    /// </summary>
    
    void ResetStages()
    {
        // First reset ship's stats
        statSystem.AlterAttack(-(attackStage - 1) * STAGE_MULTIPLIER);
        statSystem.AlterDefense(-(defenseStage - 1) * STAGE_MULTIPLIER);
        statSystem.AlterSpeed(-(speedStage - 1) * STAGE_MULTIPLIER);

        // Reset stages and timer
        attackStage = CREW_MIN;
        defenseStage = CREW_MIN;
        speedStage = CREW_MIN;
        
        crewCount = startCrew;
        crewText.text = "Available Crew: " + crewCount;
        cooldownTimer = 0;
    }
}
