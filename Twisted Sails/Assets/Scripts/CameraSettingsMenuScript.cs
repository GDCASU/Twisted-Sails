using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 Kyle Chapman
5/15/17
 Changed initialization code so to work with changes to the camera script for saving camera settings in the playerprefs.
*/

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
	
	//reference to text
	public Text verticalSensitivitySliderText;
	public Text horizontalSensitivitySliderText;
	public Text scrollSensitivitySliderText;
	
	//temporary values of camera settings
	private float tempHorizontalSensitivity;
    private float tempVerticalSensitivity;
    private float tempScrollSensitivity; // the speed at which the camera zooms in or out
	private bool tempInvertHorizontal;
	private bool tempInvertVertical;
	
	private BoatCameraNetworked playerBoatCamera;
	
	// Use this for initialization
	void Start ()
	{
		//get reference to boat camera
		playerBoatCamera = Camera.main.GetComponent<BoatCameraNetworked>();

		//initialize slider values to values of boat camera (loaded from playerprefs)
		verticalSensitivitySlider.value = tempVerticalSensitivity = playerBoatCamera.VerticalSensitivity;
		horizontalSensitivitySlider.value = tempHorizontalSensitivity = playerBoatCamera.HorizontalSensitivity;
		scrollSensitivitySlider.value = tempScrollSensitivity = playerBoatCamera.ScrollSensitivity;
		
		invertVerticalToggle.isOn = tempInvertHorizontal = playerBoatCamera.InvertHorizontal;
		invertHorizontalToggle.isOn = tempInvertVertical = playerBoatCamera.InvertVertical;
		
		//set callback functions for sliders and buttons and toggles
		verticalSensitivitySlider.onValueChanged.AddListener(delegate {VerticalSensitivityChangedProcess ();});
		horizontalSensitivitySlider.onValueChanged.AddListener(delegate {HorizontalSensitivityChangedProcess ();});
		scrollSensitivitySlider.onValueChanged.AddListener(delegate {ScrollSensitivityChangedProcess ();});
		
		invertVerticalToggle.onValueChanged.AddListener(delegate {InvertVerticalToggleChangedProcess ();});
		invertHorizontalToggle.onValueChanged.AddListener(delegate {InvertHorizontalToggleChangedProcess ();});
		
		okButton.onClick.AddListener(OKButtonClickProcess);
		cancelButton.onClick.AddListener(CancelButtonClickProcess);
		
		verticalSensitivitySliderText.text = tempVerticalSensitivity.ToString("0.0");
		horizontalSensitivitySliderText.text = tempHorizontalSensitivity.ToString("0.0");
		scrollSensitivitySliderText.text = tempScrollSensitivity.ToString("0.0");
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(cameraSettingsMenuCurrentState)
		{
			case EDITING_CAMERA_SETTINGS:{break;}
			
			case CAMERA_MENU_HIDDEN:
			{
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
		verticalSensitivitySliderText.text = verticalSensitivitySlider.value.ToString("0.0");
	}
	
	void HorizontalSensitivityChangedProcess()
	{
		tempHorizontalSensitivity = horizontalSensitivitySlider.value; //assign slider value to temp horizontal sensitivity
		horizontalSensitivitySliderText.text = horizontalSensitivitySlider.value.ToString("0.0");
	}
	
	void ScrollSensitivityChangedProcess()
	{
		tempScrollSensitivity = scrollSensitivitySlider.value; //assign slider value to temp scroll sensitivity
		scrollSensitivitySliderText.text = scrollSensitivitySlider.value.ToString("0.0");
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
		playerBoatCamera.VerticalSensitivity = tempVerticalSensitivity;
		playerBoatCamera.HorizontalSensitivity = tempHorizontalSensitivity;
		playerBoatCamera.ScrollSensitivity = tempScrollSensitivity;
		playerBoatCamera.InvertVertical = tempInvertVertical;
		playerBoatCamera.InvertHorizontal = tempInvertHorizontal;
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
