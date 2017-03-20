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
    public float explodingScale;

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
        else
            if (other.gameObject.tag == "Player")
                OnInteractWithPlayer(other.gameObject.GetComponent<Health>(), other.gameObject, manager, other);
    }

    public override void OnInteractWithPlayer(Health playerHealth, GameObject playerBoat, StatusEffectsManager manager, Collision collision)
    {
        base.OnInteractWithPlayer(playerHealth, playerBoat, manager, collision);
        playerHealth.health -= damageDealt;
    }

    private void ExplodeObject()
    {
        isExploding = true;
        explodingTimer = 1;
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
}
