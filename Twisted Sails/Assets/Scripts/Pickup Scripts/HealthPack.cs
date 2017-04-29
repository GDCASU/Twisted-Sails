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
    public Material onMatRed;
    public Material offMatRed;
    public Material onMatWhite;
    public Material offMatWhite;

    private void Start()
    {
        Material[] materials = packMesh.materials;
        materials[0] = offMatWhite;
        materials[1] = offMatRed;
        packMesh.materials = materials;
        packCollider.enabled = false;
        if(isServer)
            InvokeRepeating("Respawn", packRespawnTime, packRespawnTime);
    }

    void Respawn()
	{
        RpcRespawn();
	}

    [ClientRpc]
    void RpcRespawn()
    {
        Material[] materials = packMesh.materials;
        materials[0] = onMatWhite;
        materials[1] = onMatRed;
        packMesh.materials = materials;
        packCollider.enabled = true;
    }

	public override void OnInteractWithPlayerTrigger(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collider collider)
	{
		//send out the command to change the players health
		//setting the source of the healthpack to nothing, since no player is responsible
		if (isServer)
		{
            if (playerHealth.health >= 100) return;

            //notifies the player events system that the player who interacted with this object picked up a health pack (this object)
            //also sets isHealthPack to true, since this is a health pack
            Player.ActivateEventPlayerPickup(MultiplayerManager.FindPlayer(playerBoat.GetComponent<NetworkIdentity>().netId), true);
            playerHealth.ChangeHealth(healAmount, NetworkInstanceId.Invalid);
            RpcConsumePack(playerBoat.GetComponent<NetworkIdentity>().netId);
		}
	}

    [ClientRpc]
    public void RpcConsumePack(NetworkInstanceId player)
    {
        GameObject playerBoat = ClientScene.FindLocalObject(player);
        Health playerHealth = playerBoat.GetComponent<Health>();
        //play sounds and send command for ammo
        if (MultiplayerManager.GetLocalPlayer() != null && MultiplayerManager.GetLocalPlayer().objectId == playerBoat.GetComponent<NetworkIdentity>().netId)
        {
            playerBoat.transform.Find("ShipSounds").Find("HealthPickupVO").GetComponent<AudioSource>().Play();
            //Debug.Log(MultiplayerManager.GetLocalPlayer().name);
        }
        Instantiate(playerHealth.powerupParticle, playerBoat.transform).transform.localPosition = Vector3.zero;

        playerBoat.transform.Find("ShipSounds").Find("HealthPickup").GetComponent<AudioSource>().Play();

        Material[] materials = packMesh.materials;
        materials[0] = offMatWhite;
        materials[1] = offMatRed;
        packMesh.materials = materials;
        packCollider.enabled = false;
    }

    public override bool DoesDestroyInInteract()
	{
		return false;
	}
}