using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshipHeavyProjectileBeahvior : MonoBehaviour {

	Transform startingLocation;
	Transform currentLocation;
	float distanceFromOrigin;
	public float maxDistance =1f;


	// Use this for initialization
	void Start () {
		startingLocation = this.transform; 
		Debug.Log("My starting location was:" + startingLocation.position);
	}
	
	// Update is called once per frame
	void Update () {
		currentLocation = this.transform;
		Debug.Log ("current location is" + currentLocation.position);
		distanceFromOrigin = Mathf.Abs(Vector3.Distance(currentLocation.position, startingLocation.position));
		Debug.Log (distanceFromOrigin);
		if (distanceFromOrigin >= maxDistance) {
			Debug.Log ("I have hit my terminal location, please blow me up");
		}

		
	}
		
}
