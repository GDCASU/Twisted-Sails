using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
/************************** BOAT MOVEMENT SCRIPT *******************************
 * This script controls the movement of the boat, now with key bindings and 
 * deceleration physics! The boat now has more realistic motion.
 * 
 * Author: Diego Wilde
 * Date Modified: September 30th, 2016
 **/
//9th Oct 16 Scotty Molt 
//	- Networked movement and attach camera on Start()
//	- Attempt to fire cannons from here instead of from BroadsideCannonFire
//	- Changed cannonball scale to static, on init in CannonBallNetworked
//	- Updated default values to Inspector values
//	11th Oct
//	-	Implemented BoatInput struct in preparation for sending input to server
//	-	Added BoatState struct, does nothing yet

public class BoatMovementNetworked : NetworkBehaviour
{
	//Movement values
    private Rigidbody boat;
    public float acceleration = 20f;
    public float topSpeed = 30;
    public float rotationalVelocity = 50;
    public float minorRotationalVelocity = 5;
    public float speedBoostValue = 100;
    public bool speedBoost = false;

	//Boat Input
    public KeyCode forwardKey	= KeyCode.A;
    public KeyCode backwardsKey = KeyCode.S;
    public KeyCode leftKey		= KeyCode.A;
    public KeyCode rightKey		= KeyCode.D;
	public KeyCode fireKey		= KeyCode.Space;
	private struct BoatInput
	{
		public bool forward, backwards, left, right, fire;
	}
	private BoatInput KeysDown;

	//Boat State (Position, Velocity, Quaternion rotation, Angular velocity)
	private struct BoatState
	{
		public float posX, posY, posZ;
		public float velX, velY, velZ;
		public float quatW, quatX, quatY, quatZ;
		public float angX, angY, angZ;
	}
	private BoatState MyState;

	//List of cannons - Assume EIGHT cannons
	public GameObject cannonBall;
	private BroadsideCannonFireNetworked[] cannonScripts 
		= new BroadsideCannonFireNetworked[8];
	//Allocate space for Position, and Velocity for firing cannonballs
	private static Vector3 cannonPosition, cannonBallVelocity;

    // Use this for initialization
    private void Start()
    {
		//Only perform for THIS player's boat
		if (!isLocalPlayer) { return; }

		boat = this.GetComponent<Rigidbody>();
		
		//Make camera follow boat
		BoatCameraNetworked boatCam = Camera.main.GetComponent<BoatCameraNetworked>();
		boatCam.boatToFollow = this.gameObject;

		//Get cannon scripts
		cannonScripts = this.GetComponentsInChildren<BroadsideCannonFireNetworked>();
    }

	//Get input for player
	void Update()
	{
		if (isLocalPlayer)
		{
			KeysDown.forward	= Input.GetKey(forwardKey);
			KeysDown.backwards	= Input.GetKey(backwardsKey);
			KeysDown.left		= Input.GetKey(leftKey);
			KeysDown.right		= Input.GetKey(rightKey);
			KeysDown.fire		= Input.GetKey(fireKey);
		}
	}

    private void FixedUpdate()
    {
		//Only perform input for THIS player
		if (!isLocalPlayer)
		{
			return;
		}

		//Fire Input
		if (KeysDown.fire)
		{ 
			foreach(BroadsideCannonFireNetworked cScript in cannonScripts)
			{
				if ( cScript.CanFire() )
				{
					cScript.ResetFireTimer();
					//Get spawn Position, Velocity of projectile 
					//(Scale is static to CannonBallNetworked script)
					cannonPosition = cScript.transform.position;
					cannonBallVelocity = cScript.GetCannonBallVelocity(); 
					//Pass information to server and spawn cannonball on all cients
					CmdFire(cannonPosition, cannonBallVelocity, this.GetComponent<NetworkIdentity>().netId);
				}
			}
		}

		//Movement Input
        float forwardSpeed = Vector3.Dot(boat.velocity, transform.forward);
        if (KeysDown.forward && forwardSpeed < topSpeed)
        {
            boat.velocity += transform.forward * acceleration * Time.deltaTime;
            // Possible Speed Boost code
            if (speedBoost)
                boat.velocity += transform.forward * speedBoostValue;
            // boat can only turn if moving
            if (KeysDown.right)
                boat.transform.Rotate(Vector3.up * rotationalVelocity * Time.deltaTime);
            if (KeysDown.left)
                boat.transform.Rotate(-Vector3.up * rotationalVelocity * Time.deltaTime);
        }
        if (KeysDown.backwards && forwardSpeed > 0)
            boat.velocity -= transform.forward * (acceleration * Time.deltaTime);
        // Minor turning ability, needed in case the boat is stuck on a rock or something
        if (KeysDown.right)
        {
            boat.transform.Rotate(Vector3.up * minorRotationalVelocity * Time.deltaTime);
        }
        if (KeysDown.left)
        {
            boat.transform.Rotate(-Vector3.up * minorRotationalVelocity * Time.deltaTime);
        }
    }

	//Called by client, runs on server.
	//Spawns cannonball on server, then on all clients.
	[Command]
	private void CmdFire(Vector3 spawnPosition, Vector3 spawnVelocity, NetworkInstanceId shooterID)
	{ 
		//Spawn object on server
		GameObject _cannonBall = GameObject.Instantiate (this.cannonBall);
		
		// Set position, velocity
		_cannonBall.transform.position = spawnPosition;
		_cannonBall.GetComponent<Rigidbody>().velocity = spawnVelocity;
		
		//Ignore collision between cannonball and ship that shot it
		Physics.IgnoreCollision(_cannonBall.GetComponent<Collider>(), 
			NetworkServer.FindLocalObject(shooterID).GetComponent<Collider>());

		//Spawn the object across all clients
		NetworkServer.Spawn(_cannonBall);
	}
}

