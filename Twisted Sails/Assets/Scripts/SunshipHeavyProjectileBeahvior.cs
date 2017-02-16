using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshipHeavyProjectileBeahvior : MonoBehaviour {

	Vector3 startingLocation; //Location fired from
	Vector3 currentLocation; //Location current at
	float distanceFromOrigin; //Distance from starting to current (Always take ABS)
	public float maxDistance =1f; //Maximum distance until projectile explodes
	public float explosionCircum = 1f; //Distance check from explosion point
	GameObject[] explosionTargets; //List of players currently in the level, checks them later to be in the area of the explosion
	public GameObject firingPlayer;


	// Use this for initialization
	void Start () {
		startingLocation = this.transform.position; 
		Debug.Log("My starting location was:" + startingLocation);
	}
	
	// Update is called once per frame
	void Update () {

		Debug.Log (firingPlayer);

		currentLocation = this.transform.position;
		//Updating the current location every frame

		//Debug.Log ("current location is" + currentLocation);

		distanceFromOrigin = Mathf.Abs(Vector3.Distance(currentLocation, startingLocation));
		//Calculating the distance from when fired to current location

		//Debug.Log (distanceFromOrigin);


		// Checks for the position to be at the terminal point, once reached it "explodes" checking for any players within the explosionCircum variable distance
		if (distanceFromOrigin >= maxDistance) {
			explosionTargets = GameObject.FindGameObjectsWithTag ("Player");

			foreach (GameObject target in explosionTargets) 
			{
				if (target != firingPlayer && Vector3.Distance(this.transform.position, target.transform.position) < explosionCircum) {
					Debug.Log ("Sunship Projectile - TERMINAL EXPLOSION: Hit" + target);
					//Destroy(target); 
				
				}
			}

			Debug.Log ("I have hit my terminal location, please blow me up");
			Destroy (this.gameObject); 
		}

		
	}
		
		
}
