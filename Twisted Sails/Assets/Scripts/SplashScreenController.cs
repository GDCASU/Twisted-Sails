using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ((MovieTexture)GetComponent<Image>().material.mainTexture).Play();
        StartCoroutine(ChangeSceneAfterTime(((MovieTexture)GetComponent<Image>().material.mainTexture).duration));
	}
	
	IEnumerator ChangeSceneAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("Title Screen");
    }
}
