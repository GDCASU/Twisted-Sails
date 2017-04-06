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
		if (InputWrapper.GetKey(KeyCode.Q)){
			//rotationSpeed += Time.deltaTime * 20;
			this.transform.Rotate (0, 0, -rotationSpeed);
		}
		else if(InputWrapper.GetKey(KeyCode.E)){
			//rotationSpeed -= Time.deltaTime * 20;
			this.transform.Rotate (0, 0, rotationSpeed);
		}
	}
}