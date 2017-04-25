using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnWheel : MonoBehaviour {

    public GameObject movementShip;
    public int turnDirection;
    private BoatMovementNetworked movement;
    private float turnSpeed = 50f;

	// Use this for initialization
	void Start () {
        movement = movementShip.GetComponent<BoatMovementNetworked>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * (turnSpeed * movement.speed) * turnDirection, Space.Self);
	}
}
