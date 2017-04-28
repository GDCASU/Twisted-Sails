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
    public float explodeSoundTime;
    public GameObject explodeParticle;

    private void Start()
    {
        Invoke("StopSound", explodeSoundTime);
    }

    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        base.OnInteractWithPlayer(playerHealth, playerBoat, manager, collision);

        int healthChange = -damageDealt;

        //if this object is on the side of the player who owns this object
        //send out the command to change the players health
        //setting the source of the health change to be the owner of this cannonball
        if(isServer)
            playerHealth.ChangeHealth(healthChange, owner);

        DestroyPreserveParticles();
    }

    private void OnCollisionEnter(Collision other)
    {
        DestroyPreserveParticles();
    }

    public override bool DoesDestroyInInteract()
    {
        return false;
    }

    private void StopSound()
    {
        GetComponent<AudioSource>().Stop();
    }

    private void DestroyPreserveParticles()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            if (r.GetType() != typeof(ParticleSystemRenderer))
                r.enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Invoke("DestroySelf", 1.5f);
        //gameObject.AddComponent<ParticleSystemAutoDestroy>();
        GetComponentInChildren<ParticleSystem>().Stop();
        StopSound();
        Instantiate(explodeParticle, transform.position, transform.rotation);
        GetComponent<AudioSource>().time = explodeSoundTime;
        GetComponent<AudioSource>().Play();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}