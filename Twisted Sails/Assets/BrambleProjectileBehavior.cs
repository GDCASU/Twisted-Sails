using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrambleProjectileBehavior : MonoBehaviour {

	public float speed = 1f;
	public float travelTime = 8f;
	Vector3 brambleTarget;
	Vector3 brambleStart;
	bool goingOut = true;

	// Use this for initialization
	void Start () {
		this.Invoke ("ReturnToShipPos", travelTime);
		brambleStart = GameObject.Find("BrambleShipPlayer(Clone)").transform.position;
		brambleTarget = GameObject.Find("BrambleShipPlayer(Clone)/HWtarget").transform.position;
	
	}

	// Update is called once per frame
	void Update () {
		if(goingOut) 
			transform.position = Vector3.MoveTowards (this.transform.position, brambleTarget, speed * Time.deltaTime);
		else
			transform.position = Vector3.MoveTowards (this.transform.position, brambleStart, speed * Time.deltaTime);
	}
	void ReturnToShipPos(){
		goingOut = false;
	}
}
	