using UnityEngine;
using System.Collections;
/************************** BOAT MOVEMENT SCRIPT *******************************
 * This script controls the movement of the boat.
 * 
 * Added Force based movement and revised turning. Turning is now proportional to 
 * the boat's forward speed, meaning that as the forward speed of the boat increases,
 * the torque the boat experiences is decreased. The total torque will never be less 
 * than minTorque, and thus the boat handling can be tuned by adjusting both torque 
 * and minTorque.
 * 
 * Author: Diego Wilde
 * Date Modified: September 30th, 2016
 **/


public class BoatMovement : MonoBehaviour
{
    private Rigidbody boat;

    public float forwardsAcceleration = 500;
	public float backwardsAcceleration = 200;
    public float rotationalControl = 1f;
    public float speedBoostValue = 2;
    public bool speedBoost = false;

    public KeyCode forwardKey;
    public KeyCode backwardsKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    public float boatPropulsionPointOffset;

    private void Start()
    {
        boat = this.GetComponent<Rigidbody>();
    }

	private Vector3 forcePosition;
    private void Update()
    {
		Debug.DrawRay(transform.position, transform.forward * 10, Color.green, 0, false);

		if (Input.GetAxis("Vertical") > 0)
		{
			float acceleration = forwardsAcceleration * Time.deltaTime;

			if (speedBoost)
				acceleration *= speedBoostValue;

			Vector3 forceOffset = -transform.right * (Input.GetAxis("Horizontal") * rotationalControl) + transform.forward * boatPropulsionPointOffset;

			forcePosition = transform.position + forceOffset;

			boat.AddForceAtPosition(transform.forward * acceleration, forcePosition, ForceMode.Acceleration);
		}
		else if (Input.GetAxis("Vertical") < 0)
		{
			float acceleration = backwardsAcceleration * Time.deltaTime;

			if (speedBoost)
				acceleration *= speedBoostValue;

			Vector3 forceDirection = -transform.forward;

			forcePosition = transform.position + transform.forward * boatPropulsionPointOffset;
			boat.AddForceAtPosition(forceDirection * acceleration, forcePosition, ForceMode.Acceleration);
		}
    }

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(forcePosition, .2f);
	}
}

