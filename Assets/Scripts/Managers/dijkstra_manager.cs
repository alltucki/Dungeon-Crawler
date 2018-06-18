using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * Djiksta maps!
 * We create a two dimensional array of ints
 * that stores the shortest path to a given point
 * This is useful for enemy pathfinding, since we only need
 * to find the shortest path to the player once per turn loop
 */ 
public class dijkstra_manager : MonoBehaviour {

    public int[,] player_distance, player_sight, enemy_location;        //Two dimensional goal arrays
    public map_generator dungeon_map;                                   //These need to change to references to util ref
    public fov_manager f_manager;
    public player_manager p_manager;
    public LayerMask blocking_layer;
    public int max_value;                                               //Maximum value a square can be set to. Higher means better quality pathfinding, but longer to calculate
    public bool debug_dist, debug_sight, debug_enem, made_change;       //Controls whether we paint the squares for the maps
    private Gradient g;                                                 //Gradient used for painting, easier than hard coding values
    private GradientColorKey[] gck;
    private GradientAlphaKey[] gak;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        dungeon_map = GameObject.Find("Map Manager").GetComponent<map_generator>();
        f_manager = GameObject.Find("Map Manager").GetComponent<fov_manager>();
        p_manager = GameObject.Find("Player Manager").GetComponent<player_manager>();

        player_distance = new int[util_ref.m_gen.x_dim, util_ref.m_gen.y_dim];
        player_sight = new int[util_ref.m_gen.x_dim, util_ref.m_gen.y_dim];
        enemy_location = new int[util_ref.m_gen.x_dim, util_ref.m_gen.y_dim];

        init_gradient();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Create a gradient from green to red
    public void init_gradient()
    {
        g = new Gradient();

        gak = new GradientAlphaKey[2];
        gak[0].alpha = 1f;
        gak[0].time = 0f;
        gak[1].alpha = 1f;
        gak[1].time = 1f;

        gck = new GradientColorKey[2];
        gck[0].color = Color.green;
        gck[0].time = 0f;
        gck[1].color = Color.red;
        gck[1].time = 1f;

        g.SetKeys(gck, gak);
        g.mode = GradientMode.Blend;
    }

