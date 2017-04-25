using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPackRotation : MonoBehaviour {

    Vector3 startPos;

    private float currentAngle;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        currentAngle = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = startPos;
        newPos.y += Mathf.Sin(Time.time*2);
        currentAngle -= 90 * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, currentAngle, 45);
        transform.position = newPos;
	}
}
