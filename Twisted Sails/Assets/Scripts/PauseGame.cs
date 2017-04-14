using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //for using SceneManager
using UnityEngine.Networking;

//Programmer:Pablo Camacho
//Date: 03/29/17
//Description: Pauses game

//Add this script to Network Manager

public class PauseGame : MonoBehaviour 
{
	//reference to pause menu canvas
	public Canvas pauseMenuCanvas;
	
	//reference to pause menu script
	private PauseMenuScript pauseMenuScriptRef;
	
	//bool to let other scripts know if game is paused
	public bool gamePaused;
	
	//reference to boatMovementNetworked component of player
	public BoatMovementNetworked playerBoatMovementComponent;
	
	// Use this for initialization
	void Start () 
	{
		//initialize gamePaused varaible to false
		gamePaused = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(playerBoatMovementComponent == null){InitPlayerCameraPauseBool();}
		//run process for pause menu
		if(pauseMenuCanvas != null){PauseMenuProcess();}
		else if(pauseMenuCanvas == null){InitPauseMenu();}
		
	}
	
	void InitPlayerCameraPauseBool()
	{
		//If current scene is in MainLevel_PabloCamacho or MainLevel
		if(Application.loadedLevelName == "MainLevel_PabloCamacho" ||
			Application.loadedLevelName == "MainLevel")
		{
			
			//get reference to player component BoatMovementNetworked
			Player thisPlayer = MultiplayerManager.GetLocalPlayer();
			GameObject thisPlayerGameObject = thisPlayer.GetPlayerObject();
            if (thisPlayerGameObject == null) return;
			playerBoatMovementComponent = thisPlayerGameObject.GetComponentInChildren<BoatMovementNetworked>();
		}
	}
	
	private void PauseMenuProcess()
	{
		
		//if p button is pressed down
		if(Input.GetKeyDown(KeyCode.P))
		{	
			//if canvas is not active, make it active
			if(pauseMenuCanvas.gameObject.activeInHierarchy == false)
			{
				Pause();
			}
			//else if canvas is active, make it inactive
			else
			{
				UnPause();
			}
		}
		
	}
	
	public void Pause()
	{
		pauseMenuCanvas.gameObject.SetActive(true);
		pauseMenuScriptRef.pauseMenuCurrentState = PauseMenuScript.IN_PAUSE_MENU_START;
		gamePaused = true;
		
		//assign game pause bool to game is paused bool of player boat movement component
		playerBoatMovementComponent.gameIsPaused = true;
		
	}
	
	public void UnPause()
	{
		pauseMenuCanvas.gameObject.SetActive(false);
		pauseMenuScriptRef.pauseMenuCurrentState = PauseMenuScript.PAUSE_MENU_HIDDEN;
		gamePaused = false;
		
		//assign game pause bool to game is paused bool of player boat movement component
		playerBoatMovementComponent.gameIsPaused = false;
		
	}
	
	private void InitPauseMenu()
	{
		//If current scene is in MainLevel_PabloCamacho
			if(Application.loadedLevelName == "MainLevel_PabloCamacho")
			{
				//get array of root game objects in Scene MainLevel_PabloCamacho
				GameObject[] rootGameObjectsOfMainLevelScene = SceneManager.GetSceneByName("MainLevel_PabloCamacho").GetRootGameObjects();
				
				//Go through all elements in root game objects array
				for(int i = 0; i < rootGameObjectsOfMainLevelScene.Length; i++)
				{
					//Debug.Log(i + " game object in Main Level is " + rootGameObjectsOfMainLevelScene[i]);
					
					//if root game object has tag PauseMenuCanvas
					if(rootGameObjectsOfMainLevelScene[i].tag == "PauseMenuCanvas")
					{
						//assign pauseMenuCanvas to canvas component of this root game object
						pauseMenuCanvas = rootGameObjectsOfMainLevelScene[i].GetComponent<Canvas>();
						
						//assign reference to this pause game script to PauseMenuScript
						PauseMenuScript thisPauseMenuScript = pauseMenuCanvas.GetComponentInChildren<PauseMenuScript>(); 
						thisPauseMenuScript.pauseGameScriptRef = transform.root.GetComponentInChildren<PauseGame>();
						//assign reference to pause menu script
						pauseMenuScriptRef = thisPauseMenuScript;
						
						if(pauseMenuCanvas != null){break;}//stop loop
					}
				}
			}
			//else if current scene is in MainLevel 
			else if(Application.loadedLevelName == "MainLevel")
			{
				//get array of root game objects in Scene MainLevel
				GameObject[] rootGameObjectsOfMainLevelScene = SceneManager.GetSceneByName("MainLevel").GetRootGameObjects();
				
				//Go through all elements in root game objects array
				for(int i = 0; i < rootGameObjectsOfMainLevelScene.Length; i++)
				{
					//Debug.Log(i + " game object in Main Level is " + rootGameObjectsOfMainLevelScene[i]);
					
					//if root game object has tag PauseMenuCanvas
					if(rootGameObjectsOfMainLevelScene[i].tag == "PauseMenuCanvas")
					{
						//assign pauseMenuCanvas to canvas component of this root game object
						pauseMenuCanvas = rootGameObjectsOfMainLevelScene[i].GetComponent<Canvas>();
						
						//assign reference to this pause game script to PauseMenuScript
						PauseMenuScript thisPauseMenuScript = pauseMenuCanvas.GetComponentInChildren<PauseMenuScript>(); 
						thisPauseMenuScript.pauseGameScriptRef = transform.root.GetComponentInChildren<PauseGame>();
						//assign reference to pause menu script
						pauseMenuScriptRef = thisPauseMenuScript;
						
						if(pauseMenuCanvas != null){break;}//stop loop
					}
				}
			}
	}
}
