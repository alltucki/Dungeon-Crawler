using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shop_item_panel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void set_item(item new_item, int cost)
    {
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = new_item.icon;
        transform.GetChild(1).GetChild(0).GetComponent<Text>().text = cost.ToString();
    }
}
