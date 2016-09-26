using UnityEngine;
using System.Collections;

/*
If an object falls below 0, they will accelerate back up to 0.
Note: This should be changed to work with waves, as it currently does not take waves into account.

Made 9/21/16 by Ryan Black
*/

public class Buoyancy : MonoBehaviour {

    public float upwardForce = 10.0f;

    // Use this for initialization
    void Start () {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        float ypos = transform.position.y;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 force = transform.up * upwardForce;
        Rigidbody rb = this.GetComponent<Rigidbody>();

        float ypos = transform.position.y;

        if (ypos < 0)
        {
            rb.AddRelativeForce(force, ForceMode.Acceleration);
        }
    }
}
