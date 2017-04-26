using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//Created by Ryan Black
//Spawns healthpacks around the map every x seconds.

public class HealthSpawner : NetworkBehaviour
{
    public GameObject healthPackPrefab;
    public float respawnTime = 30.0f;

    public override void OnStartServer()
    {
        //InvokeRepeating("SpawnHealthPack", respawnTime, respawnTime);
    }

    private void SpawnHealthPack()
    {
        GameObject healthPack = (GameObject)Instantiate(healthPackPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        NetworkServer.Spawn(healthPack);
    }
}