using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeText : MonoBehaviour {
	
	[Range(0.1f, 5.0f)]
	[Tooltip("How quickly the text should fade. Smaller means slower fade.")]
	public float fadeTime = 2.0f;
	
	private Text uiText;
	private int fontSize; // The default size of the text object.
	private float pauseTime; // Amount of time to wait before the fade occurs.
	
	// Use this for initialization
	void Start () {
		uiText = GetComponent<Text>();
		fontSize = uiText.fontSize;
		pauseTime = 0.0f; // Start with no pause time.
	}
	
	// Update is called once per frame
	void Update () {
		if(pauseTime <= Time.fixedTime){
			Color uiColor = uiText.color;
			uiColor.a -= Time.deltaTime * fadeTime;
			uiText.color = uiColor;
			
			// Reset all fade attributes once faing is complete.
			if(uiText.color.a <= 0){
				uiText.enabled = false; // Hide status text.
				uiText.fontSize = fontSize;
				uiColor.a = 1.0f;
				uiText.color = uiColor; // Reset alpha channel to no fade.
				this.enabled = false; // Disable this script.
			}
		}
	}
	
	public void setPauseTime(float time){
		pauseTime = time;
	}
}
