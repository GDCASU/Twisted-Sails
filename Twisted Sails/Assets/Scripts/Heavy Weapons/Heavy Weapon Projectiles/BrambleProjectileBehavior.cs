using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BrambleProjectileBehavior : InteractiveObject
{

    //Variables to edit
    public float travelTime = 8f; //Amount of time it takes for projectile to hit the farthest point
    public float speed = 1f;
    public int damageDealt;

    private Vector3 brambleTarget; //Farthest point
    private Vector3 brambleStart; //Point projectile was shot from
    private bool goingOut = true; //Is projectile going towards target location
    private float totalTime; //Amount of time it takes for projectile to complete journey
    private float distanceToTarget;
    private float newTime;

    private GameObject ownerObject;

    StatusEffectsManager manager;

    // Use this for initialization
    private void Start () {
		//Debug.Log ("Print Test");
		totalTime = travelTime * 2;
		Invoke ("ReturnToShipPos", travelTime);
		Invoke ("KillMyself", totalTime);
		brambleStart = GameObject.Find("BrambleShipPlayer(Clone)").transform.position;
		brambleTarget = GameObject.Find("BrambleShipPlayer(Clone)/HWtarget").transform.position;
		speed = distanceToTarget / travelTime;

        if (MultiplayerManager.IsServer())
            ownerObject = NetworkServer.FindLocalObject(owner);
        else
            ownerObject = ClientScene.FindLocalObject(owner);
		//Debug.Log(distanceToTarget);
	
	}

    // Update is called once per frame
    private void Update () {
        if (goingOut)
            newTime += Time.deltaTime;
        else
            newTime -= Time.deltaTime;
        transform.position = ownerObject.transform.position + transform.forward * (1 + 14 * (newTime / travelTime));
    }
    //Destroys the projectile
    private void ReturnToShipPos(){
		goingOut = false;
	}
    private void KillMyself(){
		Destroy (gameObject);
	}

    //Detects collison with a player
    //Causes the projectile to go back to position if collide with non-player gameobject
    private void OnTriggerEnter (Collider other) {
		//Debug.Log ("Bramble Projectile: Collide");
        //Debug Code - Debug.Log(other.gameObject.layer);

        if (other.gameObject.tag != "Player" && other.GetComponent<BrambleProjectileBehavior>() == null)
            Destroy(gameObject);
	}

    public override void OnInteractWithPlayerTrigger(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collider collider)
    {
        base.OnInteractWithPlayerTrigger(playerHealth, playerBoat, manager, collider);

        int healthChange = -damageDealt;
        //if this object is on the side of the player who owns this object
        //send out the command to change the players health
        //setting the source of the health change to be the owner of this cannonball
        playerHealth.ChangeHealth(healthChange, owner);
        Destroy(gameObject);
    }

}
