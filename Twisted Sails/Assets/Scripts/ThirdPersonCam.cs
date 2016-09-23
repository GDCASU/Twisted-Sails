using UnityEngine;
using System.Collections;

public class ThirdPersonCam : MonoBehaviour {

	private const float Y_angle_Min = 18.0f;
	private const float Y_angle_Max = 25.0f;

	public Transform lookAt;
	public Transform camTransform;

	private Camera cam;

	private float distance = 5.0f;
	private float currentX = 0.0f;
	private float currentY = 0.0f;
	public float sensitivityX = 15.0f;
	public float sensitivityY = 1.0f;



	// Use this for initialization
	private void Start () 
	{
		camTransform = transform;
		cam = Camera.main;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		currentX += Input.GetAxis ("Mouse X");
		currentY += Input.GetAxis ("Mouse Y");

		currentY = Mathf.Clamp (currentY, Y_angle_Min, Y_angle_Max);
	
	}

	private void LateUpdate()
	{
		Vector3 dir = new Vector3 (0, 0, -distance);
		Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
		camTransform.position = lookAt.position + rotation * dir;
		camTransform.LookAt (lookAt.position);
	}
		
}
