using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	
	private  float Y_angle_Min = 18.0f;
	private  float Y_angle_Max = 25.0f;   

	public Transform lookAt;
	public Transform camTransform;

	private Camera cam;

	private float distance = 5.0f;
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
		private void Start () 
			{
				camTransform = transform;
				cam = Camera.main;
			}
		
		// Update is called once per frame
	private void Update ()
	{
		if (fps == false) 
		{
			currentX += Input.GetAxis ("Mouse X") * sensitivityX;
			currentY -= Input.GetAxis ("Mouse Y") * sensitivityY;

			currentY = Mathf.Clamp (currentY, Y_angle_Min, Y_angle_Max);
		} 
		else if(fps == true)
		{
			currentX += Input.GetAxis ("Mouse X") * offsetX;
			currentY -= Input.GetAxis ("Mouse Y") * offsetY;

			currentY = Mathf.Clamp (currentY, Y_angle_Min, Y_angle_Max);
		}
	
		//Tab Key allows to switch between first person and third person view
		if (InputWrapper.GetKeyDown(KeyCode.Tab)) 
		{
			if (fps == false) {
				
				fps = true;
				Y_angle_Max = 30.0f;
				sensitivityY = 2.0f * offsetY;
				distance = 1.5f;

			} 
			else 
			{
				fps = false;
				Y_angle_Max = 25.0f;
				sensitivityY = 1.0f;
				distance = 5.0f;
				
			}
			
		 }
	}



	private void LateUpdate()
		{	
			Vector3 dir = new Vector3 (0, 0, -distance);
			Vector3 offset = new Vector3 (0, 1.0f, 0);
			

		if (fps == false) 
			{
				Quaternion rotation = Quaternion.Euler (currentY, currentX,0 );
				camTransform.position = lookAt.position + rotation * dir;
				camTransform.LookAt (lookAt.position);
			} 


		else //sets the camera on top of the boat to give it the first person perspective
			{
				Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
				camTransform.position = lookAt.position + offset;
				camTransform.rotation = rotation;

			}

		}
			
}