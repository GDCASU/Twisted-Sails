using UnityEngine;
using System.Collections;

/*
If an object falls below 0, they will accelerate back up to 0.
Note: This should be changed to work with waves, as it currently does not take waves into account.

Made 9/21/16 by Ryan Black
*/

/*
    Guidelines of use:

    Mass doesn't affect how the object floats, just how fast the object falls when in the air.
    Drag directly affects how much the boat bobs and how fast it floats to the surface of the water.
        Recommending 1-5 as the values of drag for optimal bobbing
    Upward Force needs to be above gravity (currently 9.81) for the object to float to the top of the water at all
        Recommending 10
        Less than gravity will cause object to sink
        Over 15ish looks weird, but could be used as a one time thing to shoot objects out of the water (recommending >100 for this)
*/

public class Buoyancy : MonoBehaviour {

    public float buoyancyFactor = 10.0f;
    public float objectHeight = 0.0f;

    private Vector3 centerOfMass;
    private float ypos;
    private float waterLevel;
    private Rigidbody rb;
    private float bouyancyAmount;

    void Start () {
        rb = this.GetComponent<Rigidbody>();
    }
	
	void FixedUpdate () {
        waterLevel = 0.0f;
        bouyancyAmount = Mathf.Max((waterLevel - centerOfMass.y) / objectHeight, 1) * buoyancyFactor;
        Vector3 force = transform.up * bouyancyAmount;
        centerOfMass = rb.worldCenterOfMass;

        ypos = centerOfMass.y;
        //Debug.Log(centerOfMass.y);

        if (ypos < waterLevel)
        {
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }
}
