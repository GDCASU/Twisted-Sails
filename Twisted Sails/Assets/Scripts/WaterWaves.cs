using UnityEngine;
using System.Collections;

public class WaterWaves : MonoBehaviour {

    private Mesh water;



	// Use this for initialization
	void Start () {
        water = this.GetComponent<MeshFilter>().mesh;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
