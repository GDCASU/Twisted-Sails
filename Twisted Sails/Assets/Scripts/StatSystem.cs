/**
// The Stat System will serve as the general system for the statistics of different ships
// and mechanics that rely on altering ship stats (i.e. Crew Management). Three stats are
// available:

//      Attack  - Influences Fire Rate of cannons       - BroadsideCannonFireNetworked Scripts
//      Defense - Influences amount of damage taken     - Health Script
//      Speed   - Influences movement of the ship       - BoatMovementNetworked Script

// Each stat has a BASE constant to denote the starting value. These values can be changed
// for ships to have unique stats. To alter the stats of a ship during gameplay, the 
// appropriate MOD variable should be changed by calling one of the Alter Functions. The
// stats are calculated as:

//      Attack   = BASE / MOD
//      Defense  = BASE * MOD
//      Speed    = BASE / MOD

// Author:  Erick Ramirez Cordero
// Date:    January 22, 2017
*/

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Stat : NetworkBehaviour
{
    public float currentAttack
    {
        get;
        protected set;
    }

    public float currentDefense
    {
        get;
        protected set;
    }

    public float currentSpeed
    {
        get;
        protected set;
    }

    [Header("Ship Stats")]
    private float attackMod;
    private float defenseMod;
    private float speedMod;

    private const float BASE_ATTACK = 1.0f;
    private const float BASE_DEFENSE = 1.0f;
    private const float BASE_SPEED = 1.0f;

    private const float BASE_MOD = 1.0f;
    private const float MOD_MIN = 1.0f;
    private const float MOD_MAX = 5.0f;

    [Header("UI / Input Variables")]
    private Slider attackBar;
    private Slider defenseBar;
    private Slider speedBar;

    private Text attackText;
    private Text defenseText;
    private Text speedText;

    // Scripts influenced by Crew Management
    private Health healthScript;
    private BoatMovementNetworked boatScript;

    // Set to the number of cannons on the ship (Currently 8)
    private const int CANNON_COUNT = 8;
    private BroadsideCannonFireNetworked[] fireScripts = new BroadsideCannonFireNetworked[CANNON_COUNT];

    private bool debugFlag = true;

    void Awake ()
    {
        if (isLocalPlayer)
        {
            // Initialize scripts and UI components
            healthScript = GetComponentInChildren<Health>();
            boatScript = GetComponentInChildren<BoatMovementNetworked>();
            fireScripts = GetComponentsInChildren<BroadsideCannonFireNetworked>();

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

            // Initialize Stats

            currentAttack = BASE_ATTACK;
            currentDefense = BASE_DEFENSE;
            currentSpeed = BASE_SPEED;

            for (int i = 0; i < CANNON_COUNT; i++)
            { fireScripts[i].attackStat = currentAttack; }

            healthScript.defenseStat = currentDefense;
            boatScript.speedStat = currentSpeed;

            // Initialize UI values
            attackBar.minValue = BASE_ATTACK / MOD_MIN;
            attackBar.maxValue = BASE_ATTACK / MOD_MAX;
            attackBar.value = currentAttack;
            attackText.text = "Attack: " + currentAttack;

            defenseBar.minValue = BASE_DEFENSE * MOD_MIN;
            defenseBar.maxValue = BASE_DEFENSE * MOD_MAX;
            defenseBar.value = currentDefense;
            defenseText.text = "Defense: " + currentDefense;

            speedBar.minValue = BASE_SPEED / MOD_MIN;
            speedBar.maxValue = BASE_SPEED / MOD_MAX;
            speedBar.value = currentSpeed;
            speedText.text = "Speed: " + currentSpeed;
        }
	}
	
    /*
	void Update ()
    {

	}
    */

    /// <summary>
    /// AlterAttack is to be called when another script wants to alter the attack stat
    /// of the ship. The new modification is added to attackMod, and the currentAttack
    /// is calculated using the updated attackMod. Afterwards the appropriate script(s)
    /// and UI elements are updated with the new currentAttack.
    /// </summary>
    /// <param name="newMod">   The new modification to be applied  </param>
    public void AlterAttack(float newMod)
    {
        attackMod += newMod;
        CheckModLimits(ref attackMod);
        currentAttack = BASE_ATTACK / attackMod;

        for (int i = 0; i < CANNON_COUNT; i++)
        { fireScripts[i].attackStat = currentAttack; }

        attackBar.value = currentAttack;
        attackText.text = "Attack: " + currentAttack;
    }

    public void AlterDefense(float newMod)
    {
        defenseMod += newMod;
        CheckModLimits(ref defenseMod);
        currentDefense = BASE_DEFENSE * defenseMod;

        healthScript.defenseStat = currentDefense;
        defenseBar.value = currentDefense;
        defenseText.text = "Defense: " + currentDefense;
    }

    public void AlterSpeed(float newMod)
    {
        speedMod += newMod;
        CheckModLimits(ref speedMod);
        currentSpeed = BASE_SPEED / speedMod;

        boatScript.speedStat = currentSpeed;
        speedBar.value = currentSpeed;
        speedText.text = "Speed: " + currentSpeed;
    }

    /// <summary>
    /// This function checks if the stat modification in question exceeds the maximum
    /// or falls below the minimum modification allowed, making corrections if needed.
    /// </summary>
    /// <param name="statMod"></param>
    private void CheckModLimits(ref float statMod)
    {
        if (statMod > MOD_MAX)
        { statMod = MOD_MAX; }

        else if (statMod < MOD_MIN)
        { statMod = MOD_MIN; }
    }
}