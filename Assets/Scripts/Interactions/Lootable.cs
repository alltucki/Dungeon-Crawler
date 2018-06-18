using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : Interaction
{
    bool has_looted;

    void Start()
    {
        GetComponent<Interactable>().add(this);
    }

    //Drop item chosen from item manager
    public override void interact(rpg_character actor)
    {
        if(has_looted)
        {
            return;
        }
        item drop_item = util_ref.cur_palette.container_drops.get_weighted_item();
        GameObject looted_item = GameObject.Instantiate(util_ref.pickup_prefab);
        looted_item.transform.position = transform.position;
        looted_item.GetComponent<pickupable>().attached_item = drop_item;
        has_looted = true;
    }
}
