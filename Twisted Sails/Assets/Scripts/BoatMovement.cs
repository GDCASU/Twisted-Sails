using UnityEngine;
using System.Collections;

public class BoatMovement : MonoBehaviour
{
    private Rigidbody boat;
    public float acceleration = 1.0f;
    public float topSpeed = 10;
    public float rotationalVelocity = 30;

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
		print(boat.velocity + ", " + boat.drag);
		if (Input.GetKey(forwardKey) && Vector3.Dot(boat.velocity, transform.forward) < topSpeed)
		{
			boat.velocity += transform.forward * acceleration * Time.deltaTime;
        }
		if (Input.GetKey(backwardsKey) && Vector3.Dot(boat.velocity, transform.forward) > 0)
		{
			boat.velocity -= transform.forward * (acceleration * Time.deltaTime);
		}
		if (Input.GetKey(rightKey)) {
            boat.transform.Rotate(Vector3.up * rotationalVelocity * Time.deltaTime);
        }
        if (Input.GetKey(leftKey)) {
            boat.transform.Rotate(-Vector3.up * rotationalVelocity * Time.deltaTime);
        }

		boat.angularVelocity = Vector3.zero;
    }
}

