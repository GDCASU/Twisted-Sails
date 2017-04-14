using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameTimer : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        float time = ((TeamDeathmatch)MultiplayerManager.GetCurrentGamemode()).timeRemaining;
        int seconds = (int)(time % 60);
        GetComponent<Text>().text = (int)(time / 60) + ":" + (seconds >= 10 ? seconds.ToString() : ("0" + seconds));
	}
}
