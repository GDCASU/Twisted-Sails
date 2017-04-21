using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSelectButton : MonoBehaviour {

    public LobbyManager manager;
    public int ship;
    public AudioSource sound;
    public AudioClip backgroundMusic;

    private bool wasOn = false;
	
    public void Toggle(bool on)
    {
        if(on && !wasOn)
        {
            manager.SwitchShip(ship);
            sound.Play();
            GameObject.FindWithTag("BackgroundMusic").GetComponent<MusicManager>().inGameMusic = backgroundMusic;
        }
        wasOn = on;
    }
}
