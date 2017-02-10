using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinMovement : MonoBehaviour {

	// Instance Data:
	public float speed = 10;
	public float turnFrequency;
	public float jumpSpeed = 10;
	private Rigidbody rb;

	public int timerMax;
	private int currentTime;

	// Use this for initialization
	void Start () {
		
		currentTime = timerMax;






	}
	
	// Update is called once per frame
	void Update () {
		this.move ();
		currentTime -= Time.deltaTime;
		if (currentTime % 2 == 0) {
			rb.AddForce (Vector3.up * jumpSpeed);
			//currentTime = timerMax;
		}

		/*if (currentTime % 4 == 0) {
			rb.AddRelativeTorque(Vector3.right);	
		}*/
	}

	public void move(){
	
		rb = this.GetComponent<Rigidbody> ();
		rb.AddForce (transform.forward * speed * Time.deltaTime);




	}
}
