//Oct 9th 16 Scotty Molt: 
//	- boatToFollow can now be null, set on BoatMovementNetworked::Start()
//	- Updated default values to Inspector values

using UnityEngine;
using System.Collections;

public class BoatCameraNetworked : MonoBehaviour
{
	public GameObject boatToFollow = null;

    //public float orbitalDistance = 10f;
    //public float rotationSpeed = 1f;
    //public float upAngleChangeSpeed = 1f;

    //public float maxAngleFromUp = 1.4f;
    //public float minAngleFromUp = 0.1f;

    //private float angleFromUp = Mathf.PI/4;
    //private float rotation; 

    public float Y_angle_Min = 18.0f;
	public float Y_angle_Max = 25.0f;

    public Transform camTransform;

    private Camera cam;

    private float distance = 15.0f; // adjust this value to determine distrance from boat to camera
    private float currentX = 0.0f;
    private float currentY = 0.0f;

    public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;

    //float variables for adjusting sensitivity for first person view
    // Setting offset to 0 will disable the camera rotation, recommend using numbers greather than 0.
    public float offsetX = 1.0f;
    public float offsetY = 1.0f;

    private bool fps = false;

    // Use this for initialization
    private void Start()
    {
        camTransform = this.transform;
        cam = Camera.main;
    }

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

		currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY -= Input.GetAxis("Mouse Y") * sensitivityY;

        currentY = Mathf.Clamp(currentY, Y_angle_Min, Y_angle_Max);
    }

	private void LateUpdate()
	{
		if (!boatToFollow) { return; }

        Vector3 dir = new Vector3(0, 0, -distance);
        Vector3 offset = new Vector3(0, 1.0f, 0);

        Quaternion rotation = Quaternion.Euler(currentY, boatToFollow.transform.rotation.eulerAngles.y + currentX, 0);
        camTransform.position = boatToFollow.transform.position + rotation * dir;
        camTransform.LookAt(boatToFollow.transform.position);
    }

}
