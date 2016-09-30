using UnityEngine;
using System.Collections;

public class CannonActiveState : MonoBehaviour {

	static bool ringsShowing = false;
	public GameObject boat;
	public GameObject ring1;
	public GameObject broadsideRings;
	public GameObject swivelCannon;
	public GameObject swivelCannonChild;
	const int numOfCannons = 8;
	public GameObject [] broadsideCannons = new GameObject[8];

	// Use this for initialization
	void Start () {
		ring1.SetActive (false);
		swivelCannon.GetComponent<SwivelCannon> ().enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			ringsShowing = !ringsShowing;
		}
		//when a cannon is clicked on its script becomes active
		if (ringsShowing == true) {
			ring1.SetActive (true);
			swivelCannonChild.GetComponent<BroadsideCannonFire> ().enabled = true;
			broadsideRings.SetActive (false);
			for (int i = 0; i < numOfCannons; i++) {
				broadsideCannons [i].GetComponent<BroadsideCannonFire> ().enabled = false;
			}
			swivelCannon.GetComponent<SwivelCannon> ().enabled = true;
			boat.GetComponent<SimplestMovement> ().enabled = false;
		} else if (ringsShowing == false) {
			broadsideRings.SetActive (true);
			ring1.SetActive (false);
			swivelCannonChild.GetComponent<BroadsideCannonFire> ().enabled = false;
			for (int i = 0; i < numOfCannons; i++) {
				broadsideCannons [i].GetComponent<BroadsideCannonFire> ().enabled = true;
			}
			swivelCannon.GetComponent<SwivelCannon> ().enabled = false;
			boat.GetComponent<SimplestMovement> ().enabled = true;
		}
	}
}
