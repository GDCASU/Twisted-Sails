using UnityEngine;
using System.Collections;

//  Programmer:     Kyle Chapman
//  Date:           2-06-17
//  Description:    
//		An abstract parent class for status effect classes to derive from for use in the status effects system
//		managed by an instance of the StatusEffectsManager class on a particular boat.

public abstract class StatusEffect
{
	protected float remainingDuration;
	protected bool isFinished;

	/// <summary>
	/// Get the starting duration in seconds that the effect will last on the boat.
	/// By default the duration is -1. Negative values cause the effect to never expire.
	/// Should be overridden to any nonzero value.
	/// </summary>
	/// <returns>The starting duration in seconds that the effect will last on the boat.</returns>
	public virtual float GetStartingDuration()
	{
		return -1f;
	}

	/// <summary>
	/// Get if the status effect believes itself to be expired.
	/// By default, returns true if the remaining duration went from 0 or above to less than zero.
	/// </summary>
	/// <returns>Whether the status effect is expired or not.</returns>
	public virtual bool GetIsFinished()
	{
		return isFinished;
	}

	/// <summary>
	/// Get the max number of stacks of this type that are allowed to exist at one time on a boat.
	/// The default value is 1, and it should be overridden to any value 1 or greater.
	/// </summary>
	/// <returns>The max number of stacks of this type that are allowed to exist at one time on a boat.</returns>
	public virtual int GetMaxStacks()
	{
		return 1;
	}

	/// <summary>
	/// Get if the status effect overrides existing instances of the effect on the boat when it is applied.
	/// If no, a new instance of the effect will not be applied if the maximum amount of stacks of the effect are already on the boat.
	/// If yes, no matter how many stacks of the effect are currently on the boat, all of them will be removed when a new instance is put on.
	/// Returns false by default.
	/// </summary>
	/// <returns></returns>
	public virtual bool DoesOverrideExisting()
	{
		return false;
	}

	/// <summary>
	/// Tells the status effect to begin its effect on the boat.
	/// This is where the code that begins the effect is put, with reversing code on the end effect method.
	/// Also by default starts the duration timer.
	/// </summary>
	/// <param name="playerBoat">The instance of the boat onto which this effect is being applied.</param>
	public virtual void StartEffect(GameObject playerBoat)
	{
		remainingDuration = GetStartingDuration();
		isFinished = false;
    }

	/// <summary>
	/// Updates the effect.
	/// By default, updates the duration timer and determins if the effect has expired.
	/// </summary>
	/// <param name="playerBoat">The instance of the boat onto which this effect is being applied.</param>
	public virtual void UpdateEffect(GameObject playerBoat)
	{
		if (remainingDuration >= 0)
		{
			remainingDuration -= Time.deltaTime;
			if (remainingDuration <= 0)
			{
				isFinished = true;
			}
		}
		
	}

	/// <summary>
	/// Tells the status effect to end its effect on the boat.
	/// This is where the code that ends the effect is put, reversing the starting code on the start effect method.\
	/// By default, does nothing.
	/// </summary>
	/// <param name="playerBoat">The instance of the boat onto which this effect is being applied.</param>
	public virtual void EndEffect(GameObject playerBoat)
	{
		
	}
}
