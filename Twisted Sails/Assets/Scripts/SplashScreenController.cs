using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(ChangeSceneAfterTime());
	}
	
	IEnumerator ChangeSceneAfterTime()
    {
        yield return new WaitForSeconds(7);
        SceneManager.LoadScene("Title Screen");
    }
}
