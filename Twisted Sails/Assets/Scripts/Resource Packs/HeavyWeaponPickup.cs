using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyWeaponPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(collision.gameObject.GetComponent<HeavyWeapon>().AmmoCount<= collision.gameObject.GetComponent<HeavyWeapon>().ammoCapacity)
            collision.gameObject.GetComponent<HeavyWeapon>().AddAmmo(1);
            Destroy(gameObject);
        }


    }

}
