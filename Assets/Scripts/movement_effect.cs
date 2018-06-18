using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement_effect : effect_component {

    public Vector3 movement_speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += movement_speed * Time.deltaTime;
	}
}
