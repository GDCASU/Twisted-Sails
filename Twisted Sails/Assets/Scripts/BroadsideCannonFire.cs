// The BroadsideCannonFire class governs the firing behavior of the broadside cannons
// such as what projectile is fired, the rate of fire of the broadside cannons, and the
// speed of the projectile.

// Edward Borroughs 9/15/2016

using UnityEngine;
using System.Collections;

public class BroadsideCannonFire : MonoBehaviour {

	public GameObject cannonBall;

	private float reloadTime;
	public float fireDelay;
	public float projectileSpeed;
	
	private void Update () {
		reloadTime += Time.deltaTime;
		if (reloadTime >= fireDelay) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				reloadTime = 0;

				GameObject _cannonBall = GameObject.Instantiate (cannonBall);
				Physics.IgnoreCollision(_cannonBall.GetComponent<Collider>(), this.GetComponent<Collider>());
				_cannonBall.transform.position = this.transform.position;
				// This scales the size of the projectile to the diameter of the cannon barrel
				_cannonBall.transform.localScale = new Vector3(this.transform.lossyScale.x, this.transform.lossyScale.x, this.transform.lossyScale.x);
				// Sets the initial velocity of the cannonBall to projectileSpeed units/second in the direction of the cannon barrel
				_cannonBall.GetComponent<Rigidbody>().velocity = this.transform.up * projectileSpeed;
			}
		}
	}
}