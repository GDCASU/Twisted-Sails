using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Developer:   Kyle Chapman
// Date:        2/14/2017
// Description: A script used on prefab objects that are owned by players and are
//					able to interact with other players when they touch them (deal damage, provide status effects, etc.)

public class InteractiveObject : NetworkBehaviour
{
	[SyncVar]
	public NetworkInstanceId owner;

	public virtual bool DoesEffectTeammates()
	{
		return false;
	}

	public virtual bool DoesEffectEnemies()
	{
		return true;
	}

	public virtual bool DoesDestroyInInteract()
	{
		return true;
	}

	public virtual void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
	{
		return;
	}

	public virtual void OnInteractWithPlayerTrigger(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collider collider)
	{
		return;
	}
}
