using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class util_ref : MonoBehaviour {

    public static GameObject hotbar_prefab, sprite_prefab, pickup_prefab;
    public static game_manager g_manager;
    public static item_manager i_manager;
    public static enemy_manager e_manager;
    public static boss_manager b_manager;
    public static player_manager p_manager;
    public static map_feature_spawner feature_spawner;
    public static map_generator m_gen;
    public static dijkstra_manager d_manager;
    public static fov_manager v_manager;
    public static hud_script hud;
    public static event_manager events;
    public static perk_manager perks;
    public static palette cur_palette;

    public GameObject hotbar_ref, sprite_ref, pickup_ref;

    public static Color[] colors;

    public static Vector2Int up, down, left, right;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
        hotbar_prefab = hotbar_ref;
        sprite_prefab = sprite_ref;
        pickup_prefab = pickup_ref;

        g_manager = GameObject.Find("Game Manager").GetComponent<game_manager>();
        i_manager = GameObject.Find("Item Manager").GetComponent<item_manager>();
        e_manager = GameObject.Find("Enemy Manager").GetComponent<enemy_manager>();
        b_manager = GameObject.Find("Enemy Manager").GetComponent<boss_manager>();
        p_manager = GameObject.Find("Player Manager").GetComponent<player_manager>();
        perks = GameObject.Find("Player Manager").GetComponent<perk_manager>();
        feature_spawner = GameObject.Find("Map Manager").GetComponent<map_feature_spawner>();
        m_gen = GameObject.Find("Map Manager").GetComponent<map_generator>();
        v_manager = GameObject.Find("Map Manager").GetComponent<fov_manager>();
        d_manager = GameObject.Find("Dijkstra Manager").GetComponent<dijkstra_manager>();
        events = GameObject.Find("Event Manager").GetComponent<event_manager>();


        colors = new Color[7];
        colors[0] = Color.green;
        colors[1] = Color.red;
        colors[2] = Color.blue;
        colors[3] = Color.cyan;
        colors[4] = Color.magenta;
        colors[5] = Color.yellow;
        colors[6] = Color.grey;

        up = new Vector2Int(0, 1);
        down = new Vector2Int(0, -1);
        left = new Vector2Int(-1, 0);
        right = new Vector2Int(1, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void paint(int x, int y, Color color)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        m_gen.floor_map.SetTileFlags(pos, TileFlags.None);
        m_gen.floor_map.SetColor(pos, color);

        m_gen.wall_map.SetTileFlags(pos, TileFlags.None);
        m_gen.wall_map.SetColor(pos, color);
    }

    public static bool in_bounds(int x, int y)
    {
        if(x < 0 || x >= m_gen.x_dim)
        {
            return false;
        }
        if(y < 0 || x >= m_gen.y_dim)
        {
            return false;
        }
        return true;
    }

    public static bool okay_to_move(int x, int y)
    {
        Vector2Int comp_vec = new Vector2Int(x, y);
        if(!in_bounds(x,y))
        {
            return false;
        }
        if(e_manager.is_occupied(comp_vec))
        {
            return false;
        }
        if(p_manager.get_p_pos() == comp_vec)
        {
            return false;
        }
        if(!m_gen.is_floor[x,y])
        {
            return false;
        }
        return true;
    }

    public static bool okay_to_move(Vector2Int test_coord)
    {
        if (!in_bounds(test_coord.x, test_coord.y))
        {
            return false;
        }
        if (e_manager.is_occupied(test_coord))
        {
            return false;
        }
        if (p_manager.get_p_pos() == test_coord)
        {
            return false;
        }
        if (!m_gen.is_floor[test_coord.x, test_coord.y])
        {
            return false;
        }
        return true;
    }
}
