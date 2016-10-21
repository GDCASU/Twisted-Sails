// The BroadsideCannonFire class governs the firing behavior of the broadside cannons
// such as what projectile is fired, the rate of fire of the broadside cannons, and the
// speed of the projectile.

// Edward Borroughs 9/15/2016
// 9th Oct 16  Scotty Molt - BoatMovementNetworked now calls this script to attempt to fire
//  - Cannonball Scale is now static and set on Start() to ensure uniformity across Server and Clients
//	- Moved fire script to BoatMovementNetworked, since only NetworkIdentity object can spawn objects on server+clients
//	- Added helper functions CanFire(), ResetFireTimer(), GetCannonBallVelocity()
//	- Updated default values to Inspector values

using UnityEngine;
using System.Collections;

public class BroadsideCannonFireNetworked : MonoBehaviour {
	
	private float reloadTime = 0f;
	public float fireDelay = 1f;
	public float projectileSpeed = 20f;

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
		return reloadTime >= fireDelay;
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
}