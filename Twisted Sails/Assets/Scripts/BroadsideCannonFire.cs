// The BroadsideCannonFire class governs the firing behavior of the broadside cannons
// such as what projectile is fired, the rate of fire of the broadside cannons, and the
// speed of the projectile.

// Edward Borroughs 
// Version 10/6/2016

using UnityEngine;
using System.Collections;

public class BroadsideCannonFire : MonoBehaviour {

	public GameObject cannonBall;

	private float reloadTime;
    private float randomDelay;
    public float maxRandomDelay;
	public float fireDelay;
	public float projectileSpeed;
	
	private void Update () {
		reloadTime += Time.deltaTime;
		if (reloadTime >= fireDelay) {
			if (InputWrapper.GetKeyDown (KeyCode.Space)) {
                StartCoroutine(delay());
				reloadTime = 0;
			}
		}
	}
    private IEnumerator delay()
    {
        // This causes the cannon to wait for a random time between 0 and maxRandomDelay before firing.
        randomDelay = Random.Range(0.0f, maxRandomDelay);
        yield return new WaitForSecondsRealtime(randomDelay);

        GameObject _cannonBall = GameObject.Instantiate(cannonBall);
        Physics.IgnoreCollision(_cannonBall.GetComponent<Collider>(), this.transform.root.GetComponent<Collider>());
        Physics.IgnoreCollision(_cannonBall.GetComponent<Collider>(), this.transform.GetComponent<Collider>());
        _cannonBall.transform.position = this.transform.position;
        // This scales the size of the projectile to the diameter of the cannon barrel
        _cannonBall.transform.localScale = new Vector3(this.transform.lossyScale.x, this.transform.lossyScale.x, this.transform.lossyScale.x);
        // Sets the initial velocity of the cannonBall to projectileSpeed units/second in the direction of the cannon barrel
        Vector3 inheritedVelocity = this.transform.root.GetComponent<Rigidbody>().velocity;
        _cannonBall.GetComponent<Rigidbody>().velocity = inheritedVelocity + this.transform.up * projectileSpeed;
    }
}