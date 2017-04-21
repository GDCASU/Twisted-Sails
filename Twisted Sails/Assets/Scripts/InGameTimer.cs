using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameTimer : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        float time = ((TeamDeathmatch)MultiplayerManager.GetCurrentGamemode()).timeRemaining;
        if (time < 0)
        {
            GetComponent<Text>().text = "Overtime!";
        }
        else {
            int seconds = (int)(time % 60);
            GetComponent<Text>().text = (int)(time / 60) + ":" + (seconds >= 10 ? seconds.ToString() : ("0" + seconds));
        }

    }
}
