using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshipHeavyProjectileBeahviorFix : InteractiveObject
{

    Vector3 startingLocation;
    Vector3 currentLocation;
    float distanceFromOrigin;
    public float maxDistance = 1f;
    public float explodingScale; //How fast it explodes
    public int damageDealt;
    public AudioClip explodeSound;

    private bool isExploding;
    private float explodingTimer = 0;

    StatusEffectsManager manager;

    // Use this for initialization
    void Start()
    {
        startingLocation = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentLocation = transform.position;
        distanceFromOrigin = Mathf.Abs(Vector3.Distance(currentLocation, startingLocation));

        if (distanceFromOrigin >= maxDistance && isExploding == false)
            ExplodeObject();

        if (isExploding)
        {
            transform.localScale += new Vector3(.5f, .5f, .5f) * explodingScale;

            if (explodingTimer > 0)
                explodingTimer -= Time.deltaTime;
            if (explodingTimer <= 0)
                DestroyPreserveParticles();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isExploding)
            ExplodeObject();
    }


    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        base.OnInteractWithPlayer(playerHealth, playerBoat, manager, collision);
        int healthChange = -damageDealt;

        playerHealth.ChangeHealth(healthChange/2, owner);

    }

    public override void OnInteractWithPlayerTrigger(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collider collider)
    {
        base.OnInteractWithPlayerTrigger(playerHealth, playerBoat, manager, collider);
        int healthChange = -damageDealt;

        playerHealth.ChangeHealth(healthChange, owner);
    }

    public override bool DoesDestroyInInteract()
    {
        return false;
    }

    private void ExplodeObject()
    {
        isExploding = true;
        explodingTimer = 1;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<AudioSource>().PlayOneShot(explodeSound,2);
    }

    private void DestroyPreserveParticles()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            if (r.GetType() != typeof(ParticleSystemRenderer))
                r.enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.AddComponent<ParticleSystemAutoDestroy>();
        GetComponent<ParticleSystem>().Stop();
    }
}
