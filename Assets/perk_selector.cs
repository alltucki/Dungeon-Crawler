using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class perk_selector : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void set_perk(Perk perk)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = perk.icon;
        transform.GetChild(1).GetComponent<Text>().text = perk.name;
        transform.GetChild(2).GetComponent<Text>().text = perk.description;
    }
}
