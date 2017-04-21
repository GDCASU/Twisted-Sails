using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleMusic : MonoBehaviour {

//destroys   previous  scene's BGM  and replaces with  new  BGM, makes sure there are no more than 2 BGM at 1 time.
	void Awake(){ 
		GameObject obj = GameObject.FindGameObjectWithTag ("titlemusic");
		Destroy (obj);


	}
}
