using UnityEngine;
using System.Collections;

public class SimplestMovement : MonoBehaviour {

	Rigidbody playerBody;
	public float speed = 2;
	// Use this for initialization
	void Start () {
		playerBody = this.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
			if (InputWrapper.GetKey(KeyCode.W))
				playerBody.MovePosition(playerBody.position + Vector3.forward * speed * Time.deltaTime);

			if (InputWrapper.GetKey(KeyCode.S))
				playerBody.MovePosition(playerBody.position - Vector3.forward * speed * Time.deltaTime);

			if (InputWrapper.GetKey(KeyCode.D))
				playerBody.MovePosition(playerBody.position + Vector3.right * speed * Time.deltaTime);

			if (InputWrapper.GetKey(KeyCode.A))
				playerBody.MovePosition(playerBody.position - Vector3.right * speed * Time.deltaTime);

	}
}
