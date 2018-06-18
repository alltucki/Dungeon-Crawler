using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum enemy_class
{
    minion, artillery, soldier, brute, prop
}

public class enemy_script : MonoBehaviour {

    private rpg_character attached_character;
    public int cost, health_template, attack_template, move_template, shared_template;
    public GameObject dmg_effect_prefab, damage_prefab, death_effect_prefab, corpse;
    public GameObject targeting_square, current_target;
    public enemy_class enemy_type;
    public LayerMask blocking_layer;

	// Use this for initialization
	void Start () {
        attached_character = GetComponent<rpg_character>();

        attached_character.stats.init_character();
        attached_character.stats.set_stat_max("Health", health_template);
        attached_character.stats.set_stat_to_max("Health");
        attached_character.stats.set_stat("Max Move", move_template);
        attached_character.stats.set_stat("Max Attack", attack_template);
        attached_character.stats.set_stat("Max Shared", shared_template);

        GetComponent<StateController>().attached_character = attached_character;
        attached_character.current_target_character = util_ref.p_manager.cur_player.GetComponent<rpg_character>();
    }
	
	// Update is called once per frame
	void Update () {
		if(attached_character.is_moving)
        {
            if(attached_character.target_position.x == transform.position.x && attached_character.target_position.y == transform.position.y)
            {
                attached_character.is_moving = false;
            }
        }
	}

    public void die()
    {
        GameObject death_effect = GameObject.Instantiate(death_effect_prefab);
        death_effect.transform.position = transform.position + new Vector3(0, 1, 0);

        GameObject body = GameObject.Instantiate(corpse);
        body.transform.position = transform.position;

        util_ref.e_manager.enemies.Remove(gameObject);
        util_ref.e_manager.corpses.Add(body);

        if (current_target != null)
        {
            Destroy(current_target);
        }

        Destroy(gameObject);
    }


    /*
    private bool can_see_player()
    {
        return util_ref.d_manager.f_manager.is_visible[(int)transform.position.x, (int)transform.position.y];
    }

    private void move_adj(List<dijkstra_coord> adj)
    {
        if (adj.Count > 1)
        {
            //Shuffle the array to ensure random movement if equal cost
            for (int i = adj.Count - 1; i > 0; i--)
            {
                //Debug.Log("Shuffling position " + i.ToString());
                int r = Random.Range(0, i);
                dijkstra_coord tmp = adj[i];
                adj[i] = adj[r];
                adj[r] = tmp;
            }
        }

        int lowest_cost = 99;
        int lowest_index = -1;

        for(int i = 0; i < adj.Count; i++)
        {
            //Debug.Log("Checking score of position " + i.ToString());
            if(adj[i].score < lowest_cost)
            {
                lowest_index = i;
                lowest_cost = adj[i].score;
            }
        }

        move(adj[lowest_index].relative_coord.x, adj[lowest_index].relative_coord.y);
        attached_character.decrease_move();
    }
    */
    public void target_square(Vector3 base_square)
    {
        GameObject new_target = GameObject.Instantiate(targeting_square);
        current_target = new_target;

        int horizontal_offset = Random.Range(-1, 2);
        int vertical_offset = Random.Range(-1, 2);

        new_target.transform.position = base_square + new Vector3(horizontal_offset, vertical_offset);
    }

    public bool is_valid_square(int x_offset, int y_offset)
    {
        attached_character.hitbox.enabled = false;
        RaycastHit2D hit;
        Vector3 start = transform.position + new Vector3(.5f, .5f, 0f);
        Vector3 offset = new Vector3(transform.position.x + x_offset, transform.position.y + y_offset);
        Vector3 end = offset + new Vector3(.5f, .5f, 0f);
        hit = Physics2D.Linecast(start, end, blocking_layer);

        attached_character.hitbox.enabled = true;
        if (hit)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
}
