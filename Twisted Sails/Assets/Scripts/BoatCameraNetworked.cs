//Oct 9th 16 Scotty Molt: 
//	- boatToFollow can now be null, set on BoatMovementNetworked::Start()
//	- Updated default values to Inspector values

using UnityEngine;
using System.Collections;

public class BoatCameraNetworked : MonoBehaviour
{
	public GameObject boatToFollow = null;

	public float orbitalDistance = 10f;
	public float rotationSpeed = 1f;
	public float upAngleChangeSpeed = 1f;

	public float maxAngleFromUp = 1.4f;
	public float minAngleFromUp = 0.1f;

	private float angleFromUp = Mathf.PI/4;
	private float rotation;

	private void Update()
	{
		if (!boatToFollow) { return; }

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
