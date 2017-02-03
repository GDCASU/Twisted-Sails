using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StatusEffectsManager : MonoBehaviour
{
	public readonly List<StatusEffect> activeEffects = new List<StatusEffect>();
	private GameObject playerBoat;

	private void Start()
	{
		playerBoat = this.gameObject;
    }

	private void Update()
	{
		for(int i = 0; i< activeEffects.Count; i++)
		{
			activeEffects[i].UpdateEffect(playerBoat);
			if (activeEffects[i].GetIsFinished())
			{
				activeEffects.RemoveAt(i);
				i--;
			}
		}
	}

	public void AddNewEffect(StatusEffect effect)
	{
		int currentStacksActive;
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

	public T RemoveEffectOfTypeFromList<T>() where T : StatusEffect
	{
		for (int i = 0; i < activeEffects.Count;i++)
		{
			if (activeEffects[i] is T)
			{
				activeEffects.RemoveAt(i);
			}
		}
		return default(T);
	}

	public StatusEffect RemoveSameTypeEffectsFromList(StatusEffect effect)
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
		return null;
	}
}
