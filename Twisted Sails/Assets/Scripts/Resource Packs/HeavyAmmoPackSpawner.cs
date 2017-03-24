using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAmmoPackSpawner : MonoBehaviour
{
    public GameObject ammoPack;
    private GameObject createdPack;
    public float timer;
    public float timerLength;
    private bool spawnedPack = false;
    private void Start()
    {
       timer = timerLength;
    }
    private void Update()
    {
        if (spawnedPack == false)
        {
            if (timer <= 0)
            {
                createdPack=Instantiate(ammoPack,Vector3.zero, transform.rotation);
                createdPack.transform = transform;
                spawnedPack = true;
            }
            else
                timer -= Time.deltaTime;
        }
        else
        {
            if(createdPack==null)
            {
                timer = timerLength;
                spawnedPack = false;
            }
        }
    }






}