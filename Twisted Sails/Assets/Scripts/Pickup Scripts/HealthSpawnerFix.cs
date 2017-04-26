using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//Created by Ryan Black
//Modified by Sean McGonegle
//Spawns healthpacks around the map every x seconds.

public class HealthSpawnerFix : NetworkBehaviour
{
    public GameObject healthPackPrefab;
    public float respawnTime;

    private GameObject spawnedPack;

    public override void OnStartServer()
    {
        //InvokeRepeating("SpawnHealthPack", respawnTime, respawnTime);
    }

    private void SpawnHealthPack()
    {
        //Debug.Log(spawnedPack);
        if (spawnedPack == null)
        {
            spawnedPack = Instantiate(healthPackPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
            NetworkServer.Spawn(spawnedPack);
        }
    }
}