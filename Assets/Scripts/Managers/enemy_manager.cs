using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class enemy_manager : MonoBehaviour {

    public int level, budget, max_budget;
                                                    //We'll need to expand the *_count if additional enemy classes are added
    private int p_start_health;                     //The player's health on starting the current floor
    private int s_count, a_count, m_count;          //The number of enemies on the current level
    private float s_weight, a_weight, m_weight;     //The weights to the enemy costs

    public List<GameObject> enemies, corpses;
    private fov_manager f_manager;
    private map_feature_spawner features;
    private dijkstra_manager d_manager;
    public GameObject target_square_prefab;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        features = GameObject.Find("Map Manager").GetComponent<map_feature_spawner>();
        d_manager = GameObject.Find("Dijkstra Manager").GetComponent<dijkstra_manager>();
        f_manager = GameObject.Find("Map Manager").GetComponent<fov_manager>();
        s_weight = 1f;
        a_weight = 1f;
        m_weight = 1f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Hide enemies that shouldn't be seen through fog of war
    public void hide_enemies()
    {
        foreach(GameObject e in enemies)
        {
            Vector2Int e_pos = new Vector2Int((int)e.transform.position.x, (int)e.transform.position.y);
            if(!f_manager.is_visible[e_pos.x,e_pos.y])
            {
                e.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                e.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    //Spawn enemies based on our budget
    public void init_enemies()
    {
        budget = max_budget;
        GameObject[] e_palette = GameObject.Find("Palette").GetComponent<palette>().enemies;
        int i = 0;
        if (p_start_health == 0)
        {
            p_start_health = 10;
        }
        else
        {
            p_start_health = util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.get_stat_value("Health");
        }
        //Set count of enemy classes to zero
        s_count = 0;
        a_count = 0;
        m_count = 0;

        //Spawn at least one enemy, and no more than 50, as an arbitrary upper limit
        while (budget > 0 && i < 50)
        {
            GameObject new_enemy = GameObject.Instantiate(e_palette[Random.Range(0, e_palette.Length)]);
            new_enemy.name = "Enemy " + (i++).ToString();
            features.spawn_feature(new_enemy);
            enemies.Add(new_enemy);
            budget -= get_enemy_cost(new_enemy);
        }
        //If we broke 50 enemies, something probably went wrong
        if(i > 50)
        {
            Debug.LogError("Spawning enemies timed out");
        }
    }

    //Change our encounter budget based on the player's performance on the previous floor
    private void recalculate_budget()
    {
        int p_cur_health = util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.get_stat_value("Health");
        int h_dif = p_cur_health - p_start_health;

        //Goal is to deal about two damage on each floor
        //If we've dealt more than 2 damage, adjust the weights of the enemies upward
        if(h_dif < -2)
        {
            if(s_count > 0)
            {
                s_weight += 1f;
            }
            if(a_count > 0)
            {
                a_weight += 1f;
            }
            if(m_count > 0)
            {
                m_weight += 1f;
            }
        }
        //Otherwise if the player only lost one health, or gained health, decrease the weights
        else if(h_dif > -1)
        {
            if (s_count > 0)
            {
                s_weight -= .5f;
                if(s_weight <= 0f)
                {
                    s_weight = .1f;
                }
            }
            if (a_count > 0)
            {
                a_weight -= .5f;
                if (a_weight <= 0f)
                {
                    a_weight = .1f;
                }
            }
            if (m_count > 0)
            {
                m_weight -= .5f;
                if (m_weight <= 0f)
                {
                    m_weight = .1f;
                }
            }
        }
    }

    //Get the cost of an enemy based on their class
    private int get_enemy_cost(GameObject enemy)
    {
        int start_cost = enemy.GetComponent<enemy_script>().cost;
        switch(enemy.GetComponent<enemy_script>().enemy_type)
        {
            case enemy_class.artillery:
                a_count++;
                start_cost = (int)(a_weight * start_cost);
                break;
            case enemy_class.soldier:
                s_count++;
                start_cost = (int)(s_weight * start_cost);
                break;
            case enemy_class.minion:
                m_count++;
                start_cost = (int)(m_weight * start_cost);
                break;
        }
        return start_cost;
    }

    //Delete all enemies and corpses
    public void unload_enemies()
    {
        int count = enemies.Count;
        for(int i = 0; i < count; i++)
        {
            Destroy(enemies[i]);
        }
        enemies.RemoveRange(0, count);

        count = corpses.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(corpses[i]);
        }
        corpses.RemoveRange(0, count);
        recalculate_budget();
        Debug.Log("Recalculated budget");
        Debug.Log("Current weights: ");
        Debug.Log("Soldier weight: " + s_weight.ToString());
        Debug.Log("Artillery weight: " + a_weight.ToString());
        Debug.Log("Minion weight: " + m_weight.ToString());
    }

    //Check if there's an enemy at a given point
    public bool is_occupied(Vector2Int coord)
    {
        foreach(GameObject e in enemies)
        {
            Vector2Int e_pos = new Vector2Int((int)e.transform.position.x, (int)e.transform.position.y);
            if(e_pos.x == coord.x && e_pos.y == coord.y)
            {
                return true;
            }
        }

        return false;
    }

    //Get the enemy at a given point
    public GameObject get_at_square(Vector2Int coord)
    {
        //Debug.Log("Searching for enemy at " + coord.ToString());
        foreach (GameObject e in enemies)
        {
            Vector2Int e_pos = new Vector2Int((int)e.transform.position.x, (int)e.transform.position.y);
            if (e_pos.x == coord.x && e_pos.y == coord.y)
            {
                //Debug.Log("Found enemy " + e.name);
                return e;
            }
        }
        //Debug.Log("Did not find any enemies");
        return null;
    }
}
