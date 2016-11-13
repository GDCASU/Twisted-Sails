using UnityEngine;
using System.Collections;

//Used on an object with two colliders, one is a trigger and the other is not.
//If an object moves into the trigger, the object will be able to phase through the other collider.
//Therefore put the trigger from the middle of the object out past one side of the object
//to create a one way wall.

//First Draft:
//	Author: Kyle Chapman
//	Date: October 26, 2016

public class OneWayCollider : MonoBehaviour
{
	//the collider that will be passed through one way and not the other
	//not a trigger collider
	public Collider oneWayCollider;

	//when the triger collider is triggered (an object approaches from one side of the wall)
	//allow the object to move through the wall
	public void OnTriggerEnter(Collider collider)
	{
		Physics.IgnoreCollision(oneWayCollider, collider, true);
	}

	//when an object moves out of the trigger (away from the wall or through the other side)
	//stop allowing the object to move through the wall
	public void OnTriggerExit(Collider collider)
	{
		Physics.IgnoreCollision(oneWayCollider, collider, false);
	}
}
