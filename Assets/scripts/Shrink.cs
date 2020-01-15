using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrink : MonoBehaviour {
	
	[Range(0.01f, 10.0f)]
	[Tooltip("Amount to shink each frame update. Higher means fast shrink.")]
	public float step = 0.2f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float x = transform.localScale.x - step * Time.deltaTime;
		float y = transform.localScale.y - step * Time.deltaTime;
		float z = transform.localScale.z - step * Time.deltaTime;
		
		transform.localScale = new Vector3(x, y, z);
		
		if(transform.localScale.x <= 0){
			Destroy(gameObject);
		}
	}
}
