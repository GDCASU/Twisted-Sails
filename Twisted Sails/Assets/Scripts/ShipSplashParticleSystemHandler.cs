using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************** Ship Splash Particle System Handler *******************************
 * This script controls the Ship Trails and Ship Splash particle systems. The Ship Trails 
 * particle system emits particles in the world space using the ship's rotation and the Ship
 * Splash particle system scales in size based on the direction and speed of the ship. The splash
 * is larger when the ship is facing the direction it's moving and is also larger the closer the
 * ship's speed is to its maximum unboosted speed.
 * 
 * Author: Edward Borroughs
 * Date: April 19th, 2017
 */

public class ShipSplashParticleSystemHandler : MonoBehaviour {

    // speedCap is the speed at which the size of the splash particles will no longer increase.
    // The speed of the Bramble ship asymptotically approaches 3 with no speed boosts.
    // The speed of the Dragon ship asymptotically approaches 8 with no speed boosts.
    // All of these speeds were tested on 4/19/17, test them again before updating this value.

    public float speedCap;
    private float angle;
    private float angleScale;
    private float speed;
    private float speedScale;
    private float splashParticleScale;
    private float invokeDelay;
    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 direction;
    private Health healthScript;
    private ParticleSystem trails;
    private ParticleSystem splash;

	// Use this for initialization
	void Start () {
        healthScript = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        trails = transform.Find("Ship Trails").GetComponent<ParticleSystem>();
        splash = trails.transform.Find("Ship Splash").GetComponent<ParticleSystem>();
        invokeDelay = trails.startLifetime / trails.maxParticles;
        InvokeRepeating("DoEmit", 0.0f, invokeDelay);
	}
	
	// Update is called once per frame
	void Update () {
		//If ship is dead stop all particle emitters
        if (!IsInvoking("DoEmit"))
        {
            if (!healthScript.dead)
            {
                InvokeRepeating("DoEmit", 0.0f, invokeDelay);
            }
        }
        else if (healthScript.dead)
        {
            CancelInvoke();
        }
	}

    void FixedUpdate() {
        if (!healthScript.dead)
        {
            velocity = rb.velocity;
            direction = transform.forward;
            angle = Vector3.Angle(velocity, direction);
            angleScale = angle >= 90 ? 0 : (90 - angle) / 90;
            speed = velocity.magnitude;
            speedScale = speed >= speedCap ? 1 : speed / speedCap;
            splashParticleScale = angleScale * speedScale;
            splash.transform.localScale = new Vector3(splashParticleScale, splashParticleScale, splashParticleScale);
        }
    }

    private void DoEmit() {
        var trailsEmitParams = new ParticleSystem.EmitParams();
        trailsEmitParams.rotation3D = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        trails.Emit(trailsEmitParams, 1);
    }
}