    //Goal: The player's current position
    public void generate_player_map()
    {
        float start_time = Time.realtimeSinceStartup;

        //Set initial distance
        for (int x = 0; x < dungeon_map.x_dim; x++)
        {
            for (int y = 0; y < dungeon_map.y_dim; y++)
            {
                if (dungeon_map.is_floor[x, y])
                {
                    player_distance[x,y] = max_value;
                }
            }
        }
        Vector2Int p_pos = new Vector2Int((int)p_manager.cur_player.GetComponent<rpg_character>().target_position.x, (int)p_manager.cur_player.GetComponent<rpg_character>().target_position.y);
        player_distance[p_pos.x, p_pos.y] = 0;

        player_distance = calculate_map(player_distance);
        float end_time = Time.realtimeSinceStartup;
        

        //Color map
        if (debug_dist)
        {
            Debug.Log("Calculated player_distance map. Time: " + (end_time - start_time).ToString());
            //Debug.Log("Map contains " + player_distance.Count.ToString() + " entries");
            for (int x = 0; x < dungeon_map.x_dim; x++)
            {
                for (int y = 0; y < dungeon_map.y_dim; y++)
                {
                    if (dungeon_map.is_floor[x, y])
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);
                        dungeon_map.floor_map.SetTileFlags(pos, UnityEngine.Tilemaps.TileFlags.None);
                        int value_i = player_distance[x, y];
                        Color tile_color = g.Evaluate((float)value_i / (float)max_value);
                        dungeon_map.floor_map.SetColor(pos, tile_color);
                    }
                }
            }
        }
    }

    //Goal: squares the player CANNOT see
    public void generate_player_sight_map()
    {
        float start_time = Time.realtimeSinceStartup;

        for (int x = 0; x < dungeon_map.x_dim; x++)
        {
            for (int y = 0; y < dungeon_map.y_dim; y++)
            {
                if (dungeon_map.is_floor[x, y])
                {
                    if(f_manager.is_visible[x,y])
                    {
                        player_sight[x, y] = max_value;
                    }
                    else
                    {
                        player_sight[x, y] = 0;
                    }
                }
            }
        }

        player_sight = calculate_map(player_sight);
        float end_time = Time.realtimeSinceStartup;

        //Color map
        if (debug_sight)
        {
            Debug.Log("Calculated player_sight map. Time: " + (end_time - start_time).ToString());
            Debug.Log("Map contains " + player_sight.Length + " entries");
            for (int x = 0; x < dungeon_map.x_dim; x++)
            {
                for (int y = 0; y < dungeon_map.y_dim; y++)
                {
                    if (dungeon_map.is_floor[x, y])
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);
                        dungeon_map.floor_map.SetTileFlags(pos, UnityEngine.Tilemaps.TileFlags.None);
                        int value_i = player_sight[x, y];
                        Color tile_color = g.Evaluate((float)value_i / (float)max_value);
                        if (debug_dist)
                        {
                            Color cur_color = dungeon_map.floor_map.GetColor(pos);
                            dungeon_map.floor_map.SetColor(pos, tile_color + cur_color);
                        }
                        else
                        {
                            dungeon_map.floor_map.SetColor(pos, tile_color);
                        }
                    }
                }
            }
        }
    }

    //Goal: squares occupied by an enemy
    public void generate_enemy_location_map()
    {
        float start_time = Time.realtimeSinceStartup;

        //Set initial distance
        for (int x = 0; x < dungeon_map.x_dim; x++)
        {
            for (int y = 0; y < dungeon_map.y_dim; y++)
            {
                if (dungeon_map.is_floor[x, y])
                {
                    enemy_location[x, y] = max_value;
                }
            }
        }

        for(int i = 0; i < util_ref.e_manager.enemies.Count; i++)
        {
            Vector2Int pos = new Vector2Int((int)util_ref.e_manager.enemies[i].transform.position.x, (int)util_ref.e_manager.enemies[i].transform.position.y);
            enemy_location[pos.x, pos.y] = 0;
        }

        enemy_location = calculate_map(enemy_location);
        float end_time = Time.realtimeSinceStartup;

        if(debug_enem)
        {
            Debug.Log("Calculated enemy_location map. Time: " + (end_time - start_time).ToString());
            //Debug.Log("Map contains " + player_distance.Count.ToString() + " entries");
            for (int x = 0; x < dungeon_map.x_dim; x++)
            {
                for (int y = 0; y < dungeon_map.y_dim; y++)
                {
                    if (dungeon_map.is_floor[x, y])
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);
                        dungeon_map.floor_map.SetTileFlags(pos, UnityEngine.Tilemaps.TileFlags.None);
                        int value_i = enemy_location[x, y];
                        Color tile_color = g.Evaluate((float)value_i / (float)max_value);
                        dungeon_map.floor_map.SetColor(pos, tile_color);
                    }
                }
            }
        }
    }

    //Loop through the map until we've stopped making changes
    //Note changes using a member variable, made_change
    public int[,] calculate_map(int[,] map)
    {
        int[,] return_map = map;
        bool nochange = false;
        while(!nochange)
        {
            nochange = true;
            for(int x = 0; x < dungeon_map.x_dim; x++)
            {
                for(int y = 0; y < dungeon_map.y_dim; y++)
                {
                    if(dungeon_map.is_floor[x,y])
                    {
                        return_map = check_adjacent(x, y, return_map);
                        if (made_change)
                        {
                            nochange = false;
                        }
                    }
                }
            }
        }
        return return_map;
    }

    /*
     * Check a given square of a Djikstra map to see if it needs to be changed
     * If the value of the current square is greater than the value of the
     * adjacent square plus one, change the value
     */ 
    public int[,] check_adjacent(int x, int y, int[,] map)
    {
        made_change = false;
        int cur_value = map[x, y];
        Vector2Int up, down, left, right;
        up = new Vector2Int(x, y + 1);
        down = new Vector2Int(x, y - 1);
        left = new Vector2Int(x - 1, y);
        right = new Vector2Int(x + 1, y);

        int lowest_value = 99;

        if (is_valid(up) && cur_value > map[up.x, up.y] + 1)
        {
            if(map[up.x, up.y] + 1 < lowest_value)
            {
                lowest_value = map[up.x, up.y] + 1;
                made_change = true;
            }            
        }
        if (is_valid(down) && cur_value > map[down.x, down.y] + 1)
        {
            if (map[down.x, down.y] + 1 < lowest_value)
            {
                lowest_value = map[down.x, down.y] + 1;
                made_change = true;
            }
        }
        if (is_valid(left) && cur_value > map[left.x, left.y] + 1)
        {
            if (map[left.x, left.y] + 1 < lowest_value)
            {
                lowest_value = map[left.x, left.y] + 1;
                made_change = true;
            }
        }
        if (is_valid(right) && cur_value > map[right.x, right.y] + 1)
        {
            if (map[right.x, right.y] + 1 < lowest_value)
            {
                lowest_value = map[right.x, right.y] + 1;
                made_change = true;
            }
        }

        if(made_change)
        {
            map[x, y] = lowest_value;
        }

        return map;
    }

    //Get a list of all adjacent dijkstra coordinates
    public List<dijkstra_coord> get_adjacent(int x, int y, int[,] map)
    {
        List<dijkstra_coord> return_list = new List<dijkstra_coord>();

        Vector2Int up, down, left, right;
        up = new Vector2Int(x, y + 1);
        down = new Vector2Int(x, y - 1);
        left = new Vector2Int(x - 1, y);
        right = new Vector2Int(x + 1, y);

        if(is_valid(up))
        {
            dijkstra_coord new_coord = new dijkstra_coord();
            new_coord.absolute_coord = up;
            new_coord.relative_coord = new Vector2Int(0, 1);
            new_coord.score = map[up.x, up.y];

            return_list.Add(new_coord);
        }
        if(is_valid(down))
        {
            dijkstra_coord new_coord = new dijkstra_coord();
            new_coord.absolute_coord = down;
            new_coord.relative_coord = new Vector2Int(0, -1);
            new_coord.score = map[down.x, down.y];

            return_list.Add(new_coord);
        }
        if(is_valid(left))
        {
            dijkstra_coord new_coord = new dijkstra_coord();
            new_coord.absolute_coord = left;
            new_coord.relative_coord = new Vector2Int(-1, 0);
            new_coord.score = map[left.x, left.y];

            return_list.Add(new_coord);
        }
        if(is_valid(right))
        {
            dijkstra_coord new_coord = new dijkstra_coord();
            new_coord.absolute_coord = right;
            new_coord.relative_coord = new Vector2Int(1, 0);
            new_coord.score = map[right.x, right.y];

            return_list.Add(new_coord);
        }

        return return_list;
    }

    public bool is_valid(Vector2Int pos)
    {
        if(pos.x < 0 || pos.x >= dungeon_map.x_dim)
        {
            return false;
        }
        if(pos.y < 0 || pos.y >= dungeon_map.y_dim)
        {
            return false;
        }
        if (!dungeon_map.is_floor[pos.x, pos.y])
        {
            return false;
        }
        return true;
    }

    public Vector2Int get_lowest_adjacent(Vector2Int cur_pos, int move_dist, int[,] ref_table)
    {
        Vector2Int up, down, left, right;
        up = new Vector2Int(cur_pos.x, cur_pos.y + move_dist);
        down = new Vector2Int(cur_pos.x, cur_pos.y - move_dist);
        left = new Vector2Int(cur_pos.x - move_dist, cur_pos.y);
        right = new Vector2Int(cur_pos.x + move_dist, cur_pos.y);

        Vector2Int return_pos = cur_pos;
        int lowest_value = 99;

        if(is_valid(up) && ref_table[up.x, up.y] < lowest_value)
        {
            return_pos = up;
            lowest_value = ref_table[up.x, up.y];
        }
        if (is_valid(down) && ref_table[down.x, down.y] < lowest_value)
        {
            return_pos = down;
            lowest_value = ref_table[down.x, down.y];
        }
        if (is_valid(left) && ref_table[left.x, left.y] < lowest_value)
        {
            return_pos = left;
            lowest_value = ref_table[left.x, left.y];
        }
        if (is_valid(right) && ref_table[right.x, right.y] < lowest_value)
        {
            return_pos = right;
            lowest_value = ref_table[right.x, right.y];
        }

        return return_pos;
    }
}

//Data structure to store needed info for Djikstra maps
public struct dijkstra_coord
{
    public Vector2Int relative_coord, absolute_coord;   //Relative coord stores just the up / down / etc value. Absolute is world position.
    public int score;

    public dijkstra_coord(Vector2Int n_r_coord, Vector2Int n_a_coord, int n_score)
    {
        relative_coord = n_r_coord;
        absolute_coord = n_a_coord;
        score = n_score;
    }
}
