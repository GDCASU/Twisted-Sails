using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SplashScreenController : MonoBehaviour {

    public VideoPlayer splashVideo;
    public GameObject blackScreen;
    bool startedPlaying;

    void Start()
    {
        splashVideo.Play();
    }

    void Update()
    {
        blackScreen.SetActive(!splashVideo.isPlaying);

        if (InputWrapper.GetKeyDown(KeyCode.Escape))
        {
            StopCoroutine("ChangeSceneAfterTime");
            SceneManager.LoadScene(1);
        }

        if (splashVideo.isPlaying)
        {
            startedPlaying = true;
            return;
        }

        if (startedPlaying)
            SceneManager.LoadScene(1);
    }
}
