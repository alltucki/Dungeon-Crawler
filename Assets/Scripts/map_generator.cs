using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class map_generator : MonoBehaviour {

    public Tilemap floor_map, wall_map, palette_tiles;
    public TileBase[] wall_tiles, floor_tiles;
    public bool[,] is_floor;
    public bool generate_flag, iterate_flag, invert_flag, demolish_flag;
    private bool init;
    public float raycast_time;
    public int x_dim, y_dim, live_chance;
    public LayerMask blocking_layer;
    public GameObject player, palette;
    public map_feature_spawner features;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		if(generate_flag)
        {
            generate_flag = false;
            //generate_cave();
            blank_map();
            game_of_life();
            draw_border();
            rewrite_map();
        }
        if(iterate_flag)
        {
            iterate_flag = false;
            is_floor = iterate_life(is_floor);
            rewrite_map();
        }
        if(invert_flag)
        {
            invert_flag = false;
            invert_map();
            rewrite_map();
        }
        if(demolish_flag)
        {
            demolish_flag = false;
            demolish_walls();
            rewrite_map();
        }

        //test_raycast();
	}

    public void initialize()
    {
        floor_map = GameObject.Find("Floor").GetComponent<Tilemap>();
        wall_map = GameObject.Find("Blocking Layer").GetComponent<Tilemap>();
        floor_map.size = new Vector3Int(x_dim, y_dim, 0);
        wall_map.size = new Vector3Int(x_dim, y_dim, 0);

        features = gameObject.GetComponent<map_feature_spawner>();

        palette_tiles = GameObject.Find("Palette").transform.GetChild(0).GetComponent<Tilemap>();

        wall_tiles = new TileBase[21];
        floor_tiles = new TileBase[6];

        set_palette();

        is_floor = new bool[x_dim, y_dim];
    }

    public void set_palette()
    {
        for (int x = 0; x < 21; x++)
        {
            wall_tiles[x] = palette_tiles.GetTile(new Vector3Int(x, 0, 0));
        }
        for (int x = 0; x < 6; x++)
        {
            floor_tiles[x] = palette_tiles.GetTile(new Vector3Int(x, -1, 0));
        }
    }

    private void test_raycast()
    {
        float start_time = Time.realtimeSinceStartup;

        for(int x = 0; x < x_dim; x++)
        {
            for(int y = 0; y < y_dim; y++)
            {
                Vector2 top_left, middle, top_right, player_pos;
                top_left = new Vector2(x, y);
                middle = new Vector2(x + .5f, y - .5f);
                top_right = new Vector2(x + 1, y);
                player_pos = new Vector2(player.transform.position.x + .5f, player.transform.position.y - .5f);

                RaycastHit2D hit_tl = Physics2D.Linecast(top_left, player_pos, blocking_layer);
                RaycastHit2D hit_middle = Physics2D.Linecast(middle, player_pos, blocking_layer);
                RaycastHit2D hit_tr = Physics2D.Linecast(top_right, player_pos, blocking_layer);

                bool clear_los = (hit_tl.collider == null || hit_tr.collider.gameObject.tag == "Player")
                    || (hit_middle.collider == null || hit_middle.collider.gameObject.tag == "Player")
                    || (hit_tr.collider == null || hit_tr.collider.gameObject.tag == "Player");

                if(clear_los)
                {
                    floor_map.SetTileFlags(new Vector3Int(x,y,0), TileFlags.None);
                    floor_map.SetColor(new Vector3Int(x,y,0), Color.white);
                    wall_map.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
                    wall_map.SetColor(new Vector3Int(x, y, 0), Color.white);
                }
                else
                {
                    floor_map.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
                    floor_map.SetColor(new Vector3Int(x, y, 0), Color.black);
                    wall_map.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
                    wall_map.SetColor(new Vector3Int(x, y, 0), Color.black);
                }
            }
        }

        float end_time = Time.realtimeSinceStartup;

        raycast_time = end_time - start_time;
    }

    public void generate_cavern()
    {
        blank_map();
        drunkards_walk();
        draw_border();
        rewrite_map();
    }

    private void drunkards_walk()
    {
        int x = Random.Range(0, x_dim);
        int y = Random.Range(0, y_dim);
        is_floor[x, y] = true;
        int total_floor = 1;

        while(total_floor < (x_dim * y_dim * ((float)live_chance / 100f)))
        {
            int direction = Random.Range(0, 4);
            switch(direction)
            {
                case 0:
                    //Up
                    if(y < y_dim-1)
                        y++;
                    break;
                case 1:
                    if(y > 0)
                        y--;
                    //Down
                    break;
                case 2:
                    if(x > 0)
                        x--;
                    //Left
                    break;
                case 3:
                    if(x < x_dim-1)
                        x++;
                    //Right
                    break;
                default:
                    break;
            }
            if (!is_floor[x, y])
            {
                is_floor[x, y] = true;
                total_floor++;
            }
        }
    }

    private void blank_map()
    {
        for(int x = 0; x < x_dim; x++)
        {
            for(int y = 0; y < y_dim; y++)
            {
                is_floor[x, y] = false;
            }
        }
    }

    private void generate_cave()
    {
        game_of_life();
        invert_map();
        is_floor = iterate_life(is_floor);
        is_floor = iterate_life(is_floor);

        draw_border();
        rewrite_map();
    }

    private void draw_border()
    {
        for(int x = 0; x < x_dim; x++)
        {
            is_floor[x, 0] = false;
            is_floor[x, y_dim-1] = false;
        }
        for(int y = 0; y < y_dim; y++)
        {
            is_floor[0, y] = false;
            is_floor[x_dim-1, y] = false;
        }
    }

    private void invert_map()
    {
        for(int x = 0; x < x_dim; x++)
        {
            for(int y = 0; y < y_dim; y++)
            {
                is_floor[x, y] = !is_floor[x, y];
            }
        }
    }

    private void demolish_walls()
    {
        for (int x = 0; x < x_dim; x++)
        {
            for(int y = 0; y < y_dim; y++)
            {
                if(!is_floor[x,y] && (wall_check_neighbors(x,y) == 6 || wall_check_neighbors(x,y) == 9))
                {
                    is_floor[x, y] = true;
                }
            }
        }
    }

    private void rewrite_map()
    {
        wall_map.ClearAllTiles();
        floor_map.ClearAllTiles();
        for (int x = 0; x < x_dim; x++)
        {
            for (int y = 0; y < y_dim; y++)
            {
                /*
                if (gameobj_map[x, y] != null)
                {
                    Destroy(gameobj_map[x, y]);
                }
                */    
                if (!is_floor[x, y])
                {
                    //GameObject new_tile = GameObject.Instantiate(floor);
                    //new_tile.transform.position = new Vector3(x, y);
                    int neighbor_check = wall_check_neighbors(x, y);
                    wall_map.SetTile(new Vector3Int(x, y, 0), wall_tiles[neighbor_check]);
                    if(neighbor_check == 15)
                    {
                        wall_map.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
                        //wall_map.SetColor(new Vector3Int(x, y, 0), Color.black);
                    }
                }
                else
                {
                    //GameObject new_tile = GameObject.Instantiate(wall);
                    //new_tile.transform.position = new Vector3(x, y);
                    floor_map.SetTile(new Vector3Int(x, y, 0), floor_tiles[Random.Range(0,4)]);
                }

            }
        }
    }

    private bool[,] iterate_life(bool[,] cur_map)
    {
        bool[,] return_map = new bool[x_dim, y_dim];
        for(int x = 0; x < x_dim; x++)
        {
            for(int y = 0; y < y_dim; y++)
            {
                return_map[x, y] = check_neighbors(cur_map, x, y);
            }
        }
        return return_map;
    }

    private bool check_neighbors(bool[,] cur_map, int x, int y)
    {
        int live_neighbors = 0;

        //Check above
        if(y == 0 || cur_map[x,y-1])
        {
            live_neighbors++;
        }

        //Check below
        if(y == y_dim-1 || cur_map[x,y+1])
        {
            live_neighbors++;
        }

        //Check left
        if(x == 0 || cur_map[x-1,y])
        {
            live_neighbors++;
        }

        //Check right
        if(x == x_dim-1 || cur_map[x+1,y])
        {
            live_neighbors++;
        }

        if(live_neighbors < 2)
        {
            return false;
        }
        else if(cur_map[x,y] && live_neighbors == 2 || live_neighbors == 3)
        {
            return true;
        }
        else if(live_neighbors > 3)
        {
            return false;
        }
        else if(live_neighbors == 3)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void game_of_life()
    {
        for (int x = 0; x < x_dim; x++)
        {
            for (int y = 0; y < y_dim; y++)
            {
                int new_flag = Random.Range(0, 100);
                is_floor[x, y] = false;
                if (new_flag < live_chance)
                {
                    is_floor[x, y] = true;
                }
            }
        }
    }

    public int wall_check_neighbors(int x, int y)
    {
        int up = 0, left = 0, right = 0, down = 0;
        if(!check_square(x,y+1))
        {
            up = 1 * 1;
        }
        if(!check_square(x-1,y))
        {
            left = 2 * 1;
        }
        if(!check_square(x+1,y))
        {
            right = 4 * 1;
        }
        if(!check_square(x,y-1))
        {
            down = 8 * 1;
        }

        return up + left + right + down;
    }

    public bool check_square(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }
        if (x >= x_dim || y >= y_dim)
        {
            return false;
        }

        return is_floor[x, y];
    }
}
