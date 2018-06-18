using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : Interaction {

    public GameObject shop_prefab;
    public item[] stock;
    public int[] costs;
    private bool[] purchased;

    // Use this for initialization
    void Start () {
        GetComponent<Interactable>().add(this);
        //Shopkeepers get three items to stock, decided on start
        stock = new item[3];
        for(int i = 0; i < stock.Length; i++)
        {
            stock[i] = util_ref.i_manager.get_item_obj();
        }
        //Randomly decide the costs
        costs = new int[stock.Length];
        for(int i = 0; i < costs.Length; i++)
        {
            costs[i] = Random.Range(0, 5) + 1;
        }
        purchased = new bool[stock.Length];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void interact(rpg_character actor)
    {
        GameObject new_shop = GameObject.Instantiate(shop_prefab);
        new_shop.transform.GetChild(0).GetComponent<shop_panel>().set_clerk(this);
    }

    public void try_buy(int index)
    {
        //Check if the player has enough gold
        if(purchased[index] || util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.get_stat_value("Gold") < costs[index])
        {
            return;
        }

        util_ref.p_manager.p_script.inv.pickup(stock[index]);
        util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.get_stat("Gold").set_value(
            util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.get_stat_value("Gold") - costs[index]);
        purchased[index] = true;
    }
}
