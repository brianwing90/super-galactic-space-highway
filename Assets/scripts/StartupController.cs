using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Screen resolution.
		if(PlayerPrefs.HasKey("screen width") && PlayerPrefs.HasKey("screen height")){
			// Use previous screen resolution. This should be already set by Unity editor.
			int screenWidth = PlayerPrefs.GetInt("screen width");
			int screenHeight = PlayerPrefs.GetInt("screen height");
			Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen); // Set the saved screen resolution.
		}else{
			// Detect appropriate first time screen resolution.
			Resolution[] res = Screen.resolutions;
			Resolution middleRes = res[res.Length / 2];
			Screen.SetResolution(middleRes.width, middleRes.height, Screen.fullScreen);
			PlayerPrefs.SetInt("screen width", middleRes.width);
			PlayerPrefs.SetInt("screen height", middleRes.height);
		}
		
		// Sound volume.
		float volume = PlayerPrefs.GetFloat("volume");
		if(volume <= 0.0f){
			volume = 0.8f;
			PlayerPrefs.SetFloat("volume", volume);
		}
		
		// Set the game's audio volume.
		AudioListener.volume = volume;
		
		// Mouse sensitivity.
		float xSens = PlayerPrefs.GetFloat("horizontal sensitivity");
		float ySens = PlayerPrefs.GetFloat("vertical sensitivity");
		if(xSens <= 0.0f){
			PlayerPrefs.SetFloat("horizontal sensitivity", 0.8f);
		}
		if(ySens <= 0.0f){
			PlayerPrefs.SetFloat("vertical sensitivity", 0.8f);
		}
		
		// Player name.
		string name = PlayerPrefs.GetString("player name");
		if(name == ""){
			GameObject[] ui = GameObject.FindGameObjectsWithTag("UI");
			foreach(GameObject obj in ui){
				if(obj.name == "main menu"){
					obj.SetActive(false);
				}else if(obj.name == "settings menu"){
					obj.SetActive(false);
				}else if(obj.name == "credits menu"){
					obj.SetActive(false);
				}else if(obj.name == "help menu"){
					obj.SetActive(false);
				}else if(obj.name == "leaderboards menu"){
					obj.SetActive(false);
				}if(obj.name == "get player name"){
					obj.SetActive(true);
				}
			}
		}else{
			GameObject[] ui = GameObject.FindGameObjectsWithTag("UI");
			foreach(GameObject obj in ui){
				if(obj.name == "get player name"){
					obj.SetActive(false);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
