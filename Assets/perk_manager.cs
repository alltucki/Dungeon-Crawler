using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perk_manager : MonoBehaviour {

    public Perk[] perk_ref;
    public List<Perk> active_perks;
    public int test_index, progression_trigger;
    public bool test_add, print_values, test_canvas;
    bool initialized;
    public GameObject canvas_prefab;
    public Perk option_a, option_b, option_c;

	// Use this for initialization
	void Start () {
        
	}

    void init()
    {
        util_ref.events.start_listening("descend", descent);
        initialized = true;
    }
	
	// Update is called once per frame
	void Update () {
        if(!initialized)
        {
            init();
        }
		if(test_add)
        {
            Perk new_perk = Object.Instantiate(perk_ref[test_index]);
            Debug.Log("Added " + new_perk.name);
            active_perks.Add(new_perk);
            new_perk.add_perk();
            test_add = false;
        }
        if(print_values)
        {
            util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.print_values();
            print_values = false;
        }
        if(test_canvas)
        {
            GameObject.Instantiate(canvas_prefab);
            test_canvas = false;
        }
	}

    public void descent()
    {
        progression_trigger++;
        if(progression_trigger >= 2)
        {
            progression_trigger = 0;
            GameObject.Instantiate(canvas_prefab);
        }
    }

    public void add_perk()
    {
        Perk new_perk = Object.Instantiate(perk_ref[test_index]);
        Debug.Log("Added " + new_perk.name);
        active_perks.Add(new_perk);
        new_perk.add_perk();
    }

    public void add_perk(Perk new_perk)
    {
        Debug.Log("Added " + new_perk.name);
        active_perks.Add(new_perk);
        new_perk.add_perk();
    }
}
