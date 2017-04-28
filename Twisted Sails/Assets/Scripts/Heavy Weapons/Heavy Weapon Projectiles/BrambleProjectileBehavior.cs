using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BrambleProjectileBehavior : InteractiveObject
{

    //Variables to edit
    public float travelTime = 8f;
    public float distFromBoat = 5f;
    public float orbitSpeed = 0.25f;
    public float speed = 1f;
    public float lifeTime = 10f;
    public int damageDealt;

    private bool goingOut = true; //Is projectile going towards target location
    private float newTime;
    private bool isDestroying = false;

    private GameObject ownerObject;

    StatusEffectsManager manager;

    // Use this for initialization
    private void Start () {
        if (MultiplayerManager.IsServer())
            ownerObject = NetworkServer.FindLocalObject(owner);
        else
            ownerObject = ClientScene.FindLocalObject(owner);

        Invoke("KillMyself", lifeTime);
	}

    // Update is called once per frame
    private void Update () {
        newTime += Time.deltaTime*speed;
        Vector3 origin = ownerObject.transform.position + transform.up;
        if (newTime > travelTime)
            goingOut = false;
        if (goingOut)
            transform.position = origin + transform.forward * (1 + distFromBoat * (newTime / travelTime));
        else if (isDestroying)
        {
            transform.position = origin + transform.forward * (1 + distFromBoat * (1 - newTime / travelTime));
            if (newTime > travelTime && GetComponent<Collider>().enabled)
                DestroyPreserveParticles();

        } else
            transform.position = origin + transform.forward * (1 + distFromBoat);
        transform.Rotate(Vector3.up, orbitSpeed * 360 * Time.deltaTime);
        
    }
    //Destroys the projectile
    private void ReturnToShipPos(){
		goingOut = false;
	}
    private void KillMyself(){
        isDestroying = true;
        newTime = 0;
    }

    //Detects collison with a player
    //Causes the projectile to go back to position if collide with non-player gameobject
    private void OnTriggerEnter (Collider other) {

        if (other.gameObject.tag != "Player" && !other.isTrigger)
        {
            if (other.gameObject.tag == "Cannonball")
            {
                if (other.GetComponent<InteractiveObject>().owner != owner)
                    Destroy(other.gameObject);
            }
            else
            {
                //Debug.Log(other.gameObject.name);
                DestroyPreserveParticles();
            }
        }
    }

    public override void OnInteractWithPlayerTrigger(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collider collider)
    {
        base.OnInteractWithPlayerTrigger(playerHealth, playerBoat, manager, collider);

        int healthChange = -damageDealt;

        playerHealth.ChangeHealth(healthChange, owner);
        DestroyPreserveParticles();
    }

    public override bool DoesDestroyInInteract()
    {
        return false;
    }

    private void DestroyPreserveParticles()
    {
        //Debug.Log("Destroying bramble hw");
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            if (r.GetType() != typeof(ParticleSystemRenderer))
                r.enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.AddComponent<ParticleSystemAutoDestroy>();
        GetComponent<ParticleSystem>().Stop();
    }


}
