using UnityEngine;
using System.Collections;

public class SunManager : MonoBehaviour {
    MeshRenderer meshRenderer;
    
	// Use this for initialization
	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        meshRenderer.material.SetFloat("_NormalBlend", (Mathf.Sin(Time.time * 5f)+1)/2f);
        transform.localEulerAngles += new Vector3(0, 1, 0);
	}
}
