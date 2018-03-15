﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
// Developer:   Cory Wyman
// Date:        9/12/2016
// Description: Initial code

// Developer:       Kyle Aycock
// Date:            10/7/2016
// Description:     NOTE: I kind of reworked this script to serve as more of a general ship controller - it controls health, respawning, & collisions
//                  Added ChangeHealth function to avoid clogging the update loop with needless health checks
//                  Added several public variables for interfacing with health UI, camera
//                  Changed existing code to use ChangeHealth function, Health UI
//                  Added code for death animation, switching camera modes to orbital on death
//                  Fixed cannonball collision code - however, cannonballs need to be nocollided with the ship collider!
//                  Added cannonball explosions!
//                  Changed public variables organized into groups because there's too many
//                  Changed OnTriggerEnter to OnCollisionEnter because the cannonballs are not triggers

//Developed:		Kyle Chapman
//Date:				10/20/2016
//Description:		Added hurt self button and enabled multiplayer functionality.

// Developer:       Kyle Aycock
// Date:            10/21/2016
// Description:     Fixed cannonball collisions for networking
//                  Cleaned up code a bit
//                  Added code to support health bars above other ships

//Developer:        Kyle Aycock
//Date:             11/3/2016
//Description:      Haphazardly threw in much network-related functionality. Requires cleanup
//                  Moved Death and Respawn procedures to separate methods
//                  Added nametag support
//                  Updated CmdChangeHealth to communicate with server manager on death
//                  Added CmdPlayerInit, the purpose of which is to tell the server the player was successfully spawned
//                  Added RpcEndGame and RpcRestartGame which communicate with new elements of the main Canvas object

//Developer:        Kyle Aycock
//Date:             11/9/2016
//Description:      Cleaned up documentation to be more friendly to developers who need to understand how this script works
//                  Made small changes to CmdPlayerInit to match the new system

//Developer:        Erick Ramirez Cordero
//Date:             11/9/2016
//Description:      Added the defenseStat variable to influence damage calculation. When the player
//                  chooses to allocate crew members to defense, the defense stat should be updated
//                  by the Crew Management Script.

// Developer:   Kyle Aycock
// Date:        2/1/2017
// Description: Adapted to use new Team system
//              Added code to trigger appropriate Player events

// Developer:   Kyle Chapman
// Date:        2/14/2017
// Description: Refactored for interaction with the InteractiveObjects system.

// Developer:   Kyle Aycock
// Date:        4/6/2017
// Description: Fixed connectionId bug on this script

public class Health : NetworkBehaviour
{
    [Header("Health")]
    [SyncVar(hook = "OnChangeHealth")]
    public float health;
    public Text healthText;
    public Slider healthSlider;

    [Header("Sinking")]
    public float sinkSpeed;
    public float sinkAngle;
    public float secondsToRespawn;

    [Header("Player")]
    [SyncVar]
    public string playerName;
    [SyncVar]
    public short team;
    [SyncVar]
    public int connectionId;
    public bool dead;

    [Header("Misc")]
    public KeyCode hurtSelfButton;
    public GameObject activeCamera;
    public float invincibleTime;
    public Vector3 spawnPoint; // NK 10/20 added original spawnpoint
    public Quaternion spawnRotation;
    [SyncVar]
    public float defenseStat; // Crew Management - Defense Crew
    public GameObject deathParticle;
    public ParticleSystem smokeParticle;
    public GameObject powerupParticle;
    public GameObject invincibilityParticle;


    private float respawnTimer;
    private bool tilting;
    private bool gameOver;
    private float currentInvincibleTimer;

    //Hooks are called automatically, usually there's no reason to manually call these hooks
    #region Hooks
    //Init. non-dependent variables
    void Awake()
    {
        dead = false;
        tilting = false;
        gameOver = false;
        defenseStat = 1.0f; // 100% damage taken initially
    }

