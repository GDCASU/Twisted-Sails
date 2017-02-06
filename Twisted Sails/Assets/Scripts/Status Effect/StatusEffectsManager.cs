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
		if (!IsEffectTypeInList<StatusEffect>(out currentStacksActive) || currentStacksActive < effect.GetMaxStacks() || effect.DoesOverrideExisting())
		{
			if (effect.DoesOverrideExisting())
			{
                RemoveSameTypeEffectsFromList(effect);
            }
			effect.StartEffect(playerBoat);
			activeEffects.Add(effect);
        }
    }

	/// <summary>
	/// Determines if an effect of the given genericized type is in the list.
	/// If it is, it returns true and assigns the out parameter amount to the
	/// number of effects from of that type that are in the list.
	/// </summary>
	/// <typeparam name="T">The type of the status effect subclass to look for the in the list.</typeparam>
	/// <param name="amount">The out parameter to assign the number of objects found of the desired type.</param>
	/// <returns>True if at least one matching object was found, false otherwise.</returns>
	public bool IsEffectTypeInList<T>(out int amount) where T : StatusEffect
	{
		amount = 0;
        foreach (StatusEffect e in activeEffects)
		{
			if (e is T)
			{
				amount++;
            }
		}
		return amount > 0;
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
	/// Removes all status effect objects from the list that match the given genericed type
	/// which is a subclass type of Status Effect.
	/// </summary>
	/// <typeparam name="T">The type of the status effect subclass whose objects are to be removed from the list.</typeparam>
	public void RemoveEffectOfTypeFromList<T>() where T : StatusEffect
	{
		for (int i = 0; i < activeEffects.Count;i++)
		{
			if (activeEffects[i] is T)
			{
				activeEffects.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// Removes all status effect objects from the list whose type is the
	/// same as the type of the given status effect object.
	/// </summary>
	/// <param name="effect">The object that is to be matched to find objects in the list to remove.</param>
	public void RemoveSameTypeEffectsFromList(StatusEffect effect)
	{
		Type effectsType = effect.GetType();
		for (int i = 0; i < activeEffects.Count; i++)
		{
			if (activeEffects[i].GetType() == effectsType.GetType())
			{
				activeEffects[i].EndEffect(playerBoat);
				activeEffects.RemoveAt(i);
				i--;
			}
		}
	}
}
