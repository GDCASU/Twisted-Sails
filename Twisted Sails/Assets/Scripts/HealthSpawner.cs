using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HealthSpawner : NetworkBehaviour
{

    public GameObject healthPackPrefab;
    public float respawnTime = 30.0f;

    public override void OnStartServer()
    {
        //Instantiate(healthPackPrefab, transform.position, transform.rotation);
        //var healthPack = (GameObject)Instantiate(healthPackPrefab, transform.position, new Quaternion(-90.0f, 0.0f, 0.0f, 0.0f));
        //NetworkServer.Spawn(healthPack);
        InvokeRepeating("SpawnHealthPack", respawnTime, respawnTime);
    }

    void SpawnHealthPack()
    {
        //Instantiate(healthPackPrefab, transform.position, transform.rotation);
        var healthPack = (GameObject)Instantiate(healthPackPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        NetworkServer.Spawn(healthPack);
    }
}