using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupable : Interaction {

    public temp_effect attached_effect;
    public item attached_item;

	// Use this for initialization
	void Start () {
        //attached_item = util_ref.i_manager.get_item_obj();

        if (attached_effect != null)
        {
            attached_effect = Object.Instantiate(attached_effect);
        }
        GetComponent<SpriteRenderer>().sprite = attached_item.icon;

        GetComponent<Interactable>().add(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Detected collision entering " + gameObject.name);
            collision.gameObject.GetComponent<player_script>().inv.pickup(attached_item);

            Destroy(gameObject);
        }
    }
    */
    public override void interact(rpg_character actor)
    {
        if(actor.gameObject.tag == "Player")
        {
            actor.GetComponent<player_script>().inv.pickup(attached_item);

            Destroy(gameObject);
        }
    }
}
