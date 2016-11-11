using UnityEngine;
using System.Collections;

public class TopSpeedTester : MonoBehaviour
{
	public KeyCode forwardKey = KeyCode.W;
	public KeyCode fireKey = KeyCode.Space;
	public KeyCode restartKey = KeyCode.S;
	public float speed = 1.0f;
	public bool alpha = true;
	public TopSpeedTester beta;

	private float startSpeed;
	private Vector3 startPosition;

	private void Start ()
	{
		startSpeed = speed;
		startPosition = transform.position;
	}
	
	private void Update ()
	{
		
	}

	private void FixedUpdate()
	{
		if (Input.GetKeyDown(fireKey))
		{
			if (alpha)
			{
				float positionDiff = Vector3.Project(beta.transform.position - transform.position, transform.forward).magnitude;
				Debug.Log("Speed: " + speed + ". Position difference: " + positionDiff + ".");
			}
			speed = 0.0f;
		}
		if (Input.GetKey(forwardKey))
			transform.position += transform.forward * speed * Time.fixedDeltaTime;
		if (Input.GetKeyDown(restartKey))
		{
			speed = startSpeed;
			transform.position = startPosition;
		}
	}
}