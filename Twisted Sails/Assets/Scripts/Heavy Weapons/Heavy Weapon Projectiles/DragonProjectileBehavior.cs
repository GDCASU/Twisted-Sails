using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonProjectileBehavior : InteractiveObject {
    
    /// <summary>
    /// Collision for the projectile is handled here in a seperate script which is attactched to the 
    /// projectile itself. Checks to see if collision is with an enemy, which it will then deal damage to.
    /// Destroys itself after.
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Dragon Projectile: Collide");

        if (other.gameObject.tag == "Player")
            Debug.Log("Dragon Projectile: I Hit a player");

        Destroy(gameObject);
    }
}
