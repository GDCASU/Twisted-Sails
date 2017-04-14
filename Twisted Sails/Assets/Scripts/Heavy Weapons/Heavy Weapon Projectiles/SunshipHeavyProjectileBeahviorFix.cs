using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshipHeavyProjectileBeahviorFix : InteractiveObject {

	Vector3 startingLocation;
	Vector3 currentLocation;
	float distanceFromOrigin;
	public float maxDistance =1f;
    public float explodingScale; //How fast it explodes
    public int damageDealt;

    private bool isExploding;
    private float explodingTimer = 0;

    StatusEffectsManager manager;

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
            transform.localScale += new Vector3(.5f, .5f, .5f) * explodingScale;

            if (explodingTimer > 0)
                explodingTimer -= Time.deltaTime;
            if (explodingTimer <= 0)
                Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
            if (!isExploding)
                ExplodeObject();
    }

    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        base.OnInteractWithPlayer(playerHealth, playerBoat, manager, collision);
        int healthChange = -damageDealt;

        if (!isExploding)
            playerHealth.ChangeHealth(healthChange/2, owner);
        else
            playerHealth.ChangeHealth(healthChange, owner);

    }

    private void ExplodeObject()
    {
        isExploding = true;
        explodingTimer = 1;
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
}
