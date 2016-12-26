using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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

// Fire Rate:   Influences the fire delay of the ship's cannons. The lower the fireRateStage,
//              the faster the cannons should be able to fire.
//              The BroadsideCannonFireNetworked script uses attackMultiplier.

// Defense:     Influences damage calculation. The lower the defenseStage, the less damage a
//              ship takes from the enemy.
//              The Health script uses defenseMultiplier.

// Speed:       Influences the acceleration of the ship. The higher the speedStage, the faster
//              the ship should move.
//              The BoatMovementNetworked script uses speedMultiplier.

// CrewCount:   Keeps track of the current amount of crew members (points) available.
//              Starting amount and maximum are three points, and points can be regained by
//              removing a point from a stat.

// Author:      Erick Ramirez Cordero
// Date:        November 21, 2016

// Update:      Erick Ramirez Cordero
// Date:        December 26, 2016
// Description: Cleaning up code for easier readability (i.e. adding headers, adding summaries)
//              Also fixed an error with the "reset stats" section of the Update function
*/

public class CrewManagementThreeStage : NetworkBehaviour
{
    // Current Stage Variables
    [Header ("Stage Variables")]
    public int fireRateStage;
    public int defenseStage;
    public int speedStage;
    public int crewCount;

    public const int CREW_MIN = 1;
    public const int CREW_MAX = 4;

    // Multiplier Variables
    [Header ("Multiplier Variables")]
    public float attackMultiplier;
    public float defenseMultiplier;
    public float speedMultiplier;
    public float multiplier = 1;
    private const float BASE_MULTIPLIER = 1.0f;

    // Timer variables
    private float cooldown = 3.0f;
    private float cooldownTimer;

    // User Interface Variables
    [Header ("UI / Input Variables")]
    public string attackButton = "Attack Crew"; // At the moment set to '1'
    public string defenseButton = "Defense Crew"; // At the moment set to '2'
    public string speedButton = "Speed Crew"; // At the moment set to '3'
    public string resetButton = "Reset Crew"; // At the moment set to '4'

    private Slider attackBar;
    private Slider defenseBar;
    private Slider speedBar;

    private Text attackText;
    private Text defenseText;
    private Text speedText;
    private Text counterText;

    // Scripts influenced by Crew Management
    private Health healthScript;
    private BoatMovementNetworked boatScript;

    // Set to the number of cannons on the ship (Currently 8)
    private const int CANNON_COUNT = 8;
    private BroadsideCannonFireNetworked[] fireScripts = new BroadsideCannonFireNetworked[CANNON_COUNT];

    private bool debugFlag = true;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            // Initialize stages, multipliers, and scripts
            crewCount = CREW_MAX - 1;
            fireRateStage = CREW_MIN;
            defenseStage = CREW_MIN;
            speedStage = CREW_MIN;
            cooldownTimer = 0;

            attackMultiplier = BASE_MULTIPLIER;
            defenseMultiplier = BASE_MULTIPLIER;
            speedMultiplier = BASE_MULTIPLIER;

            healthScript = GetComponentInChildren<Health>();
            boatScript = GetComponentInChildren<BoatMovementNetworked>();
            fireScripts = GetComponentsInChildren<BroadsideCannonFireNetworked>();

            // Locate & initialize UI Elements
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
            counterText = crewUI.GetComponentInChildren<Text>();

            // Initialize UI bar min / max values
            attackBar.minValue = CREW_MIN;
            attackBar.maxValue = CREW_MAX;

            defenseBar.minValue = CREW_MIN;
            defenseBar.maxValue = CREW_MAX;

            speedBar.minValue = CREW_MIN;
            speedBar.maxValue = CREW_MAX;

