using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {
	
	private int damage; // How much damage the laser does on impact. This is set by the source of the shot.
	
	// Use this for initialization
	void Start () {
		damage = 20;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void setDamage(int dmg){
		damage = dmg;
	}
	
	public int getDamage(){
		return damage;
	}
	
	void OnCollisionEnter2D(){
		// Hide object before playing sound effects.
		GetComponent<SpriteRenderer>().enabled = false; // Hide the visual object.
		GetComponent<CapsuleCollider2D>().enabled = false; // Turn off collisions with hidden laser.
		
		// Play laser destruction sound and remove object when done.
		AudioSource hitSound = GetComponents<AudioSource>()[0];
		hitSound.Play();
		Destroy(gameObject, hitSound.clip.length);
	}
}
