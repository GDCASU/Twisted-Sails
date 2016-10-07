using UnityEngine;
using System.Collections;
/************************** BOAT MOVEMENT SCRIPT *******************************
 * This script controls the movement of the boat, now with key bindings and 
 * deceleration physics! The boat now has more realistic motion.
 * 
 * Author: Diego Wilde
 * Date Modified: September 30th, 2016
 **/


public class BoatMovement : MonoBehaviour
{
    private Rigidbody boat;
    public float acceleration = 15f;
    public float topSpeed = 60;
    public float rotationalVelocity = 25;
    public float minorRotationalVelocity = 5;
    public float speedBoostValue = 100;
    public bool speedBoost = false;

    public KeyCode forwardKey;
    public KeyCode backwardsKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    // Use this for initialization
    private void Start()
    {
        boat = this.GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        float forwardSpeed = Vector3.Dot(boat.velocity, transform.forward);
        if (Input.GetKey(forwardKey) && forwardSpeed < topSpeed)
        {
            boat.velocity += transform.forward * acceleration * Time.deltaTime;
            // Possible Speed Boost code
            if (speedBoost)
                boat.velocity += transform.forward * speedBoostValue;
            // boat can only turn if moving
            if (Input.GetKey(rightKey))
                boat.transform.Rotate(Vector3.up * rotationalVelocity * Time.deltaTime);
            if (Input.GetKey(leftKey))
                boat.transform.Rotate(-Vector3.up * rotationalVelocity * Time.deltaTime);
        }
        if (Input.GetKey(backwardsKey) && forwardSpeed > 0)
            boat.velocity -= transform.forward * (acceleration * Time.deltaTime);
        // Minor turning ability, needed in case the boat is stuck on a rock or something
        if (Input.GetKey(rightKey))
        {
            boat.transform.Rotate(Vector3.up * minorRotationalVelocity * Time.deltaTime);
        }
        if (Input.GetKey(leftKey))
        {
            boat.transform.Rotate(-Vector3.up * minorRotationalVelocity * Time.deltaTime);
        }
    }

        
    }

