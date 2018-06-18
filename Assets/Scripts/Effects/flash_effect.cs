using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flash_effect : effect_component {

    public int max_loops;
    private int loops;
    private float cur_time;
    private bool renderer_on;
    SpriteRenderer s_renderer;

	// Use this for initialization
	void Start () {
        s_renderer = GetComponent<SpriteRenderer>();
        renderer_on = true;
	}
	
	// Update is called once per frame
	void Update () {
        cur_time += Time.deltaTime;
        if(cur_time > .5f)
        {
            cur_time = 0f;
            if(renderer_on)
            {
                s_renderer.enabled = false;
                renderer_on = false;
            }
            else
            {
                s_renderer.enabled = true;
                renderer_on = true;
                loops++;
                if(loops > max_loops)
                {
                    Destroy(gameObject);
                }
            }
        }
	}
}