            DisplayUpdate(ref fireRateStage, ref attackBar, ref attackText);
            DisplayUpdate(ref defenseStage, ref defenseBar, ref defenseText);
            DisplayUpdate(ref speedStage, ref speedBar, ref speedText);

        }
    }

    /// <summary>
    /// In the Update event, the script checks if the cooldown timer has reached its
    /// end. If not, the cooldown timer is incremented by Time.deltaTime.
    /// Otherwise, the script checks for player input. If a crew button is selected,
    /// then the function StatUpdate is called to update the stat, the stat is updated
    /// in the respective script, the function DisplayUpdate is called to update the UI,
    /// and the cooldown timer is reset.
    /// </summary>

    // Update is called once per frame
    void Update ()
    {
        if (isLocalPlayer)
        {
            if (cooldownTimer < cooldown)
            { cooldownTimer += Time.deltaTime; }

            else
            {
                if (Input.GetButtonDown(attackButton))
                {
                    StatUpdate(ref fireRateStage, ref attackMultiplier);

                    for (int i = 0; i < CANNON_COUNT; i++)
                    { fireScripts[i].attackStat = 1 / attackMultiplier; }

                    DisplayUpdate(ref fireRateStage, ref attackBar, ref attackText);
                    cooldownTimer = 0;
                }

                else if (Input.GetButtonDown(defenseButton))
                {
                    StatUpdate(ref defenseStage, ref defenseMultiplier);
                    healthScript.defenseStat = 1 / defenseMultiplier;
                    DisplayUpdate(ref defenseStage, ref defenseBar, ref defenseText);
                    cooldownTimer = 0;
                }

                else if (Input.GetButtonDown(speedButton))
                {
                    StatUpdate(ref speedStage, ref speedMultiplier);
                    boatScript.speedStat = speedMultiplier;
                    DisplayUpdate(ref speedStage, ref speedBar, ref speedText);
                    cooldownTimer = 0;
                }

                else if (Input.GetButtonDown(resetButton))
                {
                    fireRateStage = CREW_MIN;
                    defenseStage = CREW_MIN;
                    speedStage = CREW_MIN;
                    crewCount = CREW_MAX - 1;

                    attackMultiplier = BASE_MULTIPLIER;
                    defenseMultiplier = BASE_MULTIPLIER;
                    speedMultiplier = BASE_MULTIPLIER;

                    cooldownTimer = 0;

                    // Reset variables in other scripts
                    for (int i = 0; i < CANNON_COUNT; i++)
                    { fireScripts[i].attackStat = 1 / attackMultiplier; }

                    healthScript.defenseStat = 1 / defenseMultiplier;
                    boatScript.speedStat = speedMultiplier;

                    // Reset UI variables
                    DisplayUpdate(ref fireRateStage, ref attackBar, ref attackText);
                    DisplayUpdate(ref defenseStage, ref defenseBar, ref defenseText);
                    DisplayUpdate(ref speedStage, ref speedBar, ref speedText);
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
    /// <param name="stat"> The stat's stage to be updated </param>
    /// <param name="statMultiplier"> The stat multiplier to be updated </param>

    void StatUpdate(ref int stat, ref float statMultiplier)
    {
        if (crewCount >= CREW_MIN && stat < CREW_MAX)
        {
            stat++;
            statMultiplier += multiplier;
            crewCount--;
        }

        else if (stat > CREW_MIN)
        {
            stat--;
            statMultiplier -= multiplier;
            crewCount = CREW_MIN;
        }

        // Check to ensure stat has not fallen below the min or max
        if (stat < CREW_MIN) { stat = CREW_MIN; }
        else if (stat > CREW_MAX) { stat = CREW_MAX; }

        if (debugFlag)
        {
            Debug.Log("Debug - Stat after update: " + stat);
            Debug.Log("Debug - Stat Multiplier: " + statMultiplier);
            Debug.Log("Debug - Crew count: " + crewCount);
        }
    }

    /// <summary>
    /// DisplayUpdate updates the Crew Management portion of the UI with current
    /// stat information.
    /// </summary>
    /// <param name="stat"> The stat's stage </param>
    /// <param name="statBar"> The stat's UI slider </param>
    /// <param name="statText"> The stat's UI text </param>

    void DisplayUpdate(ref int stat, ref Slider statBar, ref Text statText)
    {
        statBar.value = stat;
        statText.text = "Stage: " + stat;
        counterText.text = "Available Crew: " + crewCount;
    }
}
