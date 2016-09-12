using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	float health = 100f;
	//public static float staticHealth;
	public GameObject explosion;
	//public GameObject cannonball;
	//public GameObject healthPack;
	public int teamNumber;
	public bool dead = false;

	void Start () {
	}

	// Update is called once per frame
	void Update () {
		print (health);
		//staticHealth = health;
		if (health <= 0) {
			dead = true;
			Master.arrayOfLives[teamNumber - 1] = Master.arrayOfLives[teamNumber - 1] - 1;
			Instantiate (explosion, this.transform.position, this.transform.rotation);
			Destroy (this.gameObject);
		}

	}

	void OnTriggerEnter(Collider other) {
		if (other.name.Equals("Cannonball(Clone)")){
			health = health - 35f;
			Destroy (other.gameObject);
		}

		if (other.name.Equals ("Health Pack")) {
			health = health + 50;
			//other.gameObject.GetComponent<HealthPack> ().enabled = true;;
		}
	}
}