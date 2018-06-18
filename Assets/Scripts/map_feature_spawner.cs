using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class map_feature_spawner : MonoBehaviour {

    private bool[,] valid_squares;
    private bool init;
    public GameObject entrance, exit;
    public GameObject[] decorations;
    public List<GameObject> features;
    public List<Node> rooms;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        rooms = new List<Node>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void initialize()
    {
        valid_squares = new bool[util_ref.m_gen.x_dim, util_ref.m_gen.y_dim];
    }

    public void spawn_exit()
    {
        GameObject new_exit = GameObject.Instantiate(exit);
        new_exit.GetComponent<SpriteRenderer>().sprite = util_ref.m_gen.palette_tiles.GetSprite(new Vector3Int(5, -1, 0));

        spawn_feature(new_exit);
        new_exit.transform.position += new Vector3(.5f, .5f, 0f);
        features.Add(new_exit);
    }

    public void spawn_feature(GameObject feature)
    {
        //Build an array of possible rooms
        int valid_count = 0;
        for(int i = 0; i < rooms.Count; i++)
        {
            if(rooms[i].valid > 0)
            {
                valid_count++;
            }
        }
        if(valid_count <= 0)
        {
            Debug.LogError("No rooms with space remaining");
            return;
        }
        Node[] valid_rooms = new Node[valid_count];
        int valid_index = 0;
        for(int i = 0; i < rooms.Count; i++)
        {
            if(rooms[i].valid > 0)
            {
                valid_rooms[valid_index++] = rooms[i];
            }
        }
        //Randomly choose one of the rooms with valid spots remaining
        Node target_room = valid_rooms[Random.Range(0, valid_rooms.Length)];

        //Choose a spot in the room
        Vector2Int target_square = target_room.get_valid();

        //Spawn the feature and note that the square is now not valid
        feature.transform.position = new Vector3(target_square.x, target_square.y, 0);
        target_room.valid--;
        valid_squares[target_square.x, target_square.y] = false;
    }

    public Vector2Int get_rand_valid()
    {
        int x = Random.Range(0, util_ref.m_gen.x_dim);
        int y = Random.Range(0, util_ref.m_gen.y_dim);

        while (!valid_squares[x, y])
        {
            x = Random.Range(0, util_ref.m_gen.x_dim);
            y = Random.Range(0, util_ref.m_gen.y_dim);
        }

        return new Vector2Int(x, y);
    }

    public void set_valid()
    {
        decorations = GameObject.Find("Palette").GetComponent<palette>().decorations;

        //Get the base valid squares - all floor tiles that aren't doorways
        for (int x = 0; x < util_ref.m_gen.x_dim; x++)
        {
            for (int y = 0; y < util_ref.m_gen.y_dim; y++)
            {
                valid_squares[x, y] = false;
                if (util_ref.m_gen.is_floor[x,y])
                {
                    if(!is_doorway(x,y))
                    {
                        valid_squares[x, y] = true;
                        //util_ref.paint(x, y, Color.green);
                    }
                }
            }
        }
        Debug.Log("Completed setting valid squares");
        
        //Seperate the squares into rooms - discrete areas of valid tiles
        int index = 0;
        for (int x = 0; x < util_ref.m_gen.x_dim; x++)
        {
            for (int y = 0; y < util_ref.m_gen.y_dim; y++)
            {
                if(valid_squares[x,y])
                {

                    Node new_node = new Node();
                    rooms.Add(new_node);
                    floodfill(x, y, index);
                    index++;
                }
            }
        }
        Debug.Log("Made " + rooms.Count + " rooms");

        //Go back through and set validity again
        for (int x = 0; x < util_ref.m_gen.x_dim; x++)
        {
            for (int y = 0; y < util_ref.m_gen.y_dim; y++)
            {
                valid_squares[x, y] = false;
                if (util_ref.m_gen.is_floor[x, y])
                {
                    if (!is_doorway(x, y))
                    {
                        valid_squares[x, y] = true;
                        //util_ref.paint(x, y, Color.green);
                    }
                }
                //valid_squares[x, y] = check_neighbor(x, y);
            }
        }
    }

    public void unload_features()
    {
        int count = features.Count;
        for(int i = 0; i < count; i++)
        {
            Destroy(features[i]);
        }

        features.RemoveRange(0, count);
        rooms.RemoveRange(0, rooms.Count);
    }

    public void paint_valid()
    {
        int c_ind = 0;
        for(int i = 0; i < rooms.Count; i++)
        {
            Debug.Log(util_ref.colors[c_ind].ToString() + " room has interest of " + rooms[i].calc_interest().ToString());
            rooms[i].paint_node(util_ref.colors[c_ind]);
            c_ind++;
            if(c_ind >= 7)
            {
                c_ind = 0;
            }
        }
    }

    public void spice_it_up()
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            //For each room, add enemies, items, or features
            //For now, try with features
            //Make sure the room is more than one square,
            //and has valid squares left
            int num_dec = Random.Range(1, 3);
            for (int k = 0; k < num_dec; k++)
            {

                if (rooms[i].count > 1 && rooms[i].valid > 1)
                {
                    Vector2Int target_coord = rooms[i].get_valid();

                    //Instantiate random decoration at point
                    GameObject new_dec = GameObject.Instantiate(decorations[Random.Range(0, decorations.Length)]);
                    new_dec.transform.position = new Vector3(target_coord.x, target_coord.y);
                    features.Add(new_dec);
                    valid_squares[target_coord.x, target_coord.y] = false;
                    rooms[i].valid--;
                }
            }
        }
    }

    private bool is_doorway(int x, int y)
    {
        if(!util_ref.m_gen.is_floor[x,y - 1] && !util_ref.m_gen.is_floor[x, y + 1])
        {
            return true;
        }
        if(!util_ref.m_gen.is_floor[x - 1, y] && !util_ref.m_gen.is_floor[x + 1, y])
        {
            return true;
        }
        return false;
    }

    public void floodfill(int x, int y, int index)
    {
        rooms[index].add_coord(new Vector2Int(x, y));
        valid_squares[x, y] = false;
        //Debug.Log("Added square " + rooms[index].count.ToString() + " to current room");

        //Up
        if(ok_flood(x, y + 1))
        {
            //Debug.Log("Up was okay to add");
            floodfill(x, y + 1, index);
        }
        //Down
        if(ok_flood(x, y - 1))
        {
            //Debug.Log("Down was okay to add");
            floodfill(x, y - 1, index);
        }
        //Left
        if(ok_flood(x - 1, y))
        {
            //Debug.Log("Left was okay to add");
            floodfill(x - 1, y, index);
        }
        //Right
        if(ok_flood(x + 1, y))
        {
            //Debug.Log("Right was okay to add");
            floodfill(x + 1, y, index);
        }
    }

    public bool ok_flood(int x, int y)
    {
        if(!util_ref.in_bounds(x, y))
        {
            return false;
        }
        if(!util_ref.m_gen.is_floor[x,y])
        {
            return false;
        }
        return valid_squares[x, y];
    }

    private bool check_neighbor(int x, int y)
    {

        return (util_ref.m_gen.check_square(x, y) &&
            util_ref.m_gen.check_square(x - 1, y) &&
            util_ref.m_gen.check_square(x + 1, y) &&
            util_ref.m_gen.check_square(x, y - 1) &&
            util_ref.m_gen.check_square(x, y + 1));
    }

    public bool is_valid(int x, int y)
    {
        return valid_squares[x, y];
    }

    public bool is_valid(Vector2Int coord)
    {
        return valid_squares[coord.x, coord.y];
    }
}

