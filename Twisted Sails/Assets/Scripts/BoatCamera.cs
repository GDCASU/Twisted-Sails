using UnityEngine;
using System.Collections;

public class BoatCamera : MonoBehaviour
{
	public GameObject boatToFollow;

	public float orbitalDistance;
	public float rotationSpeed;
	public float upAngleChangeSpeed;

	public float maxAngleFromUp = Mathf.PI;
	public float minAngleFromUp = 0f;

	private float angleFromUp = Mathf.PI/4;
	private float rotation;

	private void Update()
	{
		if ( !boatToFollow ) { return; }

		if (Input.GetAxis("CameraHorizontal") > 0)
		{
			rotation += rotationSpeed * Time.deltaTime;
        }
		else if (Input.GetAxis("CameraHorizontal") < 0)
		{
			rotation -= rotationSpeed * Time.deltaTime;
		}

		if (Input.GetAxis("CameraVertical") < 0)
		{
			angleFromUp += upAngleChangeSpeed * Time.deltaTime;
			if (angleFromUp > maxAngleFromUp)
			{
				angleFromUp = maxAngleFromUp;
            }
		}
		else if (Input.GetAxis("CameraVertical") > 0)
		{
			angleFromUp -= upAngleChangeSpeed * Time.deltaTime;
			if (angleFromUp < minAngleFromUp)
			{
				angleFromUp = minAngleFromUp;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!boatToFollow) { return; }

		float offsetX = orbitalDistance * Mathf.Sin(angleFromUp) * Mathf.Cos(rotation);
		float offsetY = orbitalDistance * Mathf.Cos(angleFromUp);
        float offsetZ = orbitalDistance * Mathf.Sin(angleFromUp) * Mathf.Sin(rotation);

		float newX = boatToFollow.transform.position.x + offsetX;
		float newY = boatToFollow.transform.position.y + offsetY;
		float newZ = boatToFollow.transform.position.z + offsetZ;

		transform.position = new Vector3(newX, newY, newZ);
		transform.LookAt(boatToFollow.transform);
	}

}
