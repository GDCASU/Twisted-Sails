using UnityEngine;
using System.Collections;

public class TopSpeedTester : MonoBehaviour
{
	public KeyCode forwardKey = KeyCode.W;
	public KeyCode fireKey = KeyCode.Space;
	public KeyCode restartKey = KeyCode.S;
	public float speed = 1.0f;
    public float angularSpeed = 0.0f;
	public bool alpha = true;
	public TopSpeedTester beta;

	private float startSpeed;
    private float startAngularSpeed;
	private Vector3 startPosition;
    private Quaternion startRotation;

	private void Start ()
	{
		startSpeed = speed;
        startAngularSpeed = angularSpeed;
		startPosition = transform.position;
        startRotation = transform.rotation;
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
                float angleDiff = Quaternion.Angle(transform.rotation, beta.transform.rotation);
				Debug.Log("Speed: " + speed + ". Position difference: " + positionDiff + ". Angular speed: " + angularSpeed + ". Angular difference: " + angleDiff + ".");
			}
			speed = 0.0f;
            angularSpeed = 0.0f;
		}
        if (Input.GetKey(forwardKey))
        {
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.AngleAxis(angularSpeed * Time.fixedDeltaTime, Vector3.up);
        }
		if (Input.GetKeyDown(restartKey))
		{
			speed = startSpeed;
            angularSpeed = startAngularSpeed;
			transform.position = startPosition;
            transform.rotation = startRotation;
		}
	}
}