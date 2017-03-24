using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
	[Template Class Header]
	Template Makers:	Kyle Chapman
	Date:				2/17/2016

    Duplicate this script and rename the duplicate
	to place on a object meant to interact with boats.

	If need help using this template, contact the programmer above.
**/

public class InteractiveObjectsTemplate : InteractiveObject
{
	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}

	/// <summary>
	/// Return if the object should interact with teammates of the object's owner (if applicable).
	/// </summary>
	/// <returns>If the object should interact with teammates of the object's owner.</returns>
	public override bool DoesEffectTeammates()
	{
		return false;
	}

	/// <summary>
	/// Return if the object should interact with enemies of the object's owner (if applicable).
	/// </summary>
	/// <returns>If the object should interact with enemies of the object's owner.</returns>
	public override bool DoesEffectEnemies()
	{
		return true;
	}

	/// <summary>
	/// Return if the object is destroyed after it interacts.
	/// </summary>
	/// <returns>If the object is destroyed after it interacts.</returns>
	public override bool DoesDestroyInInteract()
	{
		return true;
	}

	/// <summary>
	/// Called when this object successfully interacts (collides) with an appropriate enemy or teammate of the player who owns this object.
	/// Or any boat if the object is owned by no-one.
	/// </summary>
	/// <param name="playerBoat">The GameObject boat of the player collided with.</param>
	/// <param name="manager">The player boat's status effect manager.</param>
	/// <param name="collision">Information about the collision that caused the interaction.</param>
	/// <param name="playerHealth">The health script of the player collided with.</param>
	public override void OnInteractWithPlayer(GameObject playerBoat, StatusEffectsManager manager, Collision collision, Health playerHealth)
    {
		return;
	}
}
