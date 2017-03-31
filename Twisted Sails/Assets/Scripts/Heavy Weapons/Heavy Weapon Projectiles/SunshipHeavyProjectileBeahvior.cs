using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshipHeavyProjectileBeahvior : InteractiveObject {

	Vector3 startingLocation;
	Vector3 currentLocation;
	float distanceFromOrigin;
	public float maxDistance =1f;
    private bool isExploding;
    private float explodingTimer = 0;

    StatusEffectsManager manager;
    public int damageDealt;
    private float explodingScale;

    // Use this for initialization
    void Start () {
		startingLocation = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		currentLocation = transform.position;
		distanceFromOrigin = Mathf.Abs(Vector3.Distance(currentLocation, startingLocation));

        if (distanceFromOrigin >= maxDistance && isExploding == false)
            ExplodeObject();

        if (isExploding)
        {
            transform.localScale += new Vector3(1, 1, 1) * explodingScale;

            if (explodingTimer > 0)
                explodingTimer -= Time.deltaTime;
            if (explodingTimer <= 0)
                Destroy(gameObject);
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isExploding)
            ExplodeObject();
    }

    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        base.OnInteractWithPlayer(playerHealth, playerBoat, manager, collision);

        int healthChange = -damageDealt;

        //if this object is on the side of the player who owns this object
        //send out the command to change the players health
        //setting the source of the health change to be the owner of this cannonball
        playerHealth.ChangeHealth(healthChange, owner);
    }

    private void ExplodeObject()
    {
        isExploding = true;
        explodingTimer = 1;
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
}
