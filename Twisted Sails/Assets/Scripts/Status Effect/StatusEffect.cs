using UnityEngine;
using System.Collections;

public abstract class StatusEffect
{
	private float remainingDuration;

	public float GetDuration()
	{
		return -1f;
	}

	public bool GetIsFinished()
	{
		return remainingDuration <= 0;
	}

	public int GetMaxStacks()
	{
		return 1;
	}

	public bool DoesOverrideExisting()
	{
		return false;
	}

	public void StartEffect(GameObject playerBoat)
	{
		remainingDuration = GetDuration();
    }

	public void UpdateEffect(GameObject playerBoat)
	{
		remainingDuration -= Time.deltaTime;
	}

	public void EndEffect(GameObject playerBoat)
	{

	}
}
