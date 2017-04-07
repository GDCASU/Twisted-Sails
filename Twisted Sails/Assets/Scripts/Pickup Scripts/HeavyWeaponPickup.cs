using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HeavyWeaponPickup : InteractiveObject
{
    public float ammoAmmount = 1f;

    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        //notifies the player events system that the player who interacted with this object picked up a health pack (this object)
        //also sets isHealthPack to true, since this is a health pack
        Player.ActivateEventPlayerPickup(MultiplayerManager.FindPlayer(playerBoat.GetComponent<NetworkIdentity>().netId), true);

        //send out the command to change the players health
        //setting the source of the healthpack to nothing, since no player is responsible
        if (isServer)
        {
            playerHealth.ChangeHealth(ammoAmmount, NetworkInstanceId.Invalid);
            Destroy(gameObject);
        }

    }

}
