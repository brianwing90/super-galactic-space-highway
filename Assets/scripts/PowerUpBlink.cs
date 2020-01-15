using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBlink : MonoBehaviour {
	
	[Range(1, 20)]
	[Tooltip("Amount of time before power up dies.")]
	public float deathTime = 14;
	[Range(1, 20)]
	[Tooltip("Amount of time to wait before the blinking starts.")]
	public int delayTime = 3;
	
	private GameController gameCon; // The game's controller.
	private SpriteRenderer sprRen; // This object's SpriteRenderer.
	private float deathTimer; // Counts down death time.
	private bool alphaUp; // Whether to increase or decrease the sprite's alpha channel.
	private float delayTimer; // Counts down blink delay time.
	
	// Use this for initialization
	void Start () {
		// Get the game controller.
		gameCon = (GameController) GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		
		sprRen = GetComponent<SpriteRenderer>();
		deathTime = Mathf.Clamp(1.5f * gameCon.getDifficulty(), 5.0f, 20.0f); // Make sure deathTime can't be below 5 seconds; but, adjust it based on difficulty.
		deathTimer = Time.fixedTime + 1.0f;
		alphaUp = false;
		delayTimer = Time.fixedTime + 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(delayTime <= 0){
			// Blink alpha channel.
			sprRen = GetComponent<SpriteRenderer>();
			if(alphaUp && sprRen.color.a < 1.0f){
				Color color = sprRen.color;
				color.a += 10.0f / deathTime * Time.deltaTime;
				color.a = Mathf.Clamp(color.a, 0.0f, 1.0f); // Make sure color stays normalized.
				sprRen.color = color;
				
				if(sprRen.color.a >= 1.0f){
					alphaUp = false;
				}
			}else if(!alphaUp && sprRen.color.a > 0.0f){
				Color color = sprRen.color;
				color.a -= 10.0f / deathTime * Time.deltaTime;
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
		
		if(deathTime > 0 && deathTimer <= Time.fixedTime){
			deathTimer = Time.fixedTime + 1.0f; // Count down again in one seconds.
			deathTime--;
		}else if(deathTime <= 0){
			Destroy(gameObject);
		}
	}
}
