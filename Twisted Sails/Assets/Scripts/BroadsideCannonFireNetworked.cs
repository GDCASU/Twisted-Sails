﻿/**
// The BroadsideCannonFire class governs the firing behavior of the broadside cannons
// such as what projectile is fired, the rate of fire of the broadside cannons, and the
// speed of the projectile.

// Edward Borroughs 9/15/2016
// 9th Oct 16  Scotty Molt - BoatMovementNetworked now calls this script to attempt to fire
//  - Cannonball Scale is now static and set on Start() to ensure uniformity across Server and Clients
//	- Moved fire script to BoatMovementNetworked, since only NetworkIdentity object can spawn objects on server+clients
//	- Added helper functions CanFire(), ResetFireTimer(), GetCannonBallVelocity()
//	- Updated default values to Inspector values

// Update:      Erick Ramirez Cordero
// Date:        Novemeber 9, 2016
// Description: Added the attackStat variable to influence fire rate. When the player chooses to
//              allocate crew members to attack (fire rate), the attack stat should be updated by
//              the Crew Management Script.
*/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BroadsideCannonFireNetworked : MonoBehaviour
{
	
	private float reloadTime = 0f;
	public float fireDelay = 1f;
	public float projectileSpeed = 20f;
    public float attackStat = 1.0f; // Crew Management - Attack Crew

    //Make cannonballs spawn at Cannon scale
    void Start()
	{
		CannonBallNetworked.SetInitScale(transform.lossyScale);
	}

	//Increment fire time
	private void Update ()
	{
		reloadTime += Time.deltaTime;
	}

	//Return true if enough time has passed
	public bool CanFire()
	{
		return reloadTime >= fireDelay * attackStat;
	}

	//Reset time to fire cannon
	public void ResetFireTimer()
	{
		reloadTime = 0;
	}

	//Calculate and return the velocity of a fired cannon ball
	private static Vector3 inheritedVelocity, totalVelocity;
	public Vector3 GetCannonBallVelocity()
	{
		//Sets the initial velocity of the cannonBall to projectileSpeed units/second in the direction of the cannon barrel
		inheritedVelocity = this.transform.root.GetComponent<Rigidbody>().velocity; 
		totalVelocity = inheritedVelocity + this.transform.up * this.projectileSpeed; 
		return totalVelocity;
	}

    //Instantiates a cannonball with a velocity on the server and returns a reference to it
    public GameObject createCannonBall(GameObject cannonBall, NetworkInstanceId shooterID)
    {
        //Reset time to fire cannon
        reloadTime = 0;

        //Spawn object on server
        GameObject _cannonBall = GameObject.Instantiate(cannonBall);

        // Set position, velocity
        _cannonBall.transform.position = this.gameObject.transform.position;
        inheritedVelocity = this.transform.root.GetComponent<Rigidbody>().velocity;
        totalVelocity = inheritedVelocity + this.transform.up * this.projectileSpeed;
        _cannonBall.GetComponent<Rigidbody>().velocity = totalVelocity;

        //Ignore collision between cannonball and ship that shot it
        Physics.IgnoreCollision(_cannonBall.GetComponent<Collider>(), NetworkServer.FindLocalObject(shooterID).GetComponent<Collider>());

        _cannonBall.GetComponent<CannonBallNetworked>().owner = shooterID;

        return _cannonBall;
    }
}