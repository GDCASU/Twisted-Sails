using UnityEngine;
using System.Collections;

public class spawnPowerUps : MonoBehaviour {

    public Transform[] PowerUpSpawnPoints;
    public float powerUpSpawnTime = 45.0f;


    public GameObject[] PowerUps;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnPowerUps", powerUpSpawnTime, powerUpSpawnTime);
	}

    void SpawnPowerUps()
    {
        int spawnIndex = Random.Range(0, PowerUpSpawnPoints.Length);
        int objectIndex = Random.Range(0, PowerUps.Length);

        Instantiate(PowerUps[objectIndex], PowerUpSpawnPoints[spawnIndex].position, PowerUpSpawnPoints[spawnIndex].rotation);
    }
}
