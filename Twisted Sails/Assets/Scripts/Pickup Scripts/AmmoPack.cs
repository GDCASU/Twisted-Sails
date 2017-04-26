using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AmmoPack : InteractiveObject
{
    public int ammoAmmount = 1;
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

    public override void OnInteractWithPlayerTrigger(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collider collider)
    {
        //notifies the player events system that the player who interacted with this object picked up a health pack (this object)
        //also sets isHealthPack to true, since this is a health pack
        Player.ActivateEventPlayerPickup(MultiplayerManager.FindPlayer(playerBoat.GetComponent<NetworkIdentity>().netId), true);
        
        //play sounds and send command for ammo
        if (MultiplayerManager.GetLocalPlayer() != null && MultiplayerManager.GetLocalPlayer().objectId == playerBoat.GetComponent<NetworkIdentity>().netId)
        {
            playerBoat.transform.Find("ShipSounds").Find("AmmoPickupVO").GetComponent<AudioSource>().Play();
            //Debug.Log(MultiplayerManager.GetLocalPlayer().name);
            
        }

        playerBoat.GetComponent<HeavyWeapon>().AddAmmo(ammoAmmount);

        Instantiate(playerHealth.powerupParticle, playerBoat.transform).transform.localPosition = Vector3.zero;

        playerBoat.transform.Find("ShipSounds").Find("AmmoPickup").GetComponent<AudioSource>().Play();

        Destroy(gameObject);
    }

}
