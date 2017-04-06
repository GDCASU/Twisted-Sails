using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;

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

// Update:      Edward Borroughs
// Date: January 27, 2016
// Description: Added swivel gun functionality as a special case of the normal broadside cannons.
//              Mouse0 (left click) was added to the list of inputs and is used to fire the 
//              swivel guns. A bool "fireSwivelGun" was added to the BoatInput struct and the bool 
//              "fire" was changed to "fireCannon". The array of cannons "cannonScripts" was 
//              increased in size from 8 to 9. Modified FixedUpdate() to rotate the swivel gun to 
//              the same angle as the boat camera.

// Update:      Edward Borroughs
// Date: February 1, 2016
// Description: Refactored the code. Now BroadsideCannonFireNetworked is in charge of instantiating 
//              fired cannon balls on the server and this code takes that instance and spawns it on
//              the other clients. 

// Update:      Edward Borroughs
// Date: March 23, 2016
// Description: Minor fixes to cannonball spawning code to prevent them from colliding with the ship
//              that spawns them.
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
    public KeyCode forwardKey	    = KeyCode.W;
    public KeyCode backwardsKey     = KeyCode.S;
    public KeyCode leftKey		    = KeyCode.A;
    public KeyCode rightKey		    = KeyCode.D;
	public KeyCode cannonFireKey	= KeyCode.Space;
    public KeyCode swivelGunFireKey = KeyCode.Mouse0;
    private struct BoatInput
	{
		public bool forward, backwards, left, right, fireCannon, fireSwivelGun;
	}

	private BoatInput KeysDown;
    private BoatInput oldKeysDown;

	public GameObject cannonBall;
    //List of cannons
    public List<BroadsideCannonFireNetworked> cannonScripts;
    public List<SwivelGun> swivelGunScripts;
	//Allocate space for Position, and Velocity for firing cannonballs
	private static Vector3 cannonPosition, cannonBallVelocity;

    //Boat cameras
    private BoatCameraNetworked boatCam;
    private OrbitalCamera orbCam;

    // Use this for initialization
    private void Start()
    {
        boat = this.GetComponent<Rigidbody>();

        //Only perform the following for THIS player's boat
        if (!isLocalPlayer) { return; }
		
		//Make orbital and boat camera follow boat
		boatCam = Camera.main.GetComponent<BoatCameraNetworked>();
        orbCam = Camera.main.GetComponent<OrbitalCamera>();
        orbCam.target = this.gameObject;
        Camera.main.GetComponent<OrbitalCamera>().enabled = false;
        boatCam.boatToFollow = this.gameObject;

        //Get cannon scripts from all cannons
        cannonScripts = new List<BroadsideCannonFireNetworked>(this.GetComponentsInChildren<BroadsideCannonFireNetworked>().Where(t => t.GetType() != typeof(SwivelGun)));
        swivelGunScripts = new List<SwivelGun>(this.GetComponentsInChildren<SwivelGun>());
    }

	//Get input for player
	void Update()
	{
		if (isLocalPlayer)
		{
            oldKeysDown = KeysDown;
			KeysDown.forward	    = InputWrapper.GetKey(forwardKey);
			KeysDown.backwards	    = InputWrapper.GetKey(backwardsKey);
			KeysDown.left		    = InputWrapper.GetKey(leftKey);
			KeysDown.right		    = InputWrapper.GetKey(rightKey);
			KeysDown.fireCannon	    = InputWrapper.GetKey(cannonFireKey);
            KeysDown.fireSwivelGun  = InputWrapper.GetKey(swivelGunFireKey);

            //detect meaningful change in input to send to server
            if(oldKeysDown.forward != KeysDown.forward ||
                oldKeysDown.backwards != KeysDown.backwards ||
                oldKeysDown.left != KeysDown.left ||
                oldKeysDown.right != KeysDown.right)
            {
                CmdBounceInput(KeysDown); //just a struct of 6 bools, shouldn't be weighty at all
            }

            if (KeysDown.fireSwivelGun)
            {

                foreach (SwivelGun sScript in swivelGunScripts)
                {
                    if (sScript.CanFire())
                    {
                        //Pass information to server and spawn cannonball on all cients
                        CmdFire(sScript.GetCannonBallPosition(), sScript.GetCannonBallVelocity());
                    }
                }
            }

            if (KeysDown.fireCannon)
			{
				foreach (BroadsideCannonFireNetworked cScript in cannonScripts)
				{
					if (cScript.CanFire())
					{
						//Pass information to server and spawn cannonball on all cients
						CmdFire(cScript.GetCannonBallPosition(), cScript.GetCannonBallVelocity());
						cScript.ResetFireTimer();
					}
				}
			}
		}
	}

    private void FixedUpdate()
    {
        //KeysDown will be networked, ships should process the networked input for more fluid movement

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

        //Asks the boat's attached swivel guns (if they exist) to update their rotations based on the rotation of the boat's camera
        if(swivelGunScripts.Count != 0 && isLocalPlayer)
        {
            foreach(SwivelGun sScript in swivelGunScripts)
            {
                sScript.updateRotation(boatCam.transform);
            }
        }
	}

    //Called by client, runs on server.
    //Spawns an existing cannonball that is on the server on all clients.
    [Command]
    private void CmdFire(Vector3 position, Vector3 velocity)
    {
		//Spawn object on server
		GameObject firedBall = Instantiate(cannonBall);
        Collider firedBallCollider = firedBall.GetComponent<Collider>();

		// Set position, velocity
		firedBall.transform.position = position;
		firedBall.GetComponent<Rigidbody>().velocity = velocity;
        foreach (Collider collider in this.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(firedBallCollider, collider);
        }

		firedBall.GetComponent<CannonBallNetworked>().owner = GetComponent<NetworkIdentity>().netId;

		NetworkServer.Spawn(firedBall);
    }

    [Command]
    private void CmdBounceInput(BoatInput input)
    {
        RpcReceiveInput(input);
    }

    [ClientRpc]
    private void RpcReceiveInput(BoatInput input)
    {
        if(!isLocalPlayer)
            KeysDown = input;
    }
}