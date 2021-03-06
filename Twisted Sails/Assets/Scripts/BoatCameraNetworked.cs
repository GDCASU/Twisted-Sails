﻿//Oct 9th 16 Scotty Molt: 
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

/* *
   November 9th, 2016: Kyle Chapman
   Added a simple system for adding an offset to where the camera is looking near the boat.
*/

// Developer:   Kyle Aycock
// Date:        3/30/2017
// Description: Repaired incorrect interpolation code for quick turn
//              quickTurnTime is the amount of time it takes to finish a quick turn, in seconds

/*
 Pablo Camacho
 04/08/17
 Added bool to let camera know that game is paused and it should stop hiding the cursor. 
*/

/*
 Kyle Chapman
5/15/17
 Added code to save camera settings in the playerprefs.
*/
using UnityEngine;
using System.Collections;

public class BoatCameraNetworked : MonoBehaviour
{
	public GameObject boatToFollow = null;

	public Vector3 cameraOffset= new Vector3(0, 0, 0);

	[Header("Rotation and Zoom Settings")]
	public float minimumVerticalRotation = 10.0f;
	public float maximumVerticalRotation = 50.0f;

	public float minimumZoomDistance = 5f;
	public float maximumZoomDistance = 25f;

	public LayerMask smartCameraLayerMask;

	[Header("Camera Control Settings")]

	//setting properties that put changes to the settings into the playerprefs
	private float horizontalSensitivity;
	private static readonly string horizSensPrefName = "CameraHorizontalSensitivity";
	public float horizontalSensitivityDefault = 4.0f;
	public float HorizontalSensitivity
	{
		get
		{
			return horizontalSensitivity;
		}
		set
		{
			horizontalSensitivity = value;
			PlayerPrefs.SetFloat(horizSensPrefName, value);
		}
	}

	private float verticalSensitivity;
	private static readonly string vertSensPrefName = "CameraVerticalSensitivity";
	public float verticalSensitivityDefault = 1.0f;
	public float VerticalSensitivity
	{
		get
		{
			return verticalSensitivity;
		}
		set
		{
			verticalSensitivity = value;
			PlayerPrefs.SetFloat(vertSensPrefName, value);
		}
	}

	//speed of zooming camera in or out
	private float scrollSensitivity;
	private static readonly string scrollSensPrefName = "CameraScrollingSensitivity";
	public float scrollSensitivityDefault = 10f;
	public float ScrollSensitivity
	{
		get
		{
			return scrollSensitivity;
		}
		set
		{
			scrollSensitivity = value;
			PlayerPrefs.SetFloat(scrollSensPrefName, value);
		}
	}

	//whether to invert the direction the camera moves in each dimmension
	private bool invertHorizontal = false;
	private static readonly string invertHorizPrefName = "CameraInvertHorizontal";
	public bool InvertHorizontal
	{
		get
		{
			return invertHorizontal;
		}
		set
		{
			invertHorizontal = value;
			PlayerPrefs.SetInt(invertHorizPrefName, value ? 1 : 0);
		}
	}
	private bool invertVertical = false;
	private static readonly string invertVertPrefName = "CameraInvertVertical";
	public bool InvertVertical
	{
		get
		{
			return invertVertical;
		}
		set
		{
			invertVertical = value;
			PlayerPrefs.SetInt(invertVertPrefName, value ? 1 : 0);
		}
	}

	private float targetDistance; //the distance the player wants the camera to be at from the boat
	private float distance = 15.0f; //the distance it actually has to be at
	private float currentHorizontalRotation = 0.0f;
	private float currentVerticalRotation = 0.0f;

	[Header("Quick Turn Settings")]
	public KeyCode quickTurnLeft;
    public KeyCode quickTurnRight;
	public float quickTurnTime = 1f;

	private bool quickTurning = false;
    private float quickTurnStartingRotation;
	private float quickTurnTargetRotation;
    private float quickTurnProgress;

	private Camera cam;
	private Transform camTransform;
	public bool gameIsPaused;
	
