using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BoatMovement : NetworkBehaviour
{
    private Rigidbody boat;
    public float acceleration = 1.0f;
    public float topSpeed = 10;
    public float rotationalVelocity = 30;

	public KeyCode forwardKey;
	public KeyCode backwardsKey;
	public KeyCode leftKey;
	public KeyCode rightKey;

	private BroadsideCannonFire[] cannons = new BroadsideCannonFire[8];

    // Use this for initialization
    private void Start()
    {
		if (!isLocalPlayer) { return; }

        boat = this.GetComponent<Rigidbody>();
		
		//Make player camera follow player boat
		BoatCamera boatCam = Camera.main.GetComponent<BoatCamera>();
		boatCam.boatToFollow = this.gameObject;

		//Get cannons
		cannons = GetComponentsInChildren<BroadsideCannonFire>();
    }

	private void FixedUpdate()
    {
		if (!isLocalPlayer) { return; }

		//Attempt to fire all cannons
		GameObject _cannonBall;
		foreach(BroadsideCannonFire c in cannons)
		{
			_cannonBall = c.AttemptToFire();
			if (_cannonBall != null)
			{
				CmdSpawn(_cannonBall);
			}
		}

		float forwardSpeed = Vector3.Dot(boat.velocity, transform.forward);
        if (Input.GetKey(forwardKey) && forwardSpeed < topSpeed)
		{
			boat.velocity += transform.forward * acceleration * Time.deltaTime;
        }
		if (Input.GetKey(backwardsKey) && forwardSpeed > 0)
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

	[Command]
	void CmdSpawn(GameObject projectile)
	{
		NetworkServer.Spawn(projectile);
	}
}

