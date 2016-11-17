using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/************************** BOAT MOVEMENT SCRIPT *******************************
 * This script controls the movement of the boat, now with key bindings and 
 * deceleration physics! The boat now has more realistic motion.
 * 
 * Author: Diego Wilde
 * Date Modified: September 30th, 2016
 */

/**
//9th Oct 16 Scotty Molt 
//	- Networked movement and attach camera on Start()
//	- Attempt to fire cannons from here instead of from BroadsideCannonFire
//	- Changed cannonball scale to static, on init in CannonBallNetworked
//	- Updated default values to Inspector values
//	11th Oct
//	-	Implemented BoatInput struct in preparation for sending input to server
//	-	Added BoatState struct, does nothing yet

// Update:      Erick Ramirez Cordero
// Date:        Novemeber 9, 2016
// Description: Added the speedStat variable to influence boat movement. When the player chooses
//              to allocate crew members to speed, the speed stat should be updated by the Crew
//              Management Script.
*/

public class BoatMovementNetworked : NetworkBehaviour
{
	//Movement values
    private Rigidbody boat;

	public float forwardsAcceleration = 500;
	public float backwardsAcceleration = 200;
	public float rotationalControl = 1f;
	public float speedBoostValue = 2;
	public bool speedBoost = false;
	public float boatPropulsionPointOffset = -1f;
    public float speedStat = 1.0f; // Crew Management - Speed Crew

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
		
		//Make orbital and boat camera follow boat
		BoatCameraNetworked boatCam = Camera.main.GetComponent<BoatCameraNetworked>();
        OrbitalCamera orbCam = Camera.main.GetComponent<OrbitalCamera>();
        orbCam.target = this.gameObject;
        Camera.main.GetComponent<OrbitalCamera>().enabled = false;
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

			if (KeysDown.fire)
			{
				foreach (BroadsideCannonFireNetworked cScript in cannonScripts)
				{
					if (cScript.CanFire())
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
		}
	}

    private void FixedUpdate()
    {
		//Only perform input for THIS player
		if (!isLocalPlayer)
		{
			return;
		}

		float horizontalAxis = 0;
		if (KeysDown.left)
			horizontalAxis--;
		if (KeysDown.right)
			horizontalAxis++;

		if (KeysDown.forward && !KeysDown.backwards)
		{
			float acceleration = forwardsAcceleration * Time.deltaTime;

            acceleration *= speedStat; // Multiplier effect for speed stat

            if (speedBoost)
				acceleration *= speedBoostValue;

			Vector3 forceOffset = -transform.right * (horizontalAxis * rotationalControl) + transform.forward * boatPropulsionPointOffset;

			boat.AddForceAtPosition(transform.forward * acceleration, transform.position + forceOffset, ForceMode.Acceleration);
		}
		else if (!KeysDown.forward && KeysDown.backwards)
		{
			float acceleration = backwardsAcceleration * Time.deltaTime;

            acceleration *= speedStat; // Multiplier effect for speed stat

            if (speedBoost)
				acceleration *= speedBoostValue;

			Vector3 forceDirection = -transform.forward;

			boat.AddForceAtPosition(forceDirection * acceleration, transform.position + transform.forward * boatPropulsionPointOffset, ForceMode.Acceleration);
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
		Physics.IgnoreCollision(_cannonBall.GetComponent<Collider>(), NetworkServer.FindLocalObject(shooterID).GetComponent<Collider>());

        _cannonBall.GetComponent<CannonBallNetworked>().owner = shooterID;

		//Spawn the object across all clients
		NetworkServer.Spawn(_cannonBall);
	}
}