using UnityEngine;
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

public class Health : NetworkBehaviour
{
    [Header("Health")]
    [SyncVar(hook = "OnChangeHealth")]
    public float health;
    public Text healthText;
    public Slider healthSlider;
    public float healthPackAmount = 25.0f;

    [Header("Sinking")]
    public float sinkSpeed;
    public float sinkAngle;
    public float secondsToRespawn;

    [Header("Player")]
    [SyncVar]
    public string playerName;
    [SyncVar]
    public short team;
    public bool dead;

    [Header("Misc")]
    public KeyCode hurtSelfButton;
    public GameObject activeCamera;
    public GameObject explosion;
    public Vector3 spawnPoint; // NK 10/20 added original spawnpoint
    public float defenseStat; // Crew Management - Defense Crew


    private float respawnTimer;
    private bool tilting;
    private bool gameOver;

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

        //Setting up health bars & nametags
        if (isLocalPlayer) //This is the local player's ship -- use the HUD healthbar
        {
            GameObject UI = GameObject.FindGameObjectWithTag("HealthUI");
            healthSlider = UI.GetComponent<Slider>(); // NK 10/20: locates the health UI in the scene
            healthText = UI.GetComponentInChildren<Text>(); // NK 10/20 locates the health text in the scene
            CmdPlayerInit(MultiplayerManager.GetLocalClient().connection.connectionId);
        }
        else //This is a ship belonging to another player -- use the ship's healthbar & nametag
        {
            GameObject UI = transform.FindChild("Canvas").FindChild("HealthUI").gameObject;
            healthSlider = UI.GetComponent<Slider>();
            if (isClient)
            {
                //Indicate if this ship is on the local player's team
                if (ClientScene.localPlayers[0].gameObject.GetComponent<Health>().team == team)
                    healthSlider.transform.FindChild("Fill Area").GetChild(0).GetComponent<Image>().color = Color.green;
            }
            healthText = UI.transform.FindChild("Text").GetComponent<Text>();
            UI.transform.FindChild("Nametag").GetComponent<Text>().text = playerName;
            UI.SetActive(true);
        }
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 100f;
        OnChangeHealth(health); //This is called to commit the initial state of the health bar
    }

    // Update is called once per frame
    void Update()
    {
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
        if (isLocalPlayer && Input.GetKeyDown(hurtSelfButton))
        {
            CmdChangeHealth(-5, NetworkInstanceId.Invalid);
            return;
        }
    }

    //Whenever ship collides with something else
    void OnCollisionEnter(Collision c)
    {
        //Cannonball collision
        if (c.transform.gameObject.tag.Equals("Cannonball") && c.gameObject.GetComponent<CannonBallNetworked>().owner != GetComponent<NetworkIdentity>().netId)
        {
            NetworkInstanceId cannonballOwner = c.gameObject.GetComponent<CannonBallNetworked>().owner;
            short cannonballTeam = ClientScene.FindLocalObject(cannonballOwner).GetComponent<Health>().team;
            if (cannonballTeam != team)
            {
                int damage = -35;
                Player.SendPlayerDamaged(MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId), MultiplayerManager.FindPlayer(cannonballOwner), ref damage);
                if (isLocalPlayer)
                {
                    CmdChangeHealth(damage, c.transform.gameObject.GetComponent<CannonBallNetworked>().owner);
                }
                GameObject explode = (GameObject)Instantiate(explosion, c.contacts[0].point, Quaternion.identity);
                explode.GetComponent<ParticleSystem>().Emit(100);
            }
            if (isServer)
            {
                Destroy(c.gameObject);
            }
        }

        //Health Pickup collision
        if (c.transform.gameObject.tag.Equals("HealthPickUp"))
        {
            Player.SendPlayerPickup(MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId), true);
            if (isLocalPlayer)
            {
                CmdChangeHealth(healthPackAmount, NetworkInstanceId.Invalid);
            }
            if (isServer)
                Destroy(c.gameObject);
        }
    }

    //This method should not be called to change health, use CmdChangeHealth for that.
    //This method is automatically called on each client when the health changes on the server (through CmdChangeHealth)
    private void OnChangeHealth(float newHealth)
    {
        health = newHealth;
        if (isClient)
        {
            healthSlider.value = health;
            healthText.text = "Health: " + (int)health + "/100";
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

    /// <summary>
    /// This method should be used for all changes to a ship's health.
    /// It should only be called on clients (as it is a command).
    /// Use NetworkInstanceId.Invalid if there is no damage source.
    /// </summary>
    /// <param name="amount">Amount to change health by</param>
    /// <param name="source">ID of the damage/heal source. Can be NetworkInstanceId.Invalid</param>
    [Command]
    public void CmdChangeHealth(float amount, NetworkInstanceId source)
    {
        if (health == 0 && amount < 0) return; //don't register damage taken after death

        amount *= defenseStat; // Multiplier effect for defense stat

        //By setting this variable in a serverside context, the OnChangeHealth hook is called on all clients
        health = Mathf.Clamp(health + amount, 0, 100);
        if (health == 0) //Tell the server about this kill
            MultiplayerManager.GetInstance().PlayerKill(MultiplayerManager.FindPlayer(GetComponent<NetworkIdentity>().netId), source);
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
        //Disable active camera controller, enable death camera controller
        activeCamera.GetComponent<BoatCameraNetworked>().enabled = false;
        activeCamera.GetComponent<OrbitalCamera>().enabled = true;
        gameOver = true;

        //Set up game-over screen with relevant information
        GameObject endScreen = GameObject.Find("Canvas(Health)").transform.FindChild("EndScreen").gameObject;
        endScreen.SetActive(true);
        Text teamWin = endScreen.transform.FindChild("TeamWinText").GetComponent<Text>();
        teamWin.gameObject.SetActive(true);
        teamWin.text = "Team " + MultiplayerManager.GetTeam(winner).teamName + " wins!";
        teamWin.color = MultiplayerManager.GetTeam(winner).teamColor;
        if (winner == team)
            endScreen.transform.FindChild("YouWin").gameObject.SetActive(true);
        else
            endScreen.transform.FindChild("YouLose").gameObject.SetActive(true);
        Text scoreText = endScreen.transform.FindChild("FinalScore").GetComponent<Text>();
        scoreText.text = "Scores: \n";
        for (short i = 0; i < MultiplayerManager.GetCurrentGamemode().NumTeams(); i++)
        {
            scoreText.text += MultiplayerManager.GetTeam(i).teamName + ": " + teamScores[i] + "\n";
        }
    }

    /// <summary>
    /// Called by the server when a game is restarted in identical conditions.
    /// </summary>
    [ClientRpc]
    public void RpcRestartGame()
    {
        gameOver = false;
        Respawn();
        GameObject endScreen = GameObject.Find("Canvas(Health)").transform.FindChild("EndScreen").gameObject;
        endScreen.SetActive(false);
        endScreen.transform.FindChild("TeamWinText").gameObject.SetActive(false);
        endScreen.transform.FindChild("YouWin").gameObject.SetActive(false);
        endScreen.transform.FindChild("YouLose").gameObject.SetActive(false);
    }
    #endregion

    #region Other
    /// <summary>
    /// Use this method to tell the ship it's dead.
    /// All input is disabled and the ship's owner experiences a little death cinematic.
    /// </summary>
    public void Death()
    {
        dead = true;
        tilting = true;
        respawnTimer = 0;

        //Put the ship into death cinematic
        if (isLocalPlayer)
        {
            activeCamera.GetComponent<BoatCameraNetworked>().enabled = false;
            activeCamera.GetComponent<OrbitalCamera>().enabled = true;
        }
        GetComponent<BoatMovementNetworked>().enabled = false;
        GetComponent<Buoyancy>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

    }

    /// <summary>
    /// Tentative respawn method. Does not actually destroy&recreate the ship,
    /// but simulates respawning by re-initializing ship state.
    /// </summary>
    public void Respawn()
    {
        //The following code effectively "re-initializes" the boat to its original state
        //Re-enable normal boat scripts, disable death-related scripts, re-initialize positions, rotations, forces
        if (isLocalPlayer)
        {
            activeCamera.GetComponent<BoatCameraNetworked>().enabled = true;
            activeCamera.GetComponent<OrbitalCamera>().enabled = false;
            CmdChangeHealth(100, NetworkInstanceId.Invalid);
        }
        GetComponent<BoatMovementNetworked>().enabled = true;
        GetComponent<Buoyancy>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;
        dead = false;
    }
    #endregion
}