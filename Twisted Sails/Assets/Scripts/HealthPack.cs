using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// Developer:   Kyle Chapman
// Date:        2/14/2017
// Description: Completely refactored for use with the InteractiveObject system. On interaction with a player, gives them health and notifies the events system of being picked up.

public class HealthPack : InteractiveObject
{
	public float healAmount = 50f;
	public float packRespawnTime = 10f;
	public MeshRenderer packMesh;
	public Collider packCollider;

	private void Start()
	{
		if (packMesh == null)
		{
			packMesh = GetComponent<MeshRenderer>();
			packCollider = GetComponent<Collider>();
        }
	}

	IEnumerator MyCoroutine()
	{
		yield return new WaitForSeconds (packRespawnTime);
		packMesh.enabled = true;
		packCollider.enabled = true;
	}

	public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
	{
		//notifies the player events system that the player who interacted with this object picked up a health pack (this object)
		//also sets isHealthPack to true, since this is a health pack
		Player.ActivateEventPlayerPickup(MultiplayerManager.FindPlayer(playerBoat.GetComponent<NetworkIdentity>().netId), true);

		//send out the command to change the players health
		//setting the source of the healthpack to nothing, since no player is responsible
		if (isServer)
		{
			playerHealth.CmdChangeHealth(healAmount, NetworkInstanceId.Invalid);
		}

		packMesh.enabled = false;
		packCollider.enabled = false;
		StartCoroutine(MyCoroutine());
	}

	public override bool DoesDestroyInInteract()
	{
		return false;
	}
}