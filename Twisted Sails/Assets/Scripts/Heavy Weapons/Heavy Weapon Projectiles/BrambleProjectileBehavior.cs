using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    StatusEffectsManager manager;

    // Use this for initialization
    private void Start () {
		//Debug.Log ("Print Test");
		totalTime = travelTime * 2;
		Invoke ("ReturnToShipPos", travelTime);
		Invoke ("KillMyself", totalTime);
		brambleStart = GameObject.Find("BrambleShipPlayer(Clone)").transform.position;
		brambleTarget = GameObject.Find("BrambleShipPlayer(Clone)/HWtarget").transform.position;
		distanceToTarget = Mathf.Abs(Vector3.Distance(brambleStart, brambleTarget));
		speed = distanceToTarget / travelTime;
		//Debug.Log(distanceToTarget);
	
	}

    // Update is called once per frame
    private void Update () {
		if(goingOut) 
			transform.position = Vector3.MoveTowards (this.transform.position, brambleTarget, speed * Time.deltaTime);
		else
			transform.position = Vector3.MoveTowards (this.transform.position, brambleStart, speed * Time.deltaTime);
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
    private void OnCollisionEnter (Collision other) {
		Debug.Log ("Bramble Projectile: Collide");
		//Debug Code - Debug.Log(other.gameObject.layer);

		if (other.gameObject.tag != "Player")
            goingOut = false;

        Invoke ("KillMyself", Mathf.Abs (Vector3.Distance (brambleStart, brambleTarget)) / (travelTime * speed));
	}

    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        base.OnInteractWithPlayer(playerHealth, playerBoat, manager, collision);

        int healthChange = -damageDealt;
        //if this object is on the side of the player who owns this object
        //send out the command to change the players health
        //setting the source of the health change to be the owner of this cannonball
        playerHealth.ChangeHealth(healthChange, owner);
        Destroy(gameObject);
    }

}
