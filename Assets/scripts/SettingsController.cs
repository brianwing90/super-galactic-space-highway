using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {
	
	[Tooltip("The Dropdown UI element that will set the possible screen resolutions.")]
	public Dropdown resDD;
	[Tooltip("The Slider UI element that will set the volume.")]
	public Slider volume;
	[Tooltip("The Slider UI element that will set the horizontal mouse sensitivity.")]
	public Slider xSens;
	[Tooltip("The Slider UI element that will set the vertical mouse sensitivity.")]
	public Slider ySens;
	
	private int width;
	private int height;
	
	// Use this for initialization
	void Start () {
		Resolution[] resList = Screen.resolutions;
		foreach(Resolution res in resList){
			resDD.options.Add(new Dropdown.OptionData(res.width + "x" + res.height)); // Add resolution option to the dropdown.
		}
		resDD.options.Reverse(); // Show resolutions from greatest to least.
		
		width = Screen.width;
		height = Screen.height;
		
		// Select the current screen resolution from the dropdown by default.
		string currentRes = width + "x" + height;
		for(int i = 0; i < resDD.options.Count; i++){
			if(resDD.options[i].text == currentRes){
				resDD.value = i;
				break;
			}
		}
		
		volume.value = PlayerPrefs.GetFloat("volume"); // Set the volume slider position.
		xSens.value = PlayerPrefs.GetFloat("horizontal sensitivity"); // Set the x slider position.
		ySens.value = PlayerPrefs.GetFloat("vertical sensitivity"); // Set the y slider position.
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void resolutionChanged(){
		// Update the screen resolution when the options change.
		string[] split = resDD.GetComponentInChildren<Text>().text.Split('x');
		int width = int.Parse(split[0]);
		int height = int.Parse(split[1]);
		Screen.SetResolution(width, height, Screen.fullScreen); // Set the new screen resolution.
		
		// Save the new resolution as a playerpref for later startups.
		PlayerPrefs.SetInt("screen width", width);
		PlayerPrefs.SetInt("screen height", height);
	}
	
	public void volumeChanged(){
		PlayerPrefs.SetFloat("volume", volume.value);
		
		// Set the game's audio volume.
		AudioListener.volume = volume.value;
	}
	
	public void sensitivityChanged(){
		PlayerPrefs.SetFloat("horizontal sensitivity", xSens.value);
		PlayerPrefs.SetFloat("vertical sensitivity", ySens.value);
	}
}
