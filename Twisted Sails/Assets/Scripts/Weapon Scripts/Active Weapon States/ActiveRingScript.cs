using UnityEngine;
using System.Collections;

public class ActiveRingScript : MonoBehaviour {

	public Transform cannon;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = cannon.position;
	}
}
