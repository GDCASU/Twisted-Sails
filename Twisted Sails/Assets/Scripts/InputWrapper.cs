using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer:   Kyle Aycock
// Date:        3/30/2017
// Description: This class should be used when seeking keyboard input instead of UnityEngine.Input
//              By default Unity does not support the ability to monopolize keyboard input - this is a generic workaround to that issue

public class InputWrapper {

    private static bool KeyboardCaptured = false;

    public static void CaptureKeyboard()
    {
        KeyboardCaptured = true;
    }

    public static void ReleaseKeyboard()
    {
        KeyboardCaptured = false;
    }

	public static bool GetKey(KeyCode key)
    {
        return !KeyboardCaptured && Input.GetKey(key);
    }

    public static bool GetKey(string key)
    {
        return !KeyboardCaptured && Input.GetKey(key);
    }

    public static bool GetKeyDown(KeyCode key)
    {
        return !KeyboardCaptured && Input.GetKeyDown(key);
    }

    public static bool GetKeyDown(string key)
    {
        return !KeyboardCaptured && Input.GetKeyDown(key);
    }

    public static bool GetButtonDown(string button)
    {
        return !KeyboardCaptured && Input.GetButtonDown(button);
    }

    public static bool GetButton(string button)
    {
        return !KeyboardCaptured && Input.GetButtonDown(button);
    }
}
