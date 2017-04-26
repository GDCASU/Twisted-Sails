using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//Spawns Ammo Packs around the map every x seconds.

public class HeavyAmmoPackSpawner : NetworkBehaviour
{
    public GameObject ammoPackPrefab;
    public float respawnTime = 30.0f;

    public override void OnStartServer()
    {
        //InvokeRepeating("SpawnAmmoPack", respawnTime, respawnTime);
    }

    private void SpawnAmmoPack()
    {
        GameObject ammoPack = (GameObject)Instantiate(ammoPackPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        NetworkServer.Spawn(ammoPack);
    }
}