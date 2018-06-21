using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_manager : MonoBehaviour {

    public GameObject player_holder, cur_player, torch_prefab;
    public player_script p_script;
    public equipment[] starting_equipment;
    public Perk[] starting_perks;
    private map_feature_spawner features;
    private bool game_over;
    private Color to_red;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        features = GameObject.Find("Map Manager").GetComponent<map_feature_spawner>();
	}
	
	// Update is called once per frame
	void Update () {
        if(p_script != null && cur_player.GetComponent<rpg_character>().stats.get_stat_value("Health") <= 0)
        {
            game_over = true;
            to_red = cur_player.GetComponent<SpriteRenderer>().color;
        }
        if(game_over)
        {
            to_red = new Color(to_red.r + Time.deltaTime, to_red.g - Time.deltaTime, to_red.b - Time.deltaTime);
            cur_player.GetComponent<SpriteRenderer>().color = to_red;
        }
	}

    public void load_character(GameObject player_ref)
    {
        player_holder = player_ref.GetComponent<hover_box>().attached_player;
        starting_equipment = player_ref.GetComponent<hover_box>().equipment;
        starting_perks = player_ref.GetComponent<hover_box>().perks;

        GameObject.Find("Menu Manager").GetComponent<menu_manager>().launch_god_menu();
    }

    public GameObject init_player()
    {
        GameObject new_player = GameObject.Instantiate(player_holder);
        features.spawn_feature(new_player);
        cur_player = new_player;
        p_script = cur_player.GetComponent<player_script>();

        GameObject new_torch = GameObject.Instantiate(torch_prefab);
        new_torch.transform.SetParent(new_player.transform);
        new_torch.transform.localPosition = new Vector3(.5f, .5f, -2f);
        
        return new_player;
    }

    public void equip_starting_items()
    {
        for (int i = 0; i < starting_equipment.Length; i++)
        {
            p_script.inv.equip(starting_equipment[i]);
        }
    }

    public void equip_starting_perks()
    {
        for(int i = 0; i < starting_perks.Length; i++)
        {
            util_ref.perks.add_perk(Object.Instantiate(starting_perks[i]));
        }
    }

    public Vector2Int get_p_pos()
    {
        if(cur_player.GetComponent<rpg_character>().target_position.x == 0 && cur_player.GetComponent<rpg_character>().target_position.y == 0)
        {
            return new Vector2Int((int)cur_player.transform.position.x, (int)cur_player.transform.position.y);
        }
        return new Vector2Int((int)cur_player.GetComponent<rpg_character>().target_position.x, (int)cur_player.GetComponent<rpg_character>().target_position.y);
    }
}