    //Init. dependent variables
    void Start()
    {
        //Variable initialization
        activeCamera = Camera.main.gameObject;
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;
        smokeParticle = transform.Find("Smoke").GetComponent<ParticleSystem>();
        currentInvincibleTimer = 0;

        //Setting up health bars & nametags
        if (isLocalPlayer) //This is the local player's ship -- use the HUD healthbar
        {
            GameObject UI = GameObject.FindGameObjectWithTag("HealthUI");
            healthSlider = UI.GetComponent<Slider>(); // NK 10/20: locates the health UI in the scene
            healthText = UI.GetComponentInChildren<Text>(); // NK 10/20 locates the health text in the scene
            GameObject.Find("Chat UI").GetComponent<ChatUI>().LinkChatHandler(GetComponent<ChatHandler>());
            CmdPlayerInit(connectionId);
            transform.Find("ShipSounds").Find("MatchStart").GetComponent<AudioSource>().Play();
        }
        else //This is a ship belonging to another player -- use the ship's healthbar & nametag
        {
            GameObject UI = transform.Find("Canvas").Find("HealthUI").gameObject;
            healthSlider = UI.GetComponent<Slider>();
            if (isClient)
            {
                //Indicate if this ship is on the local player's team
                if (ClientScene.localPlayers[0].gameObject.GetComponent<Health>().team == team)
                    healthSlider.transform.Find("Fill Area").GetChild(0).GetComponent<Image>().color = Color.green;
            }
            healthText = UI.transform.Find("Text").GetComponent<Text>();
            UI.transform.Find("Nametag").GetComponent<Text>().text = playerName;
            UI.SetActive(true);
        }
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 100f;
        OnChangeHealth(health); //This is called to commit the initial state of the health bar
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInvincibleTimer > 0 && isServer)
            currentInvincibleTimer -= Time.deltaTime;
        //Death effects/respawn timer
        if (dead)
        {
            //Tilting animation
            if (tilting)
            {
                transform.Rotate(Vector3.right, -Time.deltaTime * sinkSpeed * 20);
                transform.position = transform.position + Vector3.up * (Time.deltaTime * sinkSpeed) / 10f;
                if (Mathf.Abs(transform.rotation.eulerAngles.x - 360) >= sinkAngle)
                {
                    GetComponent<Rigidbody>().useGravity = true;
                    tilting = false;
                }
            }

            //Respawn
            if (gameOver) return;
            respawnTimer += Time.deltaTime;
            if (respawnTimer > secondsToRespawn)
            {
                Respawn();
            }
        }

