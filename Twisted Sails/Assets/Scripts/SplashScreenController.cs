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

    void Update()
    {
        if(InputWrapper.GetKeyDown(KeyCode.Escape))
        {
            StopCoroutine("ChangeSceneAfterTime");
            SceneManager.LoadScene(1);
        }
    }
	
	IEnumerator ChangeSceneAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(1);
    }
}
