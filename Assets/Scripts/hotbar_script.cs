using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hotbar_script : MonoBehaviour {

    private int last_child_count;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(transform.childCount != last_child_count)
        {
            recalculate_numbers();
        }
	}

    public void recalculate_numbers()
    {
        last_child_count = transform.childCount;
        for (int i = 0; i < last_child_count; i++)
        {
            transform.GetChild(i).GetChild(0).GetComponent<Text>().text = i.ToString();
        }
    }

    public void set_all_white()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    public void set_highlight(int target)
    {
        transform.GetChild(target).GetComponent<Image>().color = Color.yellow;
    }

    public void change_sprite(int target, Sprite new_image)
    {
        transform.GetChild(target).GetComponent<Image>().sprite = new_image;
    }
}