        //Hurt self functionality
        if (isLocalPlayer && InputWrapper.GetKeyDown(hurtSelfButton))
        {
            CmdHurtSelf(-5);
            return;
        }
    }

    //Whenever ship collides with an interactable object
    void OnCollisionEnter(Collision collision)
    {
		InteractiveObject interaction = collision.transform.GetComponent<InteractiveObject>();

		if (interaction == null)
			return;

		//allow the interaction only if the object isn't owned by player it is touching
        if (interaction.owner != GetComponent<NetworkIdentity>().netId)
		{
			GameObject otherPlayerBoat = ClientScene.FindLocalObject(interaction.owner);

			StatusEffectsManager ourEffectsManager = GetComponent<StatusEffectsManager>();

            int teamOfObject = otherPlayerBoat == null ? -1 : otherPlayerBoat.GetComponent<Health>().team;

			//allow the interaction only if the object touching us is owned by an enemy and the object is allowed to interact with enemies
			//or if the object touching us is owned by a teammate and the object is allowed to interact with teammates
			//or if the interaction object isn't owned by any particular player
			if (teamOfObject == -1 || (teamOfObject == team && interaction.DoesEffectTeammates()) || (teamOfObject != team && interaction.DoesEffectEnemies()))
			{
				//tell the interactive object about the interaction
				//giving them this health, the boat this health is attached to, this boats status effect manager, and the collision that caused the interaction
                //if(isServer)
				interaction.OnInteractWithPlayer(this, gameObject, ourEffectsManager, collision);

				//if we are the server, destroy the interactive object after the interaction
				//if it says it is destroy after interactions
				if (interaction.DoesDestroyInInteract())
				{
					Destroy(collision.gameObject);
				}
			}
		}
    }

	//Whenever ship triggers  an interactable object
	void OnTriggerEnter(Collider collider)
	{
		InteractiveObject interaction = collider.transform.GetComponent<InteractiveObject>();

		if (interaction == null)
			return;

		//allow the interaction only if the object isn't owned by player it is touching
		if (interaction.owner != GetComponent<NetworkIdentity>().netId)
		{
			GameObject otherPlayerBoat = ClientScene.FindLocalObject(interaction.owner);

			StatusEffectsManager ourEffectsManager = GetComponent<StatusEffectsManager>();

			int teamOfObject = otherPlayerBoat == null ? -1 : otherPlayerBoat.GetComponent<Health>().team;

			//allow the interaction only if the object touching us is owned by an enemy and the object is allowed to interact with enemies
			//or if the object touching us is owned by a teammate and the object is allowed to interact with teammates
			//or if the interaction object isn't owned by any particular player
			if (teamOfObject == -1 || (teamOfObject == team && interaction.DoesEffectTeammates()) || (teamOfObject != team && interaction.DoesEffectEnemies()))
			{
				//tell the interactive object about the interaction
				//giving them this health, the boat this health is attached to, this boats status effect manager, and the collision that caused the interaction
				interaction.OnInteractWithPlayerTrigger(this, gameObject, ourEffectsManager, collider);

				//if we are the server, destroy the interactive object after the interaction
				//if it says it is destroy after interactions
				if (interaction.DoesDestroyInInteract())
				{
					Destroy(collider);
				}
			}
		}
	}

	//This method should not be called to change health, use CmdChangeHealth for that.
	//This method is automatically called on each client when the health changes on the server (through CmdChangeHealth)
	private void OnChangeHealth(float newHealth)
    { 
        //play sound if this health change was negative
        if (newHealth < health)
        {
            AudioSource damageSound = transform.Find("ShipSounds").Find("TakeDamage").GetComponent<AudioSource>();
            damageSound.pitch = Random.Range(0.5f, 1.5f);
            damageSound.PlayOneShot(damageSound.clip, Random.Range(0.7f, 1f));
        }
        health = newHealth;
        if (isClient)
        {
            healthSlider.value = health;
            healthText.text = "Health: " + (int)health + "/100";

            if (health <= 25 && smokeParticle.isStopped)
                smokeParticle.Play();
            else if (health > 25 && smokeParticle.isPlaying)
                smokeParticle.Stop(); 
        }
        if (health <= 0 && !dead)
        {
            Death();
        }
    }
    #endregion

    //Commands are called on the client to send information to the server
    #region Commands
    /// <summary>
    /// This method is used to hook this ship to its owner serverside.
    /// </summary>
    /// <param name="connectionId">Owner's connection ID</param>
    [Command]
    public void CmdPlayerInit(int connectionId)
    {
        MultiplayerManager.FindPlayer(connectionId).objectId = GetComponent<NetworkIdentity>().netId;
    }

    [Command]
    public void CmdHurtSelf(float dmg)
    {
        ChangeHealth(dmg, NetworkInstanceId.Invalid);
    }

    [Command]
    public void CmdSetDefense(float newDefense)
    {
        defenseStat = newDefense;
    }
    #endregion


    //ClientRpc methods are called on the server to send information to the client
    #region ClientRpcs
    /// <summary>
    /// Called by the server to notify clients that the game is over.
    /// </summary>
    /// <param name="winner">Team that won this round</param>
    /// <param name="redScore"></param>
    /// <param name="blueScore"></param>
    [ClientRpc]
    public void RpcEndGame(short winner, int[] teamScores)
    {
        if (!isLocalPlayer) return;
        MultiplayerManager.ClientNotifyGameEnd(winner);
        //Disable active camera controller, enable death camera controller
        activeCamera.GetComponent<BoatCameraNetworked>().enabled = false;
        activeCamera.GetComponent<OrbitalCamera>().enabled = true;
        gameOver = true;

        //Set up game-over screen with relevant information
        GameObject endScreen = GameObject.Find("Canvas(Health)").transform.Find("EndScreen").gameObject;
        endScreen.SetActive(true);
        Text teamWin = endScreen.transform.Find("TeamWinText").GetComponent<Text>();
        teamWin.gameObject.SetActive(true);
        teamWin.text = "Team " + MultiplayerManager.GetTeam(winner).teamName + " wins!";
        teamWin.color = MultiplayerManager.GetTeam(winner).teamColor;
        if (winner == team)
            endScreen.transform.Find("YouWin").gameObject.SetActive(true);
        else
            endScreen.transform.Find("YouLose").gameObject.SetActive(true);
        Text scoreText = endScreen.transform.Find("FinalScore").GetComponent<Text>();
        scoreText.text = "Scores: \n";
        for (short i = 0; i < MultiplayerManager.GetCurrentGamemode().NumTeams(); i++)
        {
            scoreText.text += MultiplayerManager.GetTeam(i).teamName + ": " + teamScores[i] + "\n";
        }
        //play sounds
        if (winner == team)
        {
            transform.Find("ShipSounds").Find("MatchWin").GetComponent<AudioSource>().Play();
        }
        else
        {
            transform.Find("ShipSounds").Find("MatchLose").GetComponent<AudioSource>().Play();
        }
        GameObject.Find("InGame").GetComponent<AudioSource>().volume = 0.1f;
        InputWrapper.CaptureKeyboard();
        InputWrapper.CaptureMouse();
    }

    /// <summary>
    /// Called by the server when a game is restarted in identical conditions.
    /// </summary>
    [ClientRpc]
    public void RpcRestartGame()
    {
        gameOver = false;
        Respawn();
        GameObject endScreen = GameObject.Find("Canvas(Health)").transform.Find("EndScreen").gameObject;
        endScreen.SetActive(false);
        endScreen.transform.Find("TeamWinText").gameObject.SetActive(false);
        endScreen.transform.Find("YouWin").gameObject.SetActive(false);
        endScreen.transform.Find("YouLose").gameObject.SetActive(false);
    }
    #endregion

    #region Other
    /// <summary>
    /// This method should be used for all changes to a ship's health.
    /// It should only be called on the server.
    /// Use NetworkInstanceId.Invalid if there is no damage source.
    /// </summary>
    /// <param name="amount">Amount to change health by</param>
    /// <param name="source">ID of the damage/heal source. Can be NetworkInstanceId.Invalid</param>
    public void ChangeHealth(float amount, NetworkInstanceId source)
    {
        if ((health == 0 || currentInvincibleTimer > 0) && amount < 0) return; //don't register damage taken after death or while invincible

        //Todo: add back in this functionality in the Stat System using an event hook for PlayerDamaged
        if(amount < 0) //only for damage
            amount *= defenseStat; // Multiplier effect for defense stat

        Player.ActivateEventPlayerDamaged(MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId), MultiplayerManager.FindPlayer(source), ref amount);
        
        //By setting this variable in a serverside context, the OnChangeHealth hook is called on all clients
        health = Mathf.Clamp(health + amount, 0, 100);
        if (health == 0) //Tell the server about this kill
            MultiplayerManager.GetInstance().PlayerKill(MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId), source);
    }

    /// <summary>
    /// Use this method to tell the ship it's dead.
    /// All input is disabled and the ship's owner experiences a little death cinematic.
    /// </summary>
    public void Death()
    {
        smokeParticle.Stop();
        Instantiate(deathParticle, this.transform.position, Quaternion.LookRotation(Vector3.up));
        dead = true;
        tilting = true;
        respawnTimer = 0;
        //Put the ship into death cinematic
        if (isLocalPlayer)
        {
            activeCamera.GetComponent<BoatCameraNetworked>().enabled = false;
            activeCamera.GetComponent<OrbitalCamera>().enabled = true;
            //play the ship death voice line 1/4 of the time
            if(Random.Range(0,4) < 1) transform.Find("ShipSounds").Find("PlayerDeath").GetComponent<AudioSource>().Play();
        }
        GetComponent<BoatMovementNetworked>().enabled = false;
        GetComponent<Buoyancy>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    /// <summary>
    /// Tentative respawn method. Does not actually destroy&recreate the ship,
    /// but simulates respawning by re-initializing ship state.
    /// </summary>
    public void Respawn()
    {
        //The following code effectively "re-initializes" the boat to its original state
        //Re-enable normal boat scripts, disable death-related scripts, re-initialize positions, rotations, forces
        currentInvincibleTimer = invincibleTime;
        if (isLocalPlayer)
        {
            activeCamera.GetComponent<BoatCameraNetworked>().enabled = true;
            activeCamera.GetComponent<OrbitalCamera>().enabled = false;
            CmdHurtSelf(100); //the irony
        }
        GetComponent<BoatMovementNetworked>().enabled = true;
        GetComponent<Buoyancy>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = spawnPoint;
        transform.rotation = spawnRotation;
        Instantiate(invincibilityParticle, transform.position, invincibilityParticle.transform.rotation, transform).GetComponent<ParticleSystem>();
        dead = false;
    }
    #endregion
}