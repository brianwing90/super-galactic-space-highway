using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBlink : MonoBehaviour {
	
	[Range(1, 20)]
	[Tooltip("Amount of time to wait before the blinking starts.")]
	public int delayTime = 5;
	
	private SpriteRenderer sprRen; // This object's SpriteRenderer.
	private bool alphaUp; // Whether to increase or decrease the sprite's alpha channel.
	private float delayTimer; // Counts down blink delay time.
	private int defaultDelayTime;
	
	// Use this for initialization
	void Start () {
		sprRen = GetComponent<SpriteRenderer>();
		alphaUp = false;
		delayTimer = Time.fixedTime + 1.0f;
		defaultDelayTime = delayTime;
	}
	
	void OnEnable(){
		delayTime = defaultDelayTime;
		delayTimer = Time.fixedTime + 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(delayTime <= 0){
			// Blink alpha channel.
			sprRen = GetComponent<SpriteRenderer>();
			if(alphaUp && sprRen.color.a < 1.0f){
				Color color = sprRen.color;
				color.a += 10.0f * Time.deltaTime;
				color.a = Mathf.Clamp(color.a, 0.0f, 1.0f); // Make sure color stays normalized.
				sprRen.color = color;
				
				if(sprRen.color.a >= 1.0f){
					alphaUp = false;
				}
			}else if(!alphaUp && sprRen.color.a > 0.0f){
				Color color = sprRen.color;
				color.a -= 10.0f * Time.deltaTime;
				color.a = Mathf.Clamp(color.a, 0.0f, 1.0f); // Make sure color stays normalized.
				sprRen.color = color;
				
				if(sprRen.color.a <= 0.0f){
					alphaUp = true;
				}
			}
		}else if(delayTimer <= Time.fixedTime){
			delayTime--;
			delayTimer = Time.fixedTime + 1.0f;
		}
	}
}
