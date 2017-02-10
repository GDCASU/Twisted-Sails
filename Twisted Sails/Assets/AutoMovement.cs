using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMovement : MonoBehaviour {

	// Instance Data:
	public double speed;
	public double turnFrequency;
	public Rigidbody rb;


	
	// Update is called once per frame
	void Update () {
		
	}


	public static void randomMove(){
	
		rb = this.GetComponent<Rigidbody> ();



	}

}
