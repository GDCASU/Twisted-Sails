using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSettingsMenuScript : MonoBehaviour {
	
	//public reference to camera menu canvas
	public Image cameraMenu;
	
	public int cameraSettingsMenuCurrentState = 4;
	public const int EDITING_CAMERA_SETTINGS = 0;
	public const int CAMERA_MENU_HIDDEN = 4;
	//no states for button clicked because it proved too slow in practice
	
	//references to buttons in canvas of camera settings menu
	public Button okButton;
	public Button cancelButton;
	public Slider verticalSensitivitySlider;
	public Slider horizontalSensitivitySlider;
	public Slider scrollSensitivitySlider;
	public Toggle invertVerticalToggle;
	public Toggle invertHorizontalToggle;
	
	//temporary values of camera settings
	private float tempHorizontalSensitivity = 4.0f;
    private float tempVerticalSensitivity = 1.0f;
    private float tempScrollSensitivity = 10.0f; // the speed at which the camera zooms in or out
	private bool tempInvertHorizontal = false;
	private bool tempInvertVertical = false;
	
	
	private BoatCameraNetworked playerBoatCamera;
	
	// Use this for initialization
	void Start () 
	{
		
		//get reference to boat camera
		playerBoatCamera = Camera.main.GetComponent<BoatCameraNetworked>();
		
		//initialize slider values to values of boat camera
		
		
		//set callback functions for sliders and buttons and toggles
		verticalSensitivitySlider.onValueChanged.AddListener(delegate {VerticalSensitivityChangedProcess ();});
		horizontalSensitivitySlider.onValueChanged.AddListener(delegate {HorizontalSensitivityChangedProcess ();});
		scrollSensitivitySlider.onValueChanged.AddListener(delegate {ScrollSensitivityChangedProcess ();});
		
		invertVerticalToggle.onValueChanged.AddListener(delegate {InvertVerticalToggleChangedProcess ();});
		invertHorizontalToggle.onValueChanged.AddListener(delegate {InvertHorizontalToggleChangedProcess ();});
		
		okButton.onClick.AddListener(OKButtonClickProcess);
		cancelButton.onClick.AddListener(CancelButtonClickProcess);
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(cameraSettingsMenuCurrentState)
		{
			case EDITING_CAMERA_SETTINGS:{break;}
			
			case CAMERA_MENU_HIDDEN:
			{
				//DeActivateCameraMenuCanvas();
				break;
			}
		}
		
	}
	
	public void ActivateCameraMenuCanvas()
	{
		cameraMenu.gameObject.SetActive(true);
	}
	
	public void DeActivateCameraMenuCanvas()
	{
		cameraMenu.gameObject.SetActive(false);
	}
	
	//Functions to update temp varaibles
	void VerticalSensitivityChangedProcess()
	{
		tempVerticalSensitivity = verticalSensitivitySlider.value; //assign slider value to temp vertical sensitivity
	}
	
	void HorizontalSensitivityChangedProcess()
	{
		tempHorizontalSensitivity = horizontalSensitivitySlider.value; //assign slider value to temp horizontal sensitivity
	}
	
	void ScrollSensitivityChangedProcess()
	{
		tempScrollSensitivity = scrollSensitivitySlider.value; //assign slider value to temp scroll sensitivity
	}
	
	void InvertVerticalToggleChangedProcess()
	{
		tempInvertVertical = invertVerticalToggle.isOn; //assign invert vertical toggle value to temp invert vertical 
	}
	
	void InvertHorizontalToggleChangedProcess()
	{
		tempInvertHorizontal = invertHorizontalToggle.isOn; //assign invert horizontal toggle value to temp invert horizontal
	}
	
	void OKButtonClickProcess()
	{
		//assign temp values to boat camera values
		playerBoatCamera.verticalSensitivity = tempVerticalSensitivity;
		playerBoatCamera.horizontalSensitivity = tempHorizontalSensitivity;
		playerBoatCamera.scrollSensitivity = tempScrollSensitivity;
		playerBoatCamera.invertVertical = tempInvertVertical;
		playerBoatCamera.invertHorizontal = tempInvertHorizontal;
		//deactivate camera menu
		DeActivateCameraMenuCanvas();
		//set state to camera menu hidden
		cameraSettingsMenuCurrentState = CAMERA_MENU_HIDDEN;
	}
	
	void CancelButtonClickProcess()
	{
		//deactivate camera menu
		DeActivateCameraMenuCanvas();
		//set state to camera menu hidden
		cameraSettingsMenuCurrentState = CAMERA_MENU_HIDDEN;
	}
	
	
}