	// Use this for initialization
	private void Awake()
    {
        cam = GetComponent<Camera>();
		camTransform = this.transform;

		//try loading camera settings player prefs information
		//if nonexistant, put settings at default values
		//and store those default values in the player prefs (in the property set methods)

		if (PlayerPrefs.HasKey(horizSensPrefName))
		{
			HorizontalSensitivity = PlayerPrefs.GetFloat(horizSensPrefName);
		}
		else
		{
			HorizontalSensitivity = horizontalSensitivityDefault;
		}


		if (PlayerPrefs.HasKey(vertSensPrefName))
		{
			VerticalSensitivity = PlayerPrefs.GetFloat(vertSensPrefName);
		}
		else
		{
			VerticalSensitivity = verticalSensitivityDefault;
		}

		if (PlayerPrefs.HasKey(scrollSensPrefName))
		{
			ScrollSensitivity = PlayerPrefs.GetFloat(scrollSensPrefName);
		}
		else
		{
			ScrollSensitivity = scrollSensitivityDefault;
		}

		if (PlayerPrefs.HasKey(invertHorizPrefName))
		{
			InvertHorizontal = PlayerPrefs.GetInt(invertHorizPrefName) > 0 ? true : false;
		}
		else
		{
			InvertHorizontal = false;
		}	

		if (PlayerPrefs.HasKey(invertVertPrefName))
		{
			InvertVertical = PlayerPrefs.GetInt(invertVertPrefName) > 0 ? true : false;
		}
		else
		{
			InvertVertical = false;
		}
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
		if( !gameIsPaused)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			//else show cursor and unlck it
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		//if not doing a quickturn, do normal mouse based camera movement input
		if (!quickTurning)
		{
			//get and apply mouse movement input
			currentHorizontalRotation += Input.GetAxis("Mouse X") * horizontalSensitivity * (invertHorizontal ? -1 : 1);
			currentVerticalRotation += Input.GetAxis("Mouse Y") * verticalSensitivity * (invertVertical ? -1 : 1);
			currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minimumVerticalRotation, maximumVerticalRotation);

			//get and apply zoom input
			targetDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
			targetDistance = Mathf.Clamp(targetDistance, minimumZoomDistance, maximumZoomDistance);

			//since not turning, detect if should do a quickturn

			//quickturning to face the left was pressed
			if (InputWrapper.GetKeyDown(quickTurnLeft))
			{
                
				quickTurnTargetRotation = currentHorizontalRotation - 90f;
                quickTurnStartingRotation = currentHorizontalRotation;
				quickTurning = true;
                quickTurnProgress = 0;
			}
			//quickturning to face the right was pressed
			else if (InputWrapper.GetKeyDown(quickTurnRight))
			{
				quickTurnTargetRotation = currentHorizontalRotation + 90f;
                quickTurnStartingRotation = currentHorizontalRotation;
				quickTurning = true;
                quickTurnProgress = 0;
            }
		}
		//rotate toward the quickturn point
		else
		{
            quickTurnProgress += Time.deltaTime / quickTurnTime;
			currentHorizontalRotation = Mathf.SmoothStep(quickTurnStartingRotation, quickTurnTargetRotation, quickTurnProgress);
			if (quickTurnProgress >= 1)
			{
				quickTurning = false;
			}
		}
    }

	//calculates and sets camera position based on the previously gathered input
	private void LateUpdate()
	{
		if (!boatToFollow) { return; }

		Quaternion rotation = Quaternion.Euler(currentVerticalRotation, currentHorizontalRotation, 0);

		Vector3 followingPosition = boatToFollow.transform.position + boatToFollow.transform.right * cameraOffset.x + Vector3.up * cameraOffset.y + boatToFollow.transform.forward * cameraOffset.z;

		distance = targetDistance;
        Ray camRay = new Ray(followingPosition, -camTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(camRay, out hit, targetDistance, smartCameraLayerMask)){
            distance = hit.distance;
        }

        Vector3 dir = new Vector3(0, 0, -distance);

        camTransform.position = followingPosition + rotation * dir;
        if (camTransform.position.y < 0.1f)
            camTransform.position = new Vector3(camTransform.position.x, 0.1f, camTransform.position.z);
        camTransform.LookAt(followingPosition);
    }

}
