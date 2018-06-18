using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkable : Interaction
{
    private void Start()
    {
        GetComponent<Interactable>().add(this);
    }

    public override void interact(rpg_character actor)
    {
        
    }
}
