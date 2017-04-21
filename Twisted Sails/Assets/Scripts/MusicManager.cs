using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public AudioMixer activeMixer;
    public AudioClip inGameMusic; //to be changed in lobby

    private bool inGame;
    private static MusicManager instance;

    // Modified from Jesus's code
    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        inGame = false;
        DontDestroyOnLoad(this.gameObject); //already enforces only one of these
        SceneManager.sceneLoaded += sceneLoadCheck;
    }

    private void sceneLoadCheck(Scene scene, LoadSceneMode lsm)
    {
        if (scene.name.Contains("MainLevel"))
        {
            inGame = true;
            AudioSource inGameSource = transform.Find("InGame").GetComponent<AudioSource>();
            inGameSource.clip = inGameMusic;
            inGameSource.Play();
            activeMixer.FindSnapshot("InGame").TransitionTo(1);
        }
        else
        {
            if(inGame)
            {
                transform.Find("TitleScreen").GetComponent<AudioSource>().Play();
                inGame = false;
            }
            activeMixer.FindSnapshot("TitleScreen").TransitionTo(1);
        }
    }
}
