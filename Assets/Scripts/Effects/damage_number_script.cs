using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage_number_script : MonoBehaviour {

    public float ttl, cur_life;
    public GameObject child_number;
    private SpriteRenderer sprite;
    public Sprite[] numbers;

	// Use this for initialization
	void Start () {
        set_child();
        set_sprite();
	}
	
	// Update is called once per frame
	void Update () {
        cur_life -= Time.deltaTime;
        rise();
        fade();

        if(cur_life <= 0f)
        {
            Destroy(child_number);
            Destroy(gameObject);
        }
	}

    private void rise()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime);
    }

    private void fade()
    {
        set_child();
        set_sprite();
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, cur_life / ttl);
        child_number.GetComponent<SpriteRenderer>().color = sprite.color;
    }

    public void set_ttl(float new_time)
    {
        ttl = new_time;
        cur_life = new_time;
    }

    public void set_number(int num)
    {
        //Debug.Log("Set damage number to " + num.ToString());
        set_child();
        child_number.GetComponent<SpriteRenderer>().sprite = numbers[num];
    }

    private void set_child()
    {
        if(child_number == null)
            child_number = transform.GetChild(0).gameObject;
    }

    private void set_sprite()
    {
        if(sprite == null)
            sprite = GetComponent<SpriteRenderer>();
    }
}
