using UnityEngine;
using System.Collections;

public class BoatMovement : MonoBehaviour {

	public float speed = 1.0f;
	public float rotationSpeed= 1.5f;
	private Rigidbody rb;



	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody> ();

	}
	
	// Update is called once per frame
	void Update () {
		
		 float transAmount = speed + Time.deltaTime;
		 float rotateAmount = rotationSpeed + Time.deltaTime;

	
		if (Input.GetKey (KeyCode.W)) 
		{
			transform.Translate (0, 0,transAmount );
		}

		if (Input.GetKey (KeyCode.S)) 
		{
			transform.Translate (0,0, -transAmount);
		}

		if (Input.GetKey (KeyCode.A)) 
		{
			transform.Rotate (0,-rotateAmount,0);
		}

		if (Input.GetKey(KeyCode.D)) 
		{
			transform.Rotate (0,-rotateAmount,0);
		}




	}
}
