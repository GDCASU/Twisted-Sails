/**
// The Stat System will serve as the general system for the statistics of different ships
// and mechanics that rely on altering ship stats (i.e. Crew Management). Three stats are
// available:

//      Attack  -> Influences Fire Rate of cannons      -> BroadsideCannonFireNetworked Scripts
//      Defense -> Influences amount of damage taken    -> Health Script
//      Speed   -> Influences movement of the ship      -> BoatMovementNetworked Script

// Each stat has a BASE constant to denote the starting value. These values can be changed
// for ships to have unique stats. To alter the stats of a ship during gameplay, the 
// appropriate MOD variable should be changed by calling one of the Alter Functions.
// The stats are calculated as:

//      Attack   = 1 / (BASE + MOD)
//      Defense  = 1 / (BASE + MOD)
//      Speed    = BASE + MOD

// Author:  Erick Ramirez Cordero
// Date:    January 30, 2017
*/

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StatSystem : NetworkBehaviour
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
    
    private const float MOD_MIN = 0.0f;
    private const float MOD_MAX = 1.0f;

    [Header("UI Variables")]
    public Slider attackBar;
    public Slider defenseBar;
    public Slider speedBar;

    public Text attackText;
    public Text defenseText;
    public Text speedText;

    // Scripts influenced by Crew Management
    private Health healthScript;
    private BoatMovementNetworked boatScript;

    // Set to the number of cannons on the ship (Currently 8)
    private BroadsideCannonFireNetworked[] fireScripts;

    private bool debugFlag = true;

    void Start ()
    {
        if (isLocalPlayer)
        {
            // Initialize scripts
            healthScript = GetComponentInChildren<Health>();
            boatScript = GetComponentInChildren<BoatMovementNetworked>();
            fireScripts = GetComponentsInChildren<BroadsideCannonFireNetworked>();

            // Initialize UI components
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

            attackMod = MOD_MIN;
            defenseMod = MOD_MIN;
            speedMod = MOD_MIN;

            for (int i = 0; i < fireScripts.Length; i++)
            { fireScripts[i].attackStat = currentAttack; }

            healthScript.defenseStat = currentDefense;
            boatScript.speedStat = currentSpeed;

            // Initialize UI values
            attackBar.minValue = 0;
            attackBar.maxValue = BASE_ATTACK + MOD_MAX;
            attackBar.value = BASE_ATTACK;
            attackText.text = "Attack: " + currentAttack;

            defenseBar.minValue = 0;
            defenseBar.maxValue = BASE_DEFENSE + MOD_MAX;
            defenseBar.value = BASE_DEFENSE;
            defenseText.text = "Defense: " + currentDefense;

            speedBar.minValue = 0;
            speedBar.maxValue = BASE_SPEED + MOD_MAX;
            speedBar.value = BASE_SPEED;
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
        attackMod = Mathf.Clamp(attackMod + newMod, MOD_MIN, MOD_MAX);
        currentAttack = 1 / (BASE_ATTACK + attackMod);

        for (int i = 0; i < fireScripts.Length; i++)
        { fireScripts[i].attackStat = currentAttack; }

        attackBar.value = 1 / currentAttack;
        attackText.text = "Attack: " + (1 / currentAttack);
    }

    // AlterDefense follows the same logic as AlterAttack
    public void AlterDefense(float newMod)
    {
        defenseMod = Mathf.Clamp(defenseMod + newMod, MOD_MIN, MOD_MAX);
        currentDefense = 1 / (BASE_DEFENSE + defenseMod);

        healthScript.CmdSetDefense(currentDefense);
        defenseBar.value = 1 / currentDefense;
        defenseText.text = "Defense: " + (1 / currentDefense);

        if (debugFlag)
        {
            Debug.Log("New Mod: " + newMod);
            Debug.Log("Defense Mod: " + defenseMod);
            Debug.Log("Current Defense: " + currentDefense);
        }
    }

    // AlterSpeed follows the same logic as AlterAttack, but speed is not inverted
    public void AlterSpeed(float newMod)
    {
        speedMod = Mathf.Clamp(speedMod + newMod, MOD_MIN, MOD_MAX);
        currentSpeed = BASE_SPEED + speedMod;

        boatScript.speedStat = currentSpeed;
        speedBar.value = currentSpeed;
        speedText.text = "Speed: " + currentSpeed;

        if (debugFlag)
        {
            Debug.Log("New Mod: " + newMod);
            Debug.Log("Speed Mod: " + speedMod);
            Debug.Log("Current Speed: " + currentSpeed);
        }
    }
}