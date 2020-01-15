using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*http://dreamlo.com/lb/myRWqJsfaUCYc8XiQotBUAWFIMsYUuUECMFShEK8_rHA*/
public class LeaderboardController : MonoBehaviour {
	
	[Tooltip("The symbol to rotate while loading.")]
	public Transform loadingCircle;
	[Tooltip("The Text object to store the leaderboard names in.")]
	public Text leaderboardName;
	[Tooltip("The Text object to store the leaderboard scores in.")]
	public Text leaderboardScore;
	[Tooltip("The error to display with no connection.")]
	public GameObject wwwError;
	
	private WWW request;
	private bool scoresAreShown;
	private float lastTime; // Used to calculate custom deltaTime while Time.timeScale = 0.
	
	void Start () {
		getScores();
	}
	
	void OnEnable(){
		getScores();
	}
	
	void Update(){
		if(!scoresAreShown && request.isDone){			
			// Remove the loading symbol.
			loadingCircle.gameObject.SetActive(false);
			
			// Check if request completed successfully.
			if(request.error != null && request.error.Length > 0){
				wwwError.SetActive(true);
			}
			
			string[] line = request.text.Split('\n'); // This makes each line in the table an element.
			for(int i = 0; i < line.Length - 1; i++){
				string[] s = line[i].Split('|'); // Get individual values from the line.
				
				leaderboardName.text += s[0] + "\n";
				leaderboardScore.text += s[1] + "\n";
			}
			
			scoresAreShown = true;
		}else if(loadingCircle.gameObject.activeSelf){
			float deltaTime = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;
			loadingCircle.Rotate(0, 0, 500.0f * deltaTime);
		}
	}
	
	private void getScores(){
		lastTime = Time.realtimeSinceStartup;
		scoresAreShown = false; // Make sure new scores get shwon after request is done.
		wwwError.SetActive(false); // Hide error message until after request runs.
		loadingCircle.gameObject.SetActive(true); // Make sure loading circle is spinning.
		leaderboardName.text = ""; // Clear out current scores.
		leaderboardScore.text = ""; // Clear out current scores.
		string url = "http://dreamlo.com/lb/5917c6664389dd024c9d8cf5/pipe/20"; // Get top 20 results in ascending order.
		request = new WWW(url);
	}
}
