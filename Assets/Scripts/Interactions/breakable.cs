using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakable : Interaction {

    public int health;

    private void Start()
    {
        GetComponent<Interactable>().add(this);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public override void interact(rpg_character actor)
    {
        health--;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
