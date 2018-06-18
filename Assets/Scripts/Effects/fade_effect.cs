using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fade_effect : effect_component {

    public float speed;
    float alpha;
    SpriteRenderer s_renderer;
    Image image;

	// Use this for initialization
	void Start () {
        alpha = 1f;
        s_renderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        alpha -= Time.deltaTime * speed;
        if (s_renderer != null)
        {
            s_renderer.color = new Color(1f, 1f, 1f, alpha);
        }
        else if(image != null)
        {
            image.color = new Color(1f, 1f, 1f, alpha);
        }
        if(alpha <= 0f)
        {
            Destroy(gameObject);
        }
	}
}
