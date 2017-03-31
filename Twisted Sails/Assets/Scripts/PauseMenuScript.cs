﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Programmer:Pablo Camacho
//Date: 03/29/17


/*
 * Attach this script to canvas item used for pause menu.
*/

public class PauseMenuScript : MonoBehaviour {
	
	//reference to pause game script
	public PauseGame pauseGameScriptRef;
	
	//reference to camera settings menu script
	public CameraSettingsMenuScript camSettingsMenuScriptRef;
	
	//references to buttons in canvas of pause menu
	public Button resumeButton;
	public Button cameraSettingsButton;
	public Button exitToMainMenuButton;
	
	//state that pause menu is currently in
	public int pauseMenuCurrentState;
	
	//states of pause menu
	public const int PAUSE_MENU_HIDDEN=0;
	public const int IN_PAUSE_MENU_START=1; 
	public const int RESUME_GAME= 2; 
	public const int CAMERA_SETTINGS_EDITING = 3;
	public const int QUIT_GAME = 4;
	
	// Use this for initialization
	void Start () 
	{
		//initialize current state to pause menu hidden state
		pauseMenuCurrentState = PAUSE_MENU_HIDDEN;
		//set resume button on click to run ResumeButtonClickProcess function when clicked
		resumeButton.onClick.AddListener(ResumeButtonClickProcess);
		//set camera settings button on click to run CameraButtonClickProcess function when clicked
		cameraSettingsButton.onClick.AddListener(CameraButtonClickProcess);
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(pauseMenuCurrentState)
		{
			case PAUSE_MENU_HIDDEN:
			{
				//make pause menu inactive
				pauseGameScriptRef.UnPause();
				break;
			}
			case IN_PAUSE_MENU_START:
			{
				break;
			}
			case RESUME_GAME:
			{
				pauseMenuCurrentState = PAUSE_MENU_HIDDEN;
				break;
			}
			
			case CAMERA_SETTINGS_EDITING:
			{
				//set current state of camera settings menu to editing camera settings
				camSettingsMenuScriptRef.cameraSettingsMenuCurrentState = CameraSettingsMenuScript.EDITING_CAMERA_SETTINGS;
				// if camera settings menu is in hidden state, set current state of pause menu back to IN_PAUSE_MENU_START
				if(camSettingsMenuScriptRef.cameraSettingsMenuCurrentState == CameraSettingsMenuScript.CAMERA_MENU_HIDDEN)
				{ 
					pauseMenuCurrentState = IN_PAUSE_MENU_START;
					camSettingsMenuScriptRef.DeActivateCameraMenuCanvas();
				}
				break;
			}
			
			case QUIT_GAME:
			{
				break;
			}
		}
	}
	
	//function to run when resume button clicked
	void ResumeButtonClickProcess()
	{
		//set current state to resume game
		pauseMenuCurrentState = RESUME_GAME;
	}
	
	void CameraButtonClickProcess()
	{
		//set current state to camera editing settings
		pauseMenuCurrentState = CAMERA_SETTINGS_EDITING;
		camSettingsMenuScriptRef.ActivateCameraMenuCanvas();
	}
}