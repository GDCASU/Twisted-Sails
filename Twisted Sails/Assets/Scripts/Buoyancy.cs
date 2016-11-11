using UnityEngine;
using System.Collections;

/*
    Description:
        Makes objects float on water. Adjusts drag of objects based on current situation (above, on, or below water).

    Guidelines of use:

        Mass doesn't affect how the object floats (Because I used ForceMode.Acceleration rather than ForceMode.Force), just how fast the object falls when in the air.
        Drag is adjusted automatically based on the situation of the object. Drag values for above, on, and below water can be configured in Unity.
        Upward Force needs to be above gravity (currently 9.81) for the object to float to the top of the water at all
            Recommending 10
            Less than gravity will cause object to sink
            Over 15ish looks weird, but could be used as a one time thing to shoot objects out of the water (recommending >100 for this)

    Made 9/21/16 by Ryan Black
    Last updated 9/28/16 by Ryan Black
*/

public class Buoyancy : MonoBehaviour {

    public float buoyancyFactor = 10.0f;
    public float objectHeight = 0.0f;
    public float airDrag = 0.2f;
    public float surfaceDrag = 1.0f;
	public float submergedDrag = 1.25f;
	public KeyCode forwardKey = KeyCode.W;

    public float waterLevel;
    private Rigidbody rb;

    void Start () {
        rb = this.GetComponent<Rigidbody>();
    }
	
	void FixedUpdate ()
	{
		float bouyancyMult = 0;
		float boatHeight = transform.position.y;

        waterLevel = Mathf.Sin(Time.time);


		//Add upward force when Center of Mass falls below the water level
		if (boatHeight < waterLevel)
        {
            bouyancyMult = Mathf.Max(0, Mathf.Min(Mathf.Abs((waterLevel - boatHeight)) / objectHeight, 1));
			float buoyancyAmount = bouyancyMult * buoyancyFactor;
			Vector3 force = transform.up * buoyancyAmount;
            rb.AddForce(force, ForceMode.Acceleration);
        }

        //Adjust drag based on object situation
        if (bouyancyMult > .75f) //Under water
        {
            rb.drag = Input.GetKey(forwardKey) ? 0 : submergedDrag;
        }
		else if (bouyancyMult > 0f) //On water
		{
			rb.drag = Input.GetKey(forwardKey) ? 0 : surfaceDrag;
		}
        else //Above water
        {
			rb.drag = Input.GetKey(forwardKey) ? 0 : airDrag;
        }
    }
}
