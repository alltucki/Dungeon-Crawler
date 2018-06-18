using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bounce_effect : MonoBehaviour {

    float start_y, distance;
    bool has_bounced;

	// Use this for initialization
	void Start () {
        start_y = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        distance += Time.deltaTime;
        if (distance < .25f)
        {
            transform.position = transform.position + new Vector3(0f, Time.deltaTime * 3f, 0f);
        }
        else if(!has_bounced && distance > .25f)
        {
            transform.position = transform.position - new Vector3(0f, Time.deltaTime * 5f, 0f);
            if(transform.position.y <= start_y)
            {
                transform.position = new Vector3(transform.position.x, start_y, 0f);
                has_bounced = true;
            }
        }
	}
}
