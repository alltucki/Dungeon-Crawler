using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buyable : Interaction {

    public int requires_gold;
    public item associated_item;

    // Use this for initialization
    void Start () {
        GetComponent<Interactable>().add(this);
        requires_gold = Random.Range(1, 3);
        associated_item = util_ref.i_manager.get_item_obj();
        GetComponent<SpriteRenderer>().sprite = associated_item.icon;
        GetComponent<SpriteRenderer>().color = Color.yellow;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void interact(rpg_character actor)
    {
        if(actor.stats.get_stat_value("Gold") < requires_gold)
        {
            return;
        }
        else
        {
            actor.stats.get_stat("Gold").set_value(actor.stats.get_stat_value("Gold") - requires_gold);
            actor.GetComponent<player_script>().inv.pickup(associated_item);
            Destroy(gameObject);
        }
    }
}
