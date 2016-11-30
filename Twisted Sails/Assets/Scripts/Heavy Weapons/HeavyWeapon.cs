using UnityEngine;
using System.Collections;
//  Programmer:     Nizar Kury
//  Date:           11/30/2016
//  Description:    Heavy weapon system (ground works)
public class HeavyWeapon : MonoBehaviour {

    public float coolDownTimer;
    public int ammoCapacity;
    public float damage;
    public float projectileSpeed;
    public KeyCode key;

	// Use this for initialization
	public virtual void Start () {
	
	}
	
	// Update is called once per frame
    public void Update () {  

	}

    public void OnCollisionEnter(Collision other)
    {

    }

    public void OnCollisionExit(Collision other)
    {

    }

    public void OnCollisionStay(Collision other)
    {

    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void OnTriggerStay(Collider other)
    {

    }
}
