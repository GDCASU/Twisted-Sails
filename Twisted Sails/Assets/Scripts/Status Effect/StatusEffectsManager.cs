using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//  Programmer:     Kyle Chapman
//  Date:           2-06-17
//  Description:    Manages a list of and the condition of Status Effects on a boat.

public class StatusEffectsManager : MonoBehaviour
{
	//the current list of effects active on the boat
	public readonly List<StatusEffect> activeEffects = new List<StatusEffect>();

	//the boat to which the effects are applied
	private GameObject playerBoat;

	private void Start()
	{
		playerBoat = this.gameObject;
    }

	private void Update()
	{
		//for each effect, give it an update tick
		//then ask if it is complete, if it is, tell it to end its effect
		//and remove it from the list
		for(int i = 0; i< activeEffects.Count; i++)
		{
			activeEffects[i].UpdateEffect(playerBoat);
			if (activeEffects[i].GetIsFinished())
			{
				Debug.Log("Ending an effect.");
				activeEffects[i].EndEffect(playerBoat);
				activeEffects.RemoveAt(i);
				i--;
			}
		}
	}

	/// <summary>
	/// Add a new status effect to the list of active effects.
	/// Managing for stacking properties, override properties
	/// Then tells the effect to start itself.
	/// </summary>
	/// <param name="effect">The effect obect to begin on the boat.</param>
	public void AddNewEffect(StatusEffect effect)
	{
		int currentStacksActive;

		//if the effect is not currently in the list
		//or if the effect has less than its maximum allowed amount of stacks in the list
		//or if the new effect always overrides existing effects no matter what
		//add the effect to the list
		if (!IsSameTypeEffectInList(effect, out currentStacksActive) || currentStacksActive < effect.GetMaxStacks() || effect.DoesOverrideExisting())
		{
			if (effect.DoesOverrideExisting())
			{
				Debug.Log("Overrideing existing status effects of type " + effect.GetType());
				RemoveSameTypeEffectsFromList(effect);
			}
			Debug.Log("Adding new status effect of type " + effect.GetType());
			effect.StartEffect(playerBoat);
			activeEffects.Add(effect);
		}
		else
		{
			Debug.Log("Could not add new status effect of type " + effect.GetType() + " since too many stacks already active");
		}
    }

	/// <summary>
	/// Determines if an effect that has the same type as the given object is in the list.
	/// If it is, it returns true and assigns the out parameter amount to the
	/// number of effects from of that type that are in the list.
	/// </summary>
	/// <param name="effect">The StatusEffect object whose subclass type is to be looked for in the list.</param>
	/// <param name="amount">The out parameter to assign the number of objects found of the desired type.</param>
	/// <returns>True if at least one matching object was found, false otherwise.</returns>
	public bool IsSameTypeEffectInList(StatusEffect effect, out int amount)
	{
		Type effectsType = effect.GetType();
		amount = 0;
		foreach (StatusEffect e in activeEffects)
		{
			if (e.GetType() == effectsType)
			{
				amount++;
			}
		}
		return amount > 0;
	}

	/// <summary>
	/// Removes all status effect objects from the list whose type is the
	/// same as the type of the given status effect object.
	/// </summary>
	/// <param name="effect">The object that is to be matched to find objects in the list to remove.</param>
	public void RemoveSameTypeEffectsFromList(StatusEffect effect)
	{
		Type effectsType = effect.GetType();
		Debug.Log("Removing type " + effectsType + " from list while this many effects in list: " + activeEffects.Count);
		for (int i = 0; i < activeEffects.Count; i++)
		{
			if (activeEffects[i].GetType() == effectsType)
			{
				Debug.Log("Removing item from list of type " + activeEffects[i].GetType());
				activeEffects[i].EndEffect(playerBoat);
				activeEffects.RemoveAt(i);
				i--;
			}
		}
	}
}
