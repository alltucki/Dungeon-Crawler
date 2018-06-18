using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class torch_flicker : MonoBehaviour {

    Light attached_light;
    public float low_limit, high_limit;
    float countdown;
    public float flicker_interval;


	// Use this for initialization
	void Start () {
        attached_light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        countdown += Time.deltaTime;
        if(countdown > flicker_interval)
        {
            countdown = 0;
            flicker();
        }
	}

    public void flicker()
    {
        float flicker = .5f;
        flicker *= Random.Range(-1, 2);
        attached_light.intensity += flicker;
        if (attached_light.intensity > high_limit)
        {
            attached_light.intensity = high_limit;
            attached_light.intensity += flicker * -1f;
        }
        if (attached_light.intensity < low_limit)
        {
            attached_light.intensity = low_limit;
            attached_light.intensity += flicker * -1f;
        }
    }
}
