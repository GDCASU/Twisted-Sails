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
    
    StatusEffectsManager manager;
    public int damageDealt;

    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        base.OnInteractWithPlayer(playerHealth, playerBoat, manager, collision);
        playerHealth.health -= damageDealt;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
            OnInteractWithPlayer(other.gameObject.GetComponent<Health>(), other.gameObject, manager, other);

        Destroy(gameObject);
    }
}