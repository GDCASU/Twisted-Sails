using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonProjectileBehavior : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Dragon Projectile: Collide");

        if (other.gameObject.tag == "Player")
            Debug.Log("Bramble Projectile: I Hit a player");

        Destroy (this.gameObject);
    }
}
