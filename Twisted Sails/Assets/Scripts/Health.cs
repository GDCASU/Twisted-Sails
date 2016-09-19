using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	float health = 100f;
	public GameObject explosion;
	public int teamNumber;
	public bool dead = false;

	void Start () {
	}

	// Update is called once per frame
	void Update () {
		print ("Boat Health: " + health);
		//staticHealth = health;
		if (health <= 0) {
			dead = true;
			//Debug.Log ("Are they dead?: " + dead);
			Master.arrayOfLives[teamNumber - 1] = Master.arrayOfLives[teamNumber - 1] - 1;
			//Instantiate (explosion, this.transform.position, this.transform.rotation);
			//StartCoroutine( Wait());
			Destroy (this.gameObject);
		}
	}

	/*IEnumerator Wait() {
		yield return new WaitForSeconds (1f);
		Destroy (this.gameObject);
	}*/


	void OnTriggerEnter(Collider other) {
		if (other.name.Equals("Cannonball(Clone)")){
			health = health - 35f;
			Destroy (other.gameObject);
		}

		//Debug.Log ("Collided with something");

		if (other.name.Equals ("Health Pack")) {
			Debug.Log ("Collided with Health Pack");
			health = health + 50;
			other.gameObject.GetComponent<HealthPack> ().healing = false;
		}
		if (other.name.Equals ("Damage Object")) {
			Debug.Log ("Collided with Damage Object");
			health = health - 25;
			other.gameObject.GetComponent<HealthPack> ().healing = false;
		}
	}
}