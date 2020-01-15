using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour {
	
	public enum Size{Small, Medium, Large}
	
	[Tooltip("The size of the meteor. Bigger meteors spin slower.")]
	public Size size = Size.Medium;
	[Tooltip("This is the model of the fragments that show up when the meteor is destroyd.")]
	public GameObject meteorFragment = null;
	
	private GameController gameCon;
	private Transform trans; // The transform of this object.
	private float rotation; // The speed of rotation.
	private Vector3 direction; // The rotation direction of the meteor.
	private int health; // How much health the meteor has.
	private int damage; // How much damage the meteor does when it collides with other objects.
	private float halfSpriteHeight; // Half of this sprite's height.
	private PolygonCollider2D polyCol; // This object's PolygonCollider2D.
	private Rigidbody2D body; // This object's Rigidbody2D;
	private bool isDead; // Denotes if this meteor is still alive or not.
	private int score; // How many point destroying this meteor gives the player.
	private float difficulty; // The difficulty level of the game when this meteor is created.
	private float defaultGrav; // The default amount of gravity this meteor has.
	
	// Use this for initialization
	void Start () {
		// Get the game controller.
		gameCon = (GameController) GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		
		// Get this object's transform.
		trans = GetComponent<Transform>();
		
		// Randomly decide how fast meteor will spin.
		difficulty = gameCon.getDifficulty();
		if(size == Size.Small){
			rotation = Random.Range(100.0f, 140.0f);
			health = Mathf.CeilToInt(40 * difficulty);
			damage = Mathf.CeilToInt(10 * difficulty);
			score = Mathf.CeilToInt(100 * difficulty);
		}else if(size == Size.Medium){
			rotation = Random.Range(40.0f, 90.0f);
			health = Mathf.CeilToInt(60 * difficulty);
			damage = Mathf.CeilToInt(40 * difficulty);
			score = Mathf.CeilToInt(500 * difficulty);
		}else if(size == Size.Large){
			rotation = Random.Range(10.0f, 30.0f);
			health = Mathf.CeilToInt(100 * difficulty);
			damage = Mathf.CeilToInt(50 * difficulty);
			score = Mathf.CeilToInt(1000 * difficulty);
		}
		
		// Randomly decide which direction this metoer will spin. Should be roughly 50/50.
		if(Random.Range(0.0f, 1.0f) < 0.5f){
			direction = Vector3.back;
		}else{
			direction = Vector3.forward; 
		}
		
		// Gets player sprite specs for calculating the bounds of the sprite.
		Vector2 spriteSize = GetComponent<SpriteRenderer>().sprite.rect.size;
		float spritePPU = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
		
		// Calculates the bounds of the sprite in world coordinates.
		halfSpriteHeight = spriteSize.y / spritePPU / 2;
		
		polyCol = GetComponent<PolygonCollider2D>();
		body = GetComponent<Rigidbody2D>();
		
		isDead = false;
		
	   // Update the gravity based on dificulty.
		defaultGrav = 0.3f;
		body.gravityScale = defaultGrav * difficulty; // 0.3f is default gravity.
	}
	
	// Update is called once per frame
	void Update () {
		// Check to see if meteor is onscreen yet. If so, turn on the meteor's collider.
		// This prevents being able to destroy the meteor before it is onscreen.
		if(polyCol.enabled == false && !isDead){
			if(trans.position.y - halfSpriteHeight < gameCon.getScreenRect().height){
				polyCol.enabled = true;
			}
		}

		// Rotate the meteor.
		float step = Time.deltaTime * rotation;
		trans.Rotate(direction * step);
		
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
	
	public int getDamage(){
		return damage;
	}
	
	void OnCollisionEnter2D(Collision2D col){
		if(gameCon.isDebug){
			Debug.Log(trans.name + " collided with " + col.transform.name);
		}
		
		if(health > 0){
			if(col.transform.tag == "Meteor"){
				//Collision by another meteor occurred.
				health -= col.transform.GetComponent<MeteorController>().getDamage();
			}else if(col.transform.tag == "Laser"){
				// Collision by a laser occurred.
				health -= col.transform.GetComponent<LaserController>().getDamage();
			}
			
			// Blink meteor red.
			GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		}
		
		checkForDeath(col.transform.position);
	}
	
	void OnTriggerEnter2D(Collider2D col){
		if(gameCon.isDebug){
			Debug.Log(trans.name + " collided with " + col.name);
		}
		
		if(health > 0){
			if(col.tag == "Player"){
				//Collision by player occurred.
				health = 0;
			}
		}
		
		checkForDeath(col.transform.position);
	}
	
	private void checkForDeath(Vector3 colPos){
		// Check for meteor death.
		if(health <= 0){
			// Disable collisions and hide the meteor when dead.
			GetComponent<SpriteRenderer>().enabled = false; // Hide object.
			polyCol.enabled = false; // Turn off collisions.
			isDead = true; // Ensure collisions remain off.
			GetComponent<Rigidbody2D>().isKinematic = true; // Stop physics.
			
			// Create a spray of small rocks where the meteor died.
			int numOfFrag = 6;
			if(size == Size.Small){
				numOfFrag = 6;
			}else if(size == Size.Medium){
				numOfFrag = 8;
			}else if(size == Size.Large){
				numOfFrag = 10;
			}
			Vector3 colDirection = trans.position - colPos;
			for(int i = 0; i <= numOfFrag; i++){
				GameObject frag = GameObject.Instantiate(meteorFragment, trans.position, Quaternion.identity);
				frag.GetComponent<Rigidbody2D>().AddForce(new Vector3(colDirection.x + Random.Range(-0.5f, 0.5f), colDirection.y + Random.Range(-0.5f, 0.5f), colDirection.z) * Random.Range(600.0f, 800.0f) * Time.deltaTime);
			}
			
			// Check for power up children and release them before death.
			if(trans.childCount > 0){
				for(int i = 0; i < trans.childCount; i++){
					Transform child = trans.GetChild(i);
					child.parent = null; // Remove power up child from meteor before meteor dies.
					
					if(child.name == "score"){
						child.GetComponent<TextMesh>().text = score + "+";
						child.gameObject.SetActive(true); // Enable text object.
					}else{
						child.transform.rotation = Quaternion.identity; // Make sure power up faces the right way.
						child.GetComponent<PowerUpBlink>().enabled = true; // Start power up blinking.
						child.GetComponent<PolygonCollider2D>().enabled = true; // Allow player to pick up power up now.
						child.GetComponent<AudioSource>().enabled = true; // Play power up spawn noise.
					}
				}
			}
			
			// Add the score for this meteor to the overall score.
			gameCon.addToScore(score);
			
			// Destroy this meteor after playing the explosion sound.
			AudioSource explosionSound = GetComponent<AudioSource>();
			explosionSound.Play();
			Destroy(gameObject, explosionSound.clip.length); // Remove the object as soon as the sound is done playing.
		}
	}
}
