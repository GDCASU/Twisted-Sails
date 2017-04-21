using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipSounds : MonoBehaviour {

    public float shipMaxSpeed;

	// Use this for initialization
	void Start () {
        transform.Find("ShipAmbient").GetComponent<AudioSource>().PlayDelayed(3);
	}

    void Update()
    {
        if(IsLocalPlayer())
            transform.Find("Windy").GetComponent<AudioSource>().volume = Mathf.Pow(transform.parent.GetComponent<Rigidbody>().velocity.magnitude,2) / Mathf.Pow(shipMaxSpeed,2);
        Debug.Log(transform.parent.GetComponent<Rigidbody>().velocity.magnitude);
    }


    bool IsLocalPlayer()
    {
        return (MultiplayerManager.IsClient() && transform.parent.GetComponent<NetworkIdentity>().netId == MultiplayerManager.GetLocalPlayer().objectId);
    }
}
