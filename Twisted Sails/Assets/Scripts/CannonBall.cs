// The CannonBall class destroys the instance of the CannonBall GameObject it is 
// attached to if the vertical position of the CannonBall is below the despawnDepth
// and applies realistic gravity to the cannonBall. The scaleFactor scales gravity
// to the size of the game world (for example, a 1/10th scale game world needs 1/10th
// the gravity and a scaleFactor of 0.1).
//
// Edward Borroughs 9/15/2016

using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour {

	public float despawnDepth;
	public float scaleFactor;

	private void FixedUpdate () {
		// vertical displacement = initial vertical velocity * time + .5 * -9.81 * scaleFactor * time^2
		this.transform.GetComponent<Rigidbody> ().AddForce(new Vector3(0, -9.81f * scaleFactor, 0), ForceMode.Acceleration);

		if (this.transform.position.y <= despawnDepth) {
			Object.Destroy(this.gameObject);
		}
	}
}
