using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class add_stat_interact : Interaction {

    public string target_stat;
    public int add_amount;

    // Use this for initialization
    void Start()
    {
        GetComponent<Interactable>().add(this);
    }

    public override void interact(rpg_character actor)
    {
        actor.stats.get_stat(target_stat).set_value(actor.stats.get_stat_value(target_stat) + add_amount);
    }
}
