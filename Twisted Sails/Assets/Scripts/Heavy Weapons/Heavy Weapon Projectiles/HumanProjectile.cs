﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HumanProjectile : InteractiveObject {

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

        int healthChange = -damageDealt;

        //if this object is on the side of the player who owns this object
        //send out the command to change the players health
        //setting the source of the health change to be the owner of this cannonball
        if(isServer && playerHealth.team != NetworkServer.FindLocalObject(owner).GetComponent<Health>().team)
            playerHealth.ChangeHealth(healthChange, owner);
        DestroyPreserveParticles();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Player" && other.gameObject.GetComponent<HumanProjectile>() == null)
            DestroyPreserveParticles();
    }

    public override bool DoesDestroyInInteract()
    {
        return false;
    }

    public override bool DoesEffectTeammates()
    {
        return true;
    }

    private void DestroyPreserveParticles()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            if(r.GetType() != typeof(ParticleSystemRenderer))
                r.enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.AddComponent<ParticleSystemAutoDestroy>();
        GetComponent<ParticleSystem>().Stop();
    }

}
