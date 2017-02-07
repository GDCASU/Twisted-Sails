using UnityEngine;
using System.Collections;

/**
	[Template Class Header]
	Template Makers:	Kyle Chapman
	Date:				2/07/2017

    Duplicate this script and rename the duplicate.
	Then, place a StatusEffectManager on each boat needing to use the status effect.
	Then, during gameplay, create an instance of this class and tell the StatusEffectManager
	about the instance. The manager will control the effect behavior.

	If need help using this template, contact the programmer's above.
**/

public class StatusEffectTemplate : StatusEffect
{
	/// <summary>
	/// Get the starting duration in seconds that the effect will last on the boat.
	/// By default the duration is -1. Negative values cause the effect to never expire.
	/// Should be overridden to any nonzero value.
	/// </summary>
	/// <returns>The starting duration in seconds that the effect will last on the boat.</returns>
	override public float GetStartingDuration()
	{
		// ADD YOUR CODE HERE
		return base.GetStartingDuration();
	}

	/// <summary>
	/// Get if the status effect believes itself to be expired.
	/// By default, returns true if the remaining duration went from 0 or above to less than zero.
	/// </summary>
	/// <returns>Whether the status effect is expired or not.</returns>
	override public bool GetIsFinished()
	{
		// ADD YOUR CODE HERE
		
		// REMOVE THIS LINE TO REPLACE DEFAULT BEHAVIOR
		return base.GetIsFinished();
	}

	/// <summary>
	/// Get the max number of stacks of this type that are allowed to exist at one time on a boat.
	/// The default value is 1, and it should be overridden to any value 1 or greater.
	/// </summary>
	/// <returns>The max number of stacks of this type that are allowed to exist at one time on a boat.</returns>
	override public int GetMaxStacks()
	{
		// ADD YOUR CODE HERE

		// REMOVE THIS LINE TO REPLACE DEFAULT BEHAVIOR
		return base.GetMaxStacks();
	}

	/// <summary>
	/// Get if the status effect overrides existing instances of the effect on the boat when it is applied.
	/// If no, a new instance of the effect will not be applied if the maximum amount of stacks of the effect are already on the boat.
	/// If yes, no matter how many stacks of the effect are currently on the boat, all of them will be removed when a new instance is put on.
	/// Returns false by default.
	/// </summary>
	/// <returns></returns>
	override public bool DoesOverrideExisting()
	{
		// ADD YOUR CODE HERE

		// REMOVE THIS LINE TO REPLACE DEFAULT BEHAVIOR
		return base.DoesOverrideExisting();
	}

	/// <summary>
	/// Tells the status effect to begin its effect on the boat.
	/// This is where the code that begins the effect is put, with reversing code on the end effect method.
	/// By default, starts the duration timer.
	/// </summary>
	/// <param name="playerBoat">The instance of the boat onto which this effect is being applied.</param>
	override public void StartEffect(GameObject playerBoat)
	{
		// REMOVE THIS LINE TO REPLACE DEFAULT BEHAVIOR
		base.StartEffect(playerBoat);

		// ADD YOUR CODE HERE
	}

	/// <summary>
	/// Updates the effect.
	/// By default, updates the duration timer and determins if the effect has expired.
	/// </summary>
	/// <param name="playerBoat">The instance of the boat onto which this effect is being applied.</param>
	override public void UpdateEffect(GameObject playerBoat)
	{
		// REMOVE THIS LINE TO REPLACE DEFAULT BEHAVIOR
		base.UpdateEffect(playerBoat);

		// ADD YOUR CODE HERE
	}

	/// <summary>
	/// Tells the status effect to end its effect on the boat.
	/// This is where the code that ends the effect is put, reversing the starting code on the start effect method.\
	/// By default, does nothing.
	/// </summary>
	/// <param name="playerBoat">The instance of the boat onto which this effect is being applied.</param>
	override public void EndEffect(GameObject playerBoat)
	{
		// REMOVE THIS LINE TO REPLACE DEFAULT BEHAVIOR
		base.EndEffect(playerBoat);

		// ADD YOUR CODE HERE
	}
}
