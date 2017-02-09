using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrambleProjectileBehavior : MonoBehaviour {

	float speed = 1f; 
	public float travelTime = 8f; //Amount of time it takes for projectile to hit the farthest point
	Vector3 brambleTarget; //Farthest point
	Vector3 brambleStart; //Point projectile was shot from
	bool goingOut = true; //Is projectile going towards target location
	float totalTime; //Amount of time it takes for projectile to complete journey
	float distanceToTarget;

	// Use this for initialization
	void Start () {
		//Debug.Log ("Print Test");
		totalTime = travelTime * 2;
		this.Invoke ("ReturnToShipPos", travelTime);
		this.Invoke ("KillMyself", totalTime);
		brambleStart = GameObject.Find("BrambleShipPlayer(Clone)").transform.position;
		brambleTarget = GameObject.Find("BrambleShipPlayer(Clone)/HWtarget").transform.position;
		distanceToTarget = Mathf.Abs(Vector3.Distance(brambleStart, brambleTarget));
		speed = distanceToTarget / travelTime;
		//Debug.Log(distanceToTarget);
	
	}

	// Update is called once per frame
	void Update () {
		if(goingOut) 
			transform.position = Vector3.MoveTowards (this.transform.position, brambleTarget, speed * Time.deltaTime);
		else
			transform.position = Vector3.MoveTowards (this.transform.position, brambleStart, speed * Time.deltaTime);
	}
	//Destroys the projectile
	void ReturnToShipPos(){
		goingOut = false;
	}
	void KillMyself(){
		Destroy (this.gameObject);
	}

	//Destroys the projectile on collision with a player
	void OnTriggerEnter (Collider other) {
		Debug.Log ("Bramble Projectile: Collide");
		//Debug Code - Debug.Log(other.gameObject.layer);

		if (other.gameObject.layer == 8) 
			Debug.Log ("Bramble Projectile: I Hit a player (Layer8)");
				// Debug Code - Destroy (this.gameObject);
	}
}
	