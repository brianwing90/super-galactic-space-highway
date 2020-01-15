using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonController : MonoBehaviour {
	
	private GameObject mainMenu;
	private GameObject settingsMenu;
	private GameObject creditsMenu;
	private GameObject helpMenu;
	private GameObject leaderboardsMenu;
	private GameObject pauseScreen;
	private GameObject statusText;
	private GameObject getName;
	private Text nameField;
	
	// Use this for initialization
	void Start () {
		GameObject[] ui = GameObject.FindGameObjectsWithTag("UI");
		foreach(GameObject obj in ui){
			if(obj.name == "main menu"){
				mainMenu = obj;
			}else if(obj.name == "settings menu"){
				settingsMenu = obj;
				settingsMenu.SetActive(false);
			}else if(obj.name == "credits menu"){
				creditsMenu = obj;
				creditsMenu.SetActive(false);
			}else if(obj.name == "help menu"){
				helpMenu = obj;
				helpMenu.SetActive(false);
			}else if(obj.name == "leaderboards menu"){
				leaderboardsMenu = obj;
				leaderboardsMenu.SetActive(false);
			}else if(obj.name == "leaderboards screen"){
				leaderboardsMenu = obj;
			}else if(obj.name == "pause screen"){
				pauseScreen = obj;
			}else if(obj.name == "status text"){
				statusText = obj;
			}else if(obj.name == "get player name"){
				getName = obj;
			}else if(obj.name == "name field"){
				nameField = obj.GetComponent<Text>();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void DoAction(string name){
		if(transform.tag == "UI"){ // Object is a UI object.
			// Play button sound.
			GetComponent<AudioSource>().Play();
			
			switch(name){
				case "play button":
					Debug.Log("The play button was pressed.");
					SceneManager.LoadSceneAsync("game", LoadSceneMode.Single); // Load the new game asynchronously so that frames do not drop.
					break;
				case "leaderboards button":
					Debug.Log("The leaderboards button was pressed.");
					// Show leaderboards screen.
					if(mainMenu != null){
						mainMenu.SetActive(false); // Turn off main menu.
					}
					if(pauseScreen != null){
						pauseScreen.SetActive(false); // Turn off pause menu.
					}
					if(statusText != null){
						statusText.SetActive(false); // Hide status text.
					}
					leaderboardsMenu.SetActive(true); // Turn on leaderboards menu.
					break;
				case "settings button":
					Debug.Log("The settings button was pressed.");
					// Show settings menu.
					mainMenu.SetActive(false); // Turn off main menu.
					settingsMenu.SetActive(true); // Turn on settings menu.
					break;
				case "quit button":
					Debug.Log("The quit button was pressed.");
					Application.Quit(); // Quit the game.
					break;
				case "restart button":
					Debug.Log("The restart button was pressed.");
					// Restart the game.
					Time.timeScale = 1.0f;
					SceneManager.LoadSceneAsync("game", LoadSceneMode.Single);
					break;
				case "main menu button":
					Debug.Log("The main menu button was pressed.");
					// Go back to the main menu.
					Time.timeScale = 1.0f;
					SceneManager.LoadSceneAsync("main menu", LoadSceneMode.Single);
					break;
				case "help button":
					Debug.Log("The help button was pressed.");
					// Go to the help menu.
					mainMenu.SetActive(false); // Turn off main menu.
					helpMenu.SetActive(true); // Turn on help menu.
					break;
				case "credits button":
					Debug.Log("The credits button was pressed.");
					// Go to credits.
					mainMenu.SetActive(false); // Turn off main menu.
					creditsMenu.SetActive(true); // Turn on credits menu.
					break;
				case "back button":
					Debug.Log("The back button was pressed.");
					if(settingsMenu != null){
						settingsMenu.SetActive(false); // Turn off all other menus.
					}
					if(creditsMenu != null){
						creditsMenu.SetActive(false); // Turn off all other menus.
					}
					if(helpMenu != null){
						helpMenu.SetActive(false); // Turn off all other menus.
					}
					if(leaderboardsMenu != null){
						leaderboardsMenu.SetActive(false); // Turn off all other menus.
					}
					if(mainMenu != null){
						mainMenu.SetActive(true); // Turn on main menu.
					}
					if(pauseScreen != null){
						pauseScreen.SetActive(true); // Turn on pause screen.
					}
					if(statusText != null){
						statusText.SetActive(true); // Show status text.
					}
					break;
				case "submit button":
					// Players name submitted.
					nameSubmit();
					break;
				default:
					Debug.Log("An unknown button was pressed.");
					break;
			}
		}
	}
	
	public void nameSubmit(){
		// Accept name if name is not blank.
		if(nameField.text != ""){
			PlayerPrefs.SetString("player name", nameField.text); // Store player's name.
			getName.SetActive(false); // Hide name field.
			mainMenu.SetActive(true); // Show main menu.
		}
	}
}
