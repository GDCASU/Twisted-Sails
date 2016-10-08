using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
public class Health : MonoBehaviour
{
    [Header("Health")]
    public float health;
    public Text healthText;
    public Slider healthSlider;
    [Header("Sinking")]
    public float sinkSpeed;
    public float sinkAngle;
    public float secondsToRespawn;
    [Header("Misc")]
    public bool dead;
    public GameObject activeCamera;
    public GameObject explosion;

    //public int teamNumber;
    
    private float respawnTimer;
    private bool tilting;

    void Start()
    {
        //Variable initialization
        dead = false;
        tilting = false;
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 100f;
        ChangeHealth(0);
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
                //The following block effectively "re-initializes" the boat to its original state
                //Re-enable normal boat scripts, disable death-related scripts, re-initialize positions, rotations, forces
                activeCamera.GetComponent<BoatCamera>().enabled = true; //must change this to match whatever the active camera controller is
                activeCamera.GetComponent<OrbitalCamera>().enabled = false;
                GetComponent<BoatMovement>().enabled = true;
                GetComponent<Buoyancy>().enabled = true;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                ChangeHealth(100f - health);
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                dead = false;
            }
        }

    }

    /*IEnumerator Wait() {
		yield return new WaitForSeconds (1f);
		Destroy (this.gameObject);
	}*/

    //
    void OnCollisionEnter(Collision c)
    {
        //CannonBall collision
        if (c.transform.gameObject.name.Equals("CannonBall(Clone)")) //todo: change this to use tags when possible
        { 
            ChangeHealth(-35);
            GameObject explode = (GameObject)Instantiate(explosion, c.contacts[0].point, Quaternion.identity);
            explode.GetComponent<ParticleSystem>().Emit(100);
            Destroy(c.transform.gameObject);
        }

        //Debug.Log ("Collided with something");

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

    //Use this method to add/remove health from the ship
    public void ChangeHealth(float change)
    {
        health += change;
        healthSlider.value = health;
        healthText.text = "Health: " + (int)health + "/100";
        //staticHealth = health;
        if (health <= 0 && !dead)
        {
            dead = true;
            tilting = true;
            respawnTimer = 0;
            //Debug.Log ("Are they dead?: " + dead);
            //Master.arrayOfLives[teamNumber - 1] = Master.arrayOfLives[teamNumber - 1] - 1;
            //Instantiate (explosion, this.transform.position, this.transform.rotation);
            //StartCoroutine( Wait());
            //Destroy (this.gameObject);
            
            //The code below puts the ship into an automated death sequence
            activeCamera.GetComponent<BoatCamera>().enabled = false; //change BoatCamera to match whatever the active camera controller script is
            activeCamera.GetComponent<OrbitalCamera>().enabled = true;
            GetComponent<BoatMovement>().enabled = false;
            GetComponent<Buoyancy>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}