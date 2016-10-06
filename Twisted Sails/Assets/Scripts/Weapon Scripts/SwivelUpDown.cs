using UnityEngine;
using System.Collections;

public class SwivelUpDown : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.LeftShift)){
			//Debug.Log (this.transform.localEulerAngles.x);
			this.transform.Rotate (Input.GetAxis ("Mouse Y") * 2,0,0);
		}
	//this.transform.rotation = new Quaternion (Mathf.Clamp (this.transform.localEulerAngles.x, -360, 360), 0, 0, 0);
	}
}
