using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //for using SceneManager

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
	public const int NULL = 0; //for start of game
	public const int PAUSE_MENU_HIDDEN = 1;
	public const int IN_PAUSE_MENU_START = 2; 
	public const int RESUME_GAME = 3; 
	public const int CAMERA_SETTINGS_EDITING = 4;
	public const int QUIT_GAME = 5;
	
	// Use this for initialization
	void Start () 
	{
		//initialize current state to pause menu hidden state
		pauseMenuCurrentState = NULL;
		//set resume button on click to run ResumeButtonClickProcess function when clicked
		resumeButton.onClick.AddListener(ResumeButtonClickProcess);
		//set camera settings button on click to run CameraButtonClickProcess function when clicked
		cameraSettingsButton.onClick.AddListener(CameraButtonClickProcess);
		//set exit to main menu button on click to run ExitToMainMenuButtonClickProcess when clicked
		exitToMainMenuButton.onClick.AddListener(ExitToMainMenButtonClickProcess);
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(pauseMenuCurrentState)
		{
			case NULL:
			{
				break;
			}
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
	
	//function to run when camera settings button clicked
	void CameraButtonClickProcess()
	{
		//set current state to camera editing settings
		pauseMenuCurrentState = CAMERA_SETTINGS_EDITING;
		camSettingsMenuScriptRef.ActivateCameraMenuCanvas();
	}
	
	//function to run when exit to main menu button clicked
	void ExitToMainMenButtonClickProcess()
	{
		//set current state to quit game
		pauseMenuCurrentState = QUIT_GAME;
		
		//disconnect from network
		Network.Disconnect();
		//go to Title Screen
		int nextSceneIndex = 0;
		SceneManager.LoadScene(nextSceneIndex);
	}
}