public class Node
{
    public List<Vector2Int> coords;
    public int count, valid;
    public float interest;

    public Node()
    {
        coords = new List<Vector2Int>();
        count = 0;
    }

    public void add_coord(Vector2Int new_coord)
    {
        coords.Add(new_coord);
        count++;
        if(util_ref.feature_spawner.is_valid(new_coord))
        {
            valid++;
        }
        //valid++;
    }

    public void paint_node(Color color)
    {
        for(int i = 0; i < coords.Count; i++)
        {
            util_ref.paint(coords[i].x, coords[i].y, color);
        }
    }

    public float calc_interest()
    {
        if(valid == 0)
        {
            interest = count;
            return 0;
        }
        interest = (float)count / (float)valid;
        return interest;
    }

    public Vector2Int get_valid()
    {
        if(valid <= 0)
        {
            Debug.LogError("No valid squares left in node");
            return new Vector2Int(-1, -1);
        }

        //Build an array of possible coordinates
        int valid_count = 0;
        for(int i = 0; i < coords.Count; i++)
        {
            if(util_ref.feature_spawner.is_valid(coords[i]))
            {
                valid_count++;
            }
        }
        if(valid_count != valid)
        {
            Debug.LogWarning("Mismatch between manual valid count (" + valid_count.ToString() + ") and auto sum (" + valid.ToString() + ")");

            for(int i = 0; i < coords.Count; i++)
            {
                util_ref.paint(coords[i].x, coords[i].y, Color.red);
            }
        }
        if(valid_count == 0)
        {
            Debug.LogError("Did not find any valid squares");
        }

        Vector2Int[] possible_coords = new Vector2Int[valid_count];
        int valid_index = 0;
        for(int i = 0; i < coords.Count; i++)
        {
            if(util_ref.feature_spawner.is_valid(coords[i]))
            {
                possible_coords[valid_index++] = coords[i];
            }
        }

        int random_index = Random.Range(0, possible_coords.Length);
        Vector2Int return_coords = possible_coords[random_index];

        return return_coords;
    }
}
