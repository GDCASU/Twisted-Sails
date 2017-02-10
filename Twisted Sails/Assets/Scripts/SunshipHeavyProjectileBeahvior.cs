using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshipHeavyProjectileBeahvior : MonoBehaviour {

	Vector3 startingLocation;
	Vector3 currentLocation;
	float distanceFromOrigin;
	public float maxDistance =1f;


	// Use this for initialization
	void Start () {
		startingLocation = this.transform.position; 
		Debug.Log("My starting location was:" + startingLocation);
	}
	
	// Update is called once per frame
	void Update () {
		currentLocation = this.transform.position;
		//Debug.Log ("current location is" + currentLocation);
		distanceFromOrigin = Mathf.Abs(Vector3.Distance(currentLocation, startingLocation));
		Debug.Log (distanceFromOrigin);
		if (distanceFromOrigin >= maxDistance) {
			Debug.Log ("I have hit my terminal location, please blow me up");
			Destroy (this.gameObject); 
		}

		
	}
		
}
