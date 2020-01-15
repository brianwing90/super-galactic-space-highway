using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class ScoreController : MonoBehaviour {
	
	private TextMesh textMesh;
	private float step; // How much to grow text by;
	private bool shrink; // Whether to grow or shrink.
	
	// Use this for initialization
	void Start () {
		textMesh = GetComponent<TextMesh>();
		step = 0.25f;
		shrink = false;
		transform.rotation = Quaternion.identity; // Ensure text is displayed upright.
	}
	
	// Update is called once per frame
	void Update () {
		if(!shrink){
			textMesh.characterSize += step * Time.deltaTime;
			
			if(textMesh.characterSize >= 0.2f){
				shrink = true; // Shrink back down now.
			}
		}else{
			textMesh.characterSize -= step * Time.deltaTime;
			
			if(textMesh.characterSize <= 0.0f){
				Destroy(gameObject);
			}
		}
	}
}
