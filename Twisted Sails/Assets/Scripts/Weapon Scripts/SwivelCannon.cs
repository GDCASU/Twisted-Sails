using UnityEngine;
using System.Collections;

public class SwivelCannon : MonoBehaviour {

	public float rotationSpeed = 4;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (rotationSpeed);
		if (Input.GetKey(KeyCode.A)){
			//rotationSpeed += Time.deltaTime * 20;
			this.transform.Rotate (0, -rotationSpeed, 0);
		}
		else if(Input.GetKey(KeyCode.D)){
			//rotationSpeed -= Time.deltaTime * 20;
			this.transform.Rotate (0, rotationSpeed, 0);
		}
	}
}
