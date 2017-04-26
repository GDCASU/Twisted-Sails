using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//Created by Sean McGonegle
//Spawns healthpacks around the map every x seconds.

public class AmmoSpawner : NetworkBehaviour
{
    public GameObject ammoPackPrefab;
    public float respawnTime;

    private GameObject spawnedPack;

    public override void OnStartServer()
    {
        //InvokeRepeating("SpawnAmmoPack", respawnTime, respawnTime);
    }

    private void SpawnAmmoPack()
    {
        Debug.Log(spawnedPack);
        if (spawnedPack == null)
        {
            spawnedPack = Instantiate(ammoPackPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
            NetworkServer.Spawn(spawnedPack);
        }
    }
}