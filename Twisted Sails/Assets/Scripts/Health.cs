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
//                  ADDED ChangeHealth function to avoid clogging the update loop with needless health checks
//                  ADDED several public variables for interfacing with health UI, camera
//                  CHANGED existing code to use ChangeHealth function, Health UI
//                  ADDED code for death animation, switching camera modes to orbital on death
//                  FIXED cannonball collision code - however, cannonballs need to be nocollided with the ship collider!
//                  ADDED cannonball explosions!
//                  CHANGED public variables organized into groups because there's too many
//                  CHANGED OnTriggerEnter to OnCollisionEnter because the cannonballs are not triggers

//Developed:		Kyle Chapman
//Date:				10/20/2016
//Description:		Added hurt self button and enabled multiplayer functionality.

// Developer:       Kyle Aycock
// Date:            10/21/2016
// Description:     Fixed cannonball collisions for networking
//                  Cleaned up code a bit
//                  Added code to support health bars above other ships
public class Health : NetworkBehaviour
{
    [Header("Health")]
    //Decided to just have it hook to the ChangeHealth function
    [SyncVar(hook = "OnChangeHealth")] public float health;
    public Text healthText;
    public Slider healthSlider;
    [Header("Sinking")]
    public float sinkSpeed;
    public float sinkAngle;
    public float secondsToRespawn;
	[Header("Misc")]
	public KeyCode hurtSelfButton;
    public bool dead;
    public GameObject activeCamera;
    public GameObject explosion;
    public Vector3 spawnPoint; // NK 10/20 added original spawnpoint
    
    private float respawnTimer;
    private bool tilting;

    void Start()
    {
        //Variable initialization
        activeCamera = Camera.main.gameObject;
        dead = false;
        tilting = false;
        spawnPoint = this.transform.position;

        if (isLocalPlayer)
        {
            //Use the main HealthUI at the top of the screen if this ship is the local player's ship
            GameObject UI = GameObject.FindGameObjectWithTag("HealthUI");
            healthSlider = UI.GetComponent<Slider>(); // NK 10/20: locates the health UI in the scene
            healthText = UI.GetComponentInChildren<Text>(); // NK 10/20 locates the health text in the scene
        }
        else
        {
            //Otherwise, use the healthbar that's currently disabled within the ship
            GameObject UI = transform.FindChild("Canvas").FindChild("HealthUI").gameObject;
            healthSlider = UI.GetComponent<Slider>();
            healthText = UI.GetComponentInChildren<Text>();
            UI.SetActive(true);
        }
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 100f;
        OnChangeHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
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
            respawnTimer += Time.deltaTime;
            if (respawnTimer > secondsToRespawn)
            {
                //The following code effectively "re-initializes" the boat to its original state
                //Re-enable normal boat scripts, disable death-related scripts, re-initialize positions, rotations, forces
                if (isLocalPlayer)
                {
                    //Only manipulate the camera if it's our ship that's respawning
                    activeCamera.GetComponent<BoatCameraNetworked>().enabled = true; //must change this to match whatever the active camera controller is
                    activeCamera.GetComponent<OrbitalCamera>().enabled = false;
                    CmdChangeHealth(100);
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
        }

		if (isLocalPlayer && Input.GetKeyDown(hurtSelfButton))
		{
            CmdChangeHealth(-5);
			return;
		}

	}


    //Whenever ship collides with something else
    void OnCollisionEnter(Collision c)
    {
        //Ignore cannonball collisions with owner
        if (c.transform.gameObject.tag.Equals("Cannonball") && c.gameObject.GetComponent<CannonBallNetworked>().owner != GetComponent<NetworkIdentity>().netId) //todo: change this to use tags when possible
        {
            if (isLocalPlayer)
            {
                Debug.Log("Health changed!");
                CmdChangeHealth(-35);
            }
            GameObject explode = (GameObject)Instantiate(explosion, c.contacts[0].point, Quaternion.identity);
            explode.GetComponent<ParticleSystem>().Emit(100);
            Destroy(c.gameObject);
        }

        /*if (other.name.Equals ("Health Pack")) { //should replace with tags
			Debug.Log ("Collided with Health Pack");
            ChangeHealth(50);
			other.gameObject.GetComponent<HealthPack> ().healing = false;
		}
		if (other.name.Equals ("Damage Object")) { //should replace with tags
			Debug.Log ("Collided with Damage Object");
            ChangeHealth(-25);
			other.gameObject.GetComponent<HealthPack> ().healing = false;
		}*/
    }

    //Apparently, SyncVars with hooks only call one way (Server -> Client)
    //So I had to make a command so that health changes only happen on the server
    [Command]
    public void CmdChangeHealth(float amt)
    {
        health = Mathf.Clamp(health+amt,0,100);
    }


    //This method should not be called to change health, use CmdChangeHealth for that.
    //This method is automatically called on each client when the health changes on the server (through CmdChangeHealth)
    public void OnChangeHealth(float newHealth)
    {
        health = newHealth;
        if (isClient)
        {
            healthSlider.value = health;
            healthText.text = "Health: " + (int)health + "/100";
        }
        if (health <= 0 && !dead)
        {
            dead = true;
            tilting = true;
            respawnTimer = 0;

            //The code below puts the ship into an automated death sequence
            if (isLocalPlayer)
            {
                //Only manipulate the camera if it's the player's ship that's dying
                activeCamera.GetComponent<BoatCameraNetworked>().enabled = false; //change BoatCamera to match whatever the active camera controller script is
                activeCamera.GetComponent<OrbitalCamera>().enabled = true;
            }
            GetComponent<BoatMovementNetworked>().enabled = false;
            GetComponent<Buoyancy>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}