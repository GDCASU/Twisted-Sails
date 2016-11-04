//Oct 9th 16 Scotty Molt: 
//	- boatToFollow can now be null, set on BoatMovementNetworked::Start()
//	- Updated default values to Inspector values
/* *
 * Oct 27th 2016 Diego Wilde
 * added bend camera, should adjust follow distance if any collider is between the camera and 
 * its focus point
 * */

 /* *
	October 29th, 2016: Kyle Chapman
	Implemented Kyle Aycocks smart camera fix(fixing weird behavior from incorrect raycasting code);
	Added options to invert the camera movement
	Changed back boat turning interaction with the camera
	Cleaned up fields, added headers, commented out confusing stuff related to first person view (unimplemented)
	Implemented quickturning
*/

using UnityEngine;
using System.Collections;

public class BoatCameraNetworked : MonoBehaviour
{
	public GameObject boatToFollow = null;

	[Header("Rotation and Zoom Bounds")]
	public float minimumVerticalRotation = 10.0f;
	public float maximumVerticalRotation = 50.0f;

	public float minimumZoomDistance = 5f;
	public float maximumZoomDistance = 25f;

	[Header("Camera Control Settings")]

	public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;
    public float scrollSensitivity = 10.0f; // Used for scroll camera instead of bend camera

	public bool invertX = false;
	public bool invertY = false;

	private float targetDistance; //the distance the player has set the camera to (they want it at)
	private float distance = 15.0f; //the distance it actually has to be at the moment
	private float currentHorizontalRotation = 0.0f;
	private float currentVerticalRotation = 0.0f;

	[Header("Quick Turn Settings")]
	public string quickTurnAxis;
	public float quickTurnSpeed = 1f;
	private bool quickTurning = false;
	private float quickTurnTargetHorizontalRotation;

	/*
	//float variables for adjusting sensitivity for first person view
	// Setting offset to 0 will disable the camera rotation, recommend using numbers greather than 0.
	public float offsetX = 1.0f;
    public float offsetY = 1.0f;
	*/

	private Camera cam;
	private Transform camTransform;

	// Use this for initialization
	private void Start()
    {
        cam = GetComponent<Camera>();
		camTransform = this.transform;
	}

	//used to gather input
    private void Update()
	{
		if (!boatToFollow)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
			return;
		}
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		//if not doing a quickturn, do normal mouse based camera movement input
		if (!quickTurning)
		{
			//get and apply mouse movement input
			currentHorizontalRotation += Input.GetAxis("Mouse X") * sensitivityX * (invertX ? -1 : 1);
			currentVerticalRotation += Input.GetAxis("Mouse Y") * sensitivityY * (invertY ? -1 : 1);
			currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minimumVerticalRotation, maximumVerticalRotation);

			//get and apply zoom input
			targetDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
			targetDistance = Mathf.Clamp(targetDistance, minimumZoomDistance, maximumZoomDistance);

			//since not turning, detect if should do a quickturn

			//quickturning to face the left was pressed
			if (Input.GetAxisRaw(quickTurnAxis) > 0)
			{
				quickTurnTargetHorizontalRotation = currentHorizontalRotation + 90f;
				quickTurning = true;
			}
			//quickturning to face the right was pressed
			else if (Input.GetAxisRaw(quickTurnAxis) < 0)
			{
				quickTurnTargetHorizontalRotation = currentHorizontalRotation - 90f;
				quickTurning = true;
			}
		}
		//rotate toward the quickturn point
		else
		{
			currentHorizontalRotation = Mathf.Lerp(currentHorizontalRotation, quickTurnTargetHorizontalRotation, quickTurnSpeed);
			if (Mathf.Abs(currentHorizontalRotation - quickTurnTargetHorizontalRotation) < .1f)
			{
				quickTurning = false;
			}
		}
    }

	//calculates and sets camera position based on the previously gathered input
	private void LateUpdate()
	{
		if (!boatToFollow) { return; }

		distance = targetDistance;
        Ray camRay = new Ray(boatToFollow.transform.position, -camTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(camRay, out hit, targetDistance)){
            distance = hit.distance;
        }

        Vector3 dir = new Vector3(0, 0, -distance);
        //Vector3 offset = new Vector3(0, 1.0f, 0);

        Quaternion rotation = Quaternion.Euler(currentVerticalRotation, currentHorizontalRotation, 0);
        camTransform.position = boatToFollow.transform.position + rotation * dir;
        camTransform.LookAt(boatToFollow.transform.position);
    }

}
