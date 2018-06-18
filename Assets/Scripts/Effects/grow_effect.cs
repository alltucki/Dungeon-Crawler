using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grow_effect : effect_component {

    public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(transform.localScale.x + (Time.deltaTime * speed), transform.localScale.y + (Time.deltaTime * speed));
	}
}
