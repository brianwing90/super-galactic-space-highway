using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	
	[Range(5, 15)]
	[Tooltip("Player movement speed. Higher means faster movement.")]
	public int speed = 8; // Speed of player movement.
	[Range(1, 12)]
	[Tooltip("Player fire speed. Higher means less time between shots.")]
	public int fireSpeed = 10; // Speed between shots.
	[Tooltip("The prefab to use for generating new laser shots.")]
	public GameObject laser;
	[Tooltip("The prefab to use for generating power up score popups.")]
	public GameObject scorePopup;
	[Tooltip("The prefab to use for generating death particles.")]
	public GameObject playerFragment;
	
	private GameController gameCon; // The game's controller. Stores settings and score.
	private Transform trans; // The transform of this object.
	private float halfSpriteWidth; // Half the width of the player sprite.
	private float halfSpriteHeight; // Half the height of the player sprite.
	private float fireTimer; // Timer to tell if player can fire again yet.
	private AudioSource hitSound; // Sound that plays when the player is hit.
	private AudioSource powerUpSound; // Sound that plays when the play gets a power up.
	private AudioSource deathSound; // The sound the player makes when they die.
	private int health; // The player's health.
	private int damage; // How much damage the player's lasers do.
	private bool isPoweredUp; // Whether or not the player has a power up.
	private float powerUpTimer; // Length of time before power ups wear off.
	private float powerUpLength; // Length of time a power up lasts.
	private float shieldTimer; // Length of time before the shield wear off.
	private float shieldLength; // Length of time a shield lasts.
	private int defaultSpeed; // Player's default speed. Used with power ups.
	private int defaultFireSpeed; // Player's default fire rate. Used with power ups.
	private int defaultDamage; // Player's default damage amount. Used with power ups.
	private GameObject shield; // The player's shield.
	private float xSens; // Horizontal mouse sensitivity.
	private float ySens; // Vertical mouse sensitivity.
	
	// Use this for initialization
	void Start () {
		// Get the game controller.
		gameCon = (GameController) GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		
		// Get this object's transform.
		trans = GetComponent<Transform>(); 
		
		// Gets player sprite specs for calculating the bounds of the sprite.
		Vector2 spriteSize = GetComponent<SpriteRenderer>().sprite.rect.size;
		float spritePPU = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
		
		// Calculates the bounds of the sprite in world coordinates.
		halfSpriteWidth = spriteSize.x / spritePPU / 2;
		halfSpriteHeight = spriteSize.y / spritePPU / 2;
		
		if(gameCon.isDebug){
			Debug.Log("Player Sprite Size\nHalf Width: " + halfSpriteWidth + "\nHalf Height: " + halfSpriteHeight);
		}
		
		fireTimer = 0.0f;
		
		// Get the sounds attached to the player.
		AudioSource[] clips = GetComponents<AudioSource>();
		hitSound = clips[1];
		powerUpSound = clips[2];
		deathSound = clips[3];
		
		health = 100;
		damage = 20;
		defaultDamage = damage;
		isPoweredUp = false;
		powerUpTimer = 0.0f;
		powerUpLength = 8.0f;
		shieldTimer = 0.0f;
		shieldLength = 3.0f;
		defaultSpeed = speed;
		defaultFireSpeed = fireSpeed;
		
		shield = trans.GetChild(0).gameObject; // The player's shield.
		
		// Set the mouse sensitivity.
		xSens = PlayerPrefs.GetFloat("horizontal sensitivity");
		ySens = PlayerPrefs.GetFloat("vertical sensitivity");
	}
	
	// Update is called once per frame
	void Update () {
		if(!gameCon.isPaused()){
			// Shoot if key is pressed and timer allows it.
			if(Input.GetButton("Fire1") && fireTimer <= Time.fixedTime){
				shoot();
				fireTimer = Time.fixedTime + 0.1f / fireSpeed * 10;
			}
			
			Vector3 pos = trans.position;
			Rect screen = gameCon.getScreenRect();
			float step = Time.deltaTime * speed; // Ensure player movement speed is normalized accross differing frame rates.
			
			// Move forward or backward.
			float newY = pos.y;
			float bottom = screen.y + halfSpriteHeight + 1.0f; // Set the bottom screen bound to the edge of the screen + 1 to offset the UI at the bottom.
			float top = screen.height - halfSpriteHeight;
			newY = Mathf.Clamp(pos.y + step * Input.GetAxis("Mouse Y") * ySens, bottom, top);
			
			// Move left or rght.
			float newX = pos.x;
			float left = screen.x + halfSpriteWidth;
			float right = screen.width - halfSpriteWidth;
			newX = Mathf.Clamp(pos.x + step * Input.GetAxis("Mouse X") * xSens, left, right);
			trans.position = new Vector3(newX, newY, pos.z);
			
			// Check for power ups and disable if time has run out.
			if(isPoweredUp && powerUpTimer <= Time.fixedTime){
				isPoweredUp = false;
				
				// Remove power up effects.
				speed = defaultSpeed;
				fireSpeed = defaultFireSpeed;
				damage = defaultDamage;
				shield.SetActive(false);
			}
			if(shield.activeSelf && shieldTimer <= Time.fixedTime){
				// Remove shield.
				shield.SetActive(false);
			}
			
			// Undo red blink after being hit.
			SpriteRenderer spriteRen = GetComponent<SpriteRenderer>();
			if(spriteRen.color.g <= 1.0f || spriteRen.color.b <= 1.0f){
				Color color = spriteRen.color;
				color.g += 4.0f * Time.deltaTime;
				color.g = Mathf.Clamp(color.g, 0.0f, 1.0f); // Make sure color stays normalized.
				color.b += 4.0f * Time.deltaTime;
				color.b = Mathf.Clamp(color.b, 0.0f, 1.0f); // Make sure color stays normalized.
				spriteRen.color = color;
			}
		}
	}
	
	private void shoot(){
		if(gameCon.isDebug){
			Debug.Log("Shots fired.");
		}
		
		// Fire left laser.
		Vector3 lPos = new Vector3(trans.position.x - 0.48f, trans.position.y + 0.9f, trans.position.z);
		GameObject l = GameObject.Instantiate(laser, lPos, Quaternion.identity);
		l.GetComponent<LaserController>().setDamage(damage); // Give player's damage to laser.
		l.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 0.08f); // Set laser's forward motion.
		Destroy(l, 3.0f);
		
		// Fire right laser.
		lPos = new Vector3(trans.position.x + 0.48f, trans.position.y + 0.9f, trans.position.z);
		l = GameObject.Instantiate(laser, lPos, Quaternion.identity);
		l.GetComponent<LaserController>().setDamage(damage); // Give player's damage to laser.
		l.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 0.08f); // Set laser's forward motion.
		Destroy(l, 3.0f);
	}
	
	public int getDamage(){
		return damage;
	}
	
	void OnTriggerEnter2D(Collider2D col){
		// Apply damage to player if he/she collided with a meteor.
		if(col.tag == "Meteor"){
			hitSound.Play();
			
			// Only take damage if the shield is not up.
			if(!shield.activeSelf){
				health -= col.GetComponent<MeteorController>().getDamage(); // Subtract meteor damage from player's health.
				gameCon.updateUI(health);
				
				// Blink player red.
				GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
			}
			
			
			if(gameCon.isDebug){
				Debug.Log("Player hit by a meteor.\nNew Health: " + health);
			}
		}else if(col.tag == "Power Up"){
			// Show power up score bonus.
			GameObject popup = GameObject.Instantiate(scorePopup, col.transform.position, Quaternion.identity);
			popup.GetComponent<TextMesh>().text = "3000+";
			
			// Add power up power.
			string name = col.name; // Save name for later so we can add power up ability.
			powerUpSound.Play();
			col.GetComponent<SpriteRenderer>().enabled = false; // Hide the power up until death.
			col.GetComponent<PolygonCollider2D>().enabled = false; // Prevent picking up the power up twice.
			Destroy(col.gameObject, powerUpSound.clip.length);
			
			if(name == "shield"){
				// Add player shield.
				if(shield.activeSelf){
					shield.GetComponent<ShieldBlink>().enabled = false;
					shield.GetComponent<ShieldBlink>().enabled = true;
				}else{
					shield.SetActive(true);
				}
				gameCon.showStatusFade("Shield"); // Give player notice of what power up they picked up.
				shieldTimer = Time.fixedTime + shieldLength;
			}else if(name == "bronze star"){
				// Increase player speed, fire rate, and damage.
				float diff = gameCon.getDifficulty(); // Game's current difficulty rating.
				speed = Mathf.Clamp(speed + Mathf.CeilToInt(1.5f * diff), defaultSpeed, defaultSpeed + 5);
				fireSpeed = Mathf.Clamp(fireSpeed + Mathf.CeilToInt(1.5f * diff), defaultFireSpeed, defaultFireSpeed + 6);
				damage = Mathf.Clamp(damage + Mathf.CeilToInt(20.0f * diff), defaultDamage, defaultDamage + 40);
				gameCon.showStatusFade("Super Charged"); // Give player notice of what power up they picked up.
				isPoweredUp = true;
				powerUpTimer = Time.fixedTime + powerUpLength;
			}else if(name == "gold star"){
				// Fill up player's health.
				gameCon.showStatusFade("Full Healh"); // Give player notice of what power up they picked up.
				health = 100;
				gameCon.updateUI(health);
			}
			
			if(gameCon.isDebug){
				Debug.Log("Player picked up a " + name + " power up.");
			}
		}
		
		// Check for death condition.
		if(health <= 0){
			// Disable collisions and hide the player when dead.
			GetComponent<SpriteRenderer>().enabled = false; // Hide object.
			GetComponent<PolygonCollider2D>().enabled = false; // Turn off collisions.
			GetComponent<Rigidbody2D>().isKinematic = true; // Stop physics.
			
			// Play death sound.
			deathSound.Play();
			
			// Create a spray of small rocks where the meteor died.
			int numOfFrag = 10;
			Vector3 colDirection = trans.position - col.transform.position;
			for(int i = 0; i <= numOfFrag; i++){
				GameObject frag = GameObject.Instantiate(playerFragment, trans.position, Quaternion.identity);
				frag.GetComponent<Rigidbody2D>().AddForce(new Vector3(colDirection.x + Random.Range(-0.5f, 0.5f), colDirection.y + Random.Range(-0.5f, 0.5f), colDirection.z) * Random.Range(600.0f, 800.0f) * Time.deltaTime);
			}
			
			if(gameCon.isDebug){
				Debug.Log("Player has died");
			}
			
			// Notify game controller that the player died.
			gameCon.gameOver();
		}
	}
}
