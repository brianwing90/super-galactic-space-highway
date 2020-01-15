using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	
	[Tooltip("Turns on console debugging information.")]
	public bool isDebug = true;
	[Range(6.0f, 16.0f)]
	[Tooltip("A higher frequency means more meteors will spawn.")]
	public float meteorFrequency = 8.0f;
	[Tooltip("A list of meteors that can spawn at random.")]
	public List<GameObject> meteors;
	[Range(0.001f, 1.0f)]
	[Tooltip("A higher frequency means more power ups will spawn.")]
	public float powerUpFrequency = 0.3f;
	[Tooltip("A list of power ups that can spawn at random.")]
	public List<GameObject> powerUps;
	
	private int startDelay; // Seconds to count down before game starts.
	private float startDelayTimer; // Times when to decrease the startDelay before the game starts.
	private bool paused; // Whether or not the game is paused.
	private Rect screenRect; // The world coordinates of the edges of the screen.
	private float meteorTimer; // The time between meteor spawns.
	private float powerUpTimer; // The time between power up spawns.
	private RectTransform healthRect; // The level of the player's health in the UI.
	private int score; // The player's score this run.
	private Text scoreText; // The text script of the score UI element.
	private float scoreTimer; // How long to wait between updating the score.
	private int diffThreshold; // The next score that will increase difficulty level.
	private float difficulty; // The game's difficulty level. Affects meteor spawn, health, and damage.
	private Text statusText; // The text object that shows the startup countdown, paused, and game over.
	private bool isGameOver; // True if the game is over.
	private float gameOverTimer; // How long to wait after the game is over before movement stops.
	private AudioSource startSoundShort; // Short start beep.
	private AudioSource startSoundLong; // Long start beep.
	private AudioSource diffSound; // The sound played when the difficulty goes up.
	private GameObject pauseScreen; // Contains all of the pause screen UI.
	private GameObject leaderboardScreen; // Shows the leaderboards.
	private int time; // The time in seconds for each run of the game. Used for leaderboarding.
	private float timeCounter; // Used to increment the time.
	
	// Use this for initialization
	void Start () {
		// Disable mouse during play.
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		startDelay = 3;
		startDelayTimer = Time.fixedTime + 0.667f; // Prevent changing status from 3 to 2 until after 2/3 of a second.
		paused = true;
		
		refreshScreenRect();
		
		meteorTimer = 1.0f;
		powerUpTimer = Time.fixedTime + 4.0f / powerUpFrequency;
		scoreTimer = 0.0f;
		
		GameObject[] ui = GameObject.FindGameObjectsWithTag("UI");
		foreach(GameObject obj in ui){
			if(obj.name == "health"){
				healthRect = obj.GetComponent<RectTransform>(); // Locate the RectTransform of the healthbar.
			}else if(obj.name == "score text"){
				scoreText = obj.GetComponent<Text>(); // Locate the Text of the score.
			}else if(obj.name == "status text"){
				statusText = obj.GetComponent<Text>();
				statusText.text = "3";
			}else if(obj.name == "pause screen"){
				pauseScreen = obj; // The parent object of the restard button and main menu button.
				pauseScreen.SetActive(false); // Hide the pause screen until the player dies or game is paused.
			}else if(obj.name == "leaderboards screen"){
				leaderboardScreen = obj;
			}
		}
		updateUI(100);
		
		diffThreshold = 1000;
		difficulty = 1.0f;
		gameOverTimer = 0.0f;
		
		// Get attached audio sources.
		AudioSource[] audio = GetComponents<AudioSource>();
		startSoundShort = audio[0];
		startSoundLong = audio[1];
		diffSound = audio[2];
		
		// Setup time counting.
		time = 0;
		timeCounter = Time.fixedTime + 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		// Run the game when it is not paused.
		if(!paused){
			// Increment the game's run time.
			if(timeCounter <= Time.fixedTime){
				time++;
				timeCounter = Time.fixedTime + 1.0f;
			}
			
			// Drop meteors and power ups randomly.
			if(meteorTimer <= Time.fixedTime && !isGameOver){
				// Drop power up instead of meteor.
				if(powerUpTimer <= Time.fixedTime){
					spawnPowerUp();
					powerUpTimer = Time.fixedTime + 4.0f / powerUpFrequency; // Time to wait before next power up spawn.
				}else{
					spawnMeteor();
					meteorTimer = Time.fixedTime + 4.0f / meteorFrequency; // Time to wait before next meteor spawn.
				}
			}
			
			// Add score.
			if(scoreTimer <= Time.fixedTime && !isGameOver){
				score++;
				scoreText.text = string.Format("{0}", score.ToString("D7")); // Show score padded with zeros.
				scoreTimer = Time.fixedTime + 0.05f * difficulty; // Update score every 1/20 of a second at 1.0 difficulty.
				
				// Increase difficulty based on score.
				if(score >= diffThreshold){
					diffThreshold *= 5;
					difficulty += 0.2f;
					meteorFrequency *= difficulty; // More meteors when it is more difficult.
					powerUpFrequency *= difficulty; // More power ups when it is more difficult.
					
					// Show player that it got harder.
					if(difficulty >= 1.4f){
						showStatusFade("Difficulty Up!"); // Show text on the screen.
						diffSound.Play();
					}
					
					if(isDebug){
						Debug.Log("The difficulty level is now " + difficulty);
					}
				}
			}
			
			// Check for game over.
			if(isGameOver){
				if(gameOverTimer <= Time.fixedTime){
					paused = true; // Stop all movement in the game.
					Time.timeScale = 0.0f;
					statusText.text = "Game Over"; // Show game over on the screen.
					statusText.enabled = true; // Turn on status text object.
					pauseScreen.SetActive(true); // Show the restart button.
					
					// Disable mouse only during play.
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					
					// Add player' score to leaderboards.
					string playerName = PlayerPrefs.GetString("player name");
					string platform = getPlatform();
					string url = "http://dreamlo.com/lb/myRWqJsfaUCYc8XiQotBUAWFIMsYUuUECMFShEK8_rHA/add/" + playerName + "/" + score + "/" + time + "/" + platform;
					new WWW(url);
				}
			}
			
			// Check for pause button being pressed and pause the game. Cannot pause during startup countdown.
			if(Input.GetButtonDown("Cancel") && startDelay <= 0){
				if(isDebug){
					Debug.Log("Escape button pressed.");
				}
				
				// Stop game movement.
				paused = true;
				Time.timeScale = 0.0f;
				
				// Disable mouse only during play.
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				
				// Show paused menu.
				statusText.text = "Paused"; // Show game over on the screen.
				statusText.enabled = true; // Turn on status text object.
				pauseScreen.SetActive(true); // Show the restart button.
			}
		}else{
			// Start the game after a brief, displayed delay. This is only used for new games.
			if(startDelay > 0 && startDelayTimer <= Time.fixedTime){
				startDelay--; // Decrease startDelay.
				if(startDelay <= 0){
					paused = false; // Start the game when startDelay gets to zero.
					statusText.text = "Go!";
					statusText.GetComponent<FadeText>().enabled = true; // Turn on the FadeText script.
					startSoundLong.Play();
				}else{
					statusText.text = "" + startDelay;
					startSoundShort.Play();
				}
				startDelayTimer = Time.fixedTime + 0.667f; // Decrease start delay again in 2/3 of a second.
			}else if(startDelay > 0){
				// Hide the leaderboards if it is shown.
				if(leaderboardScreen.activeSelf){
					leaderboardScreen.SetActive(false); // Hide leaderboards screen until button is pressed.
				}
			}
			
			// Check for pause button being pressed and unpause the game.
			if(Input.GetButtonDown("Cancel") && startDelay <= 0){
				if(isDebug){
					Debug.Log("Escape button pressed.");
				}
				
				// Start game movement.
				paused = false;
				Time.timeScale = 1.0f;
				
				// Disable mouse only during play.
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				
				// Hide pause menu and leaderboards menu.
				statusText.enabled = false; // Turn off status text object.
				pauseScreen.SetActive(false); // Hide the restart button.
				leaderboardScreen.SetActive(false); // Hide the leaderboards.
			}
		}
	}
	
	private string getPlatform(){
		string s = "";
		switch(Application.platform){
			case RuntimePlatform.OSXEditor:
				s = "osx editor";
				break;
			case RuntimePlatform.OSXPlayer:
				s = "osx";
				break;
			case RuntimePlatform.WindowsPlayer:
				s = "windows";
				break;
			case RuntimePlatform.OSXDashboardPlayer:
				s = "osx dashboard";
				break;
			case RuntimePlatform.WindowsEditor:
				s = "windows editor";
				break;
			case RuntimePlatform.IPhonePlayer:
				s = "iphone";
				break;
			case RuntimePlatform.Android:
				s = "android";
				break;
			case RuntimePlatform.LinuxPlayer:
				s = "linux";
				break;
			case RuntimePlatform.LinuxEditor:
				s = "linux editor";
				break;
			case RuntimePlatform.WebGLPlayer:
				s = "web";
				break;
			case RuntimePlatform.XboxOne:
				s = "xbox";
				break;
			case RuntimePlatform.Switch:
				s = "switch";
				break;
			default:
				s = "unknown";
				break;
		}
		
		return s;
	}
	
	public float getDifficulty(){
		return difficulty;
	}
	
	public bool isPaused(){
		if(paused && startDelay <= 0){
			// Game is paused and it is not the start of a game.
			return true;
		}else if(isGameOver){
			// Game is over.
			return true;
		}else{
			// Game is not paused or it is the start of a new game.
			return false;
		}
	}
	
	public void addToScore(int amount){
		if(!isGameOver){
			score += amount;
			scoreText.text = string.Format("{0}", score.ToString("D7")); // Show score padded with zeros.
		}
	}
	
	public void showStatus(string status){
		statusText.enabled = true;
		statusText.text = status;
		statusText.GetComponent<FadeText>().enabled = true; // Turn on the FadeText script.
	}
	
	public void showStatusFade(string status){
		statusText.enabled = true;
		FadeText script = statusText.GetComponent<FadeText>();
		script.enabled = true; // Enable fade message script.
		script.setPauseTime(Time.fixedTime + 1.0f); // Wait one second to fade.
		statusText.fontSize = 20; // Shrink font for in-game messages.
		statusText.text = status;
		statusText.GetComponent<FadeText>().enabled = true; // Turn on the FadeText script.
	}
	
	public Rect getScreenRect(){
		return screenRect;
	}
	
	public void gameOver(){
		isGameOver = true;
		gameOverTimer = Time.fixedTime + 3.0f; // Allow for 3 seconds of game action after game is over.
	}
	
	public void refreshScreenRect(){
		// Gets world coordinates from the camera for objects z distance away from the camera.
		Vector3 lowerLeft  = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
		Vector3 upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
		
		// Calculates the bounds of the screen.
		float top = upperRight.y; // Upper bound of the screen in world coordinates.
		float bottom = lowerLeft.y; // Upper bound of the screen in world coordinates.
		float left = lowerLeft.x; // Upper bound of the screen in world coordinates.
		float right = upperRight.x; // Upper bound of the screen in world coordinates.
		screenRect = new Rect(left, bottom, right, top);
		
		if(isDebug){
			Debug.Log("Calculated Screen Rect\nTop: " + top + "\nBottom: " + bottom + "\nLeft: " + left + "\nRight: " + right);
		}
	}
	
	private void spawnMeteor(){
		int ran = Mathf.FloorToInt(Random.Range(0.0f, meteors.Count)); // Randomly pick which meteor to create.
		float posX = Random.Range(screenRect.x, screenRect.width); // Randomly place new meteor within screen bounds.
		GameObject meteor = GameObject.Instantiate(meteors[ran], new Vector3(posX, 7, 0), Quaternion.identity);
		GameObject.Destroy(meteor, 8.0f); // Ensure all meteors eventually leave the game.
	}
	
	private void spawnPowerUp(){
		int ran = Mathf.FloorToInt(Random.Range(0.0f, powerUps.Count)); // Randomly pick which power up to create.
		float posX = Random.Range(screenRect.x, screenRect.width); // Randomly place new power up within screen bounds.
		GameObject powerUp = GameObject.Instantiate(powerUps[ran], new Vector3(posX, 7, 0), Quaternion.identity);
		GameObject.Destroy(powerUp, 8.0f); // Ensure all power ups eventually leave the game.
	}
	
	public void updateUI(int health){
		healthRect.localPosition = new Vector3(-100 + health, healthRect.localPosition.y, healthRect.localPosition.z);
		
		if(isDebug){
			Debug.Log("New player healthbar position: " + healthRect.localPosition.x);
		}
	}
}
