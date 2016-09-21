using UnityEngine;
using System.Collections;

public class HealthPack : MonoBehaviour {

	public bool healing = true;
	public static float healAmount = 50f;
	Rigidbody healthBody;
	MeshRenderer packMesh;
	CapsuleCollider packCollider;
	// Use this for initialization
	void Start () {
		//healthBody = this.gameObject.GetComponent<Rigidbody> ();
		//this.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		//this.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (healing == false) {
			//Debug.Log ("Healing is false");
			this.gameObject.GetComponent<MeshRenderer> ().enabled = false;
			this.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
			//Debug.Log ("Right before coroutine");
			healing = true;
			StartCoroutine( MyCoroutine ());
		}
		//healthBody.MovePosition(healthBody.position + this.transform.forward * Time.deltaTime * 4);
	}

	IEnumerator MyCoroutine(){
		//Debug.Log ("Coroutine Started");
		yield return new WaitForSeconds (3f);
		this.gameObject.GetComponent<MeshRenderer> ().enabled = true;
		this.gameObject.GetComponent<CapsuleCollider> ().enabled = true;
	}

	/*void OnTriggerEnter(Collider other) {

		healing = true;
		blueScript = other.gameObject.GetComponent<BlueEnemy> ();
		redScript = other.gameObject.GetComponent<RedEnemy> ();
	}*/
}