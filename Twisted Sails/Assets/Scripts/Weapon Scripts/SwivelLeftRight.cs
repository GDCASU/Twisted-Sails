using UnityEngine;
using System.Collections;

public class SwivelLeftRight : MonoBehaviour {

	public float rotationSpeed = 4;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		//Debug.Log (rotationSpeed);
		if (Input.GetKey(KeyCode.Q)){
			//rotationSpeed += Time.deltaTime * 20;
			this.transform.Rotate (0, 0, -rotationSpeed);
		}
		else if(Input.GetKey(KeyCode.E)){
			//rotationSpeed -= Time.deltaTime * 20;
			this.transform.Rotate (0, 0, rotationSpeed);
		}
	}
}