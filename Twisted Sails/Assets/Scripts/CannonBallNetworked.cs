// The CannonBall class destroys the instance of the CannonBall GameObject it is 
// attached to if the vertical position of the CannonBall is below the despawnDepth
// and applies realistic gravity to the cannonBall. The scaleFactor scales gravity
// to the size of the game world (for example, a 1/10th scale game world needs 1/10th
// the gravity and a scaleFactor of 0.1).
//
// Edward Borroughs 9/15/2016
// 10th Oct 16 Scotty Molt
//	- Cannonball Scale is now static and set on Start() to ensure uniformity across Server and Clients
//	- Added helper function SetInitScale(Vetor3) to be called from BroadSideCannonNetworked script
//	- Added initScale so cannonballs across network spawn at same size

// Developer: Kyle Aycock
// Date: 10/26/2016
//	- Aesthetic changes. Upon reaching water level, spawn splash particlesystem.
//	- Variables added to ensure only one splash is created
//	- Changes to movement upon entering water (higher drag)

// Developer: Kyle Chapman
// Date: 2/15/2017
// Description: Refacted to work within the InteractiveObject system.

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CannonBallNetworked : InteractiveObject {

	public int damageDealt;
	public float despawnDepth = -3f;
	public float scaleFactor = 0.7f;
    public GameObject splashPrefab;
	private static Vector3 initScale = Vector3.zero;
    private bool splashed;
    public GameObject smoke;
	public GameObject explosion;

	//Set size of cannonball
	void Start()
	{
        splashed = false;
		this.transform.localScale = initScale;
        AudioSource fireSound = GetComponent<AudioSource>();
        fireSound.pitch = Random.Range(0.6f, 1.4f);
        fireSound.Play();
        GetComponent<Rigidbody>().AddForce(0, 3f, 0, ForceMode.VelocityChange);
        if(isClient)
        {
            Instantiate(smoke, transform.position, smoke.transform.rotation, ClientScene.FindLocalObject(owner).transform).transform.LookAt(transform.position+GetComponent<Rigidbody>().velocity);
        }
	}

    //Set default scale of all cannonballs
    public static void SetInitScale (Vector3 newInitScale)
	{
		initScale = newInitScale;
	}

	private void FixedUpdate () {
		// vertical displacement = initial vertical velocity * time + .5 * -9.81 * scaleFactor * time^2
		this.transform.GetComponent<Rigidbody> ().AddForce(new Vector3(0, -9.81f * scaleFactor, 0), ForceMode.Acceleration);

	    //Note: Water level assumed to be 0. Changes to water level must be reflected here.
        if(transform.position.y < 0 && !splashed)
        {
            Instantiate(splashPrefab, transform.position, splashPrefab.transform.rotation);
            splashed = true;
            GetComponent<Rigidbody>().drag = 1f;
        }

		//Check for destruction
		if (this.transform.position.y <= despawnDepth)
		{
			Object.Destroy(this.gameObject);
		}
			//destroy cannonball after 5 seconds, to avoid idle objects in game
			Destroy(this.gameObject, 5);
	}

	public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
	{
		int healthChange = -damageDealt;

		//if this object is on the side of the player who owns this object
		//send out the command to change the players health
		//setting the source of the health change to be the owner of this cannonball
        if(isServer)
            playerHealth.ChangeHealth(healthChange, owner);

        //locally instantiates an explosion prefab at the site of the interaction for graphics
        GameObject explode = (GameObject)Instantiate(explosion, collision.contacts[0].point, Quaternion.identity);
		explode.GetComponent<ParticleSystem>().Emit(100);
	}

	public override bool DoesEffectTeammates()
	{
		return false;
	}

	public override bool DoesEffectEnemies()
	{
		return true;
	}
}
