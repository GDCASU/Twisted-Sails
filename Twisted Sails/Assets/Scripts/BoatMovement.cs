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

    public float forceMultiplier = 2.5f;
    //public float minorRotationalVelocity = 5;
    public float torque = 1f;
    public float minTorque = 0.5f;
    public float speedBoostValue = 100;
    public bool speedBoost = false;

    private float forwardSpeed;

    public KeyCode forwardKey;
    public KeyCode backwardsKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    private Vector3 boatTurningPoint;
    // Use this for initialization
    private void Start()
    {
        boat = this.GetComponent<Rigidbody>();
        forwardSpeed = 1;
    }


    private void FixedUpdate()
    {
        forwardSpeed = 1 + Vector3.Dot(boat.velocity, transform.forward);
        if (Input.GetKey(forwardKey) )
        {
            boat.AddRelativeForce(Vector3.forward * forceMultiplier);

            // Possible Speed Boost code
            if (speedBoost)
                boat.AddForce(transform.forward * speedBoostValue);


        }
        if (forwardSpeed > 1) 
        {
            if (Input.GetKey(rightKey))
            {
                boat.AddForceAtPosition
                    (boat.transform.right * (torque / forwardSpeed + minTorque), boat.position);
                //boat.AddTorque(this.transform.up * (torque / forwardSpeed + minTorque));
            }
            if (Input.GetKey(leftKey))
            {
                boat.AddForceAtPosition
                    (-boat.transform.right * (torque / forwardSpeed + minTorque), boat.position);
                //boat.AddTorque(-this.transform.up * (torque / forwardSpeed + minTorque));
            }
        }

        if (Input.GetKey(backwardsKey) && boat.velocity.magnitude > 0)
            boat.AddRelativeForce(Vector3.back);
            

        // OLD
        // will be removed once we're sure we don't need it
        // Minor turning ability, needed in case the boat is stuck on a rock or something
    /*    if (Input.GetKey(rightKey))
        {
            boat.transform.Rotate(Vector3.up * minorRotationalVelocity * Time.deltaTime);
        }
        if (Input.GetKey(leftKey))
        {
            boat.transform.Rotate(-Vector3.up * minorRotationalVelocity * Time.deltaTime);
        } */
    }

        
    }

