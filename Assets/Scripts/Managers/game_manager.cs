using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

//A big meaty class that handles game initilization,
//level loading and unloading triggers,
//and turn changing
public class game_manager : MonoBehaviour {

    [HideInInspector] public bool players_turn = true;
    public GameObject player;
    public List<GameObject> enemies;
    public GameObject current_actor;
    public bool locked, enemies_cleared, paused;

    private int running_budget;

    public float time_on_current_turn;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        enemies = new List<GameObject>(0);
        get_enemies();
    }

    public void init_game()
    {
        Debug.Log("Called init game");
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Catacombs", LoadSceneMode.Single);
        Debug.Log("Ended init game");
    }

    //Clean up the current floor
    public void unload_level()
    {
        util_ref.hud.add_popup("You descend another floor...");
        util_ref.events.trigger_event("descend");
        //Unload current things
        util_ref.i_manager.unload_item();
        Debug.Log("Unloaded items");
        util_ref.e_manager.unload_enemies();
        Debug.Log("Unloaded enemies");
        util_ref.feature_spawner.unload_features();
        Debug.Log("Unloaded features");
        Debug.Log("Current floor unloaded");

        //Make a new map
        util_ref.m_gen.generate_cavern();
        Debug.Log("Generated new map");
        util_ref.feature_spawner.set_valid();
        Debug.Log("Set valid");

        util_ref.feature_spawner.spawn_feature(util_ref.p_manager.cur_player);
        Debug.Log("Moved player");

        //Player will still be considered to be moving after touching the exit
        //so force stop that so fov will work
        util_ref.p_manager.cur_player.GetComponent<rpg_character>().target_position = util_ref.p_manager.cur_player.transform.position;

        util_ref.feature_spawner.spawn_exit();
        //util_ref.i_manager.gen_items();
        util_ref.e_manager.init_enemies();
        util_ref.feature_spawner.spice_it_up();

        util_ref.v_manager.reset_fog();
        util_ref.v_manager.refresh_fog();
    }

    //This callback is used when we transition from the main menu to the dungeon
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded");
        util_ref.m_gen.initialize();
        util_ref.m_gen.generate_cavern();

        util_ref.feature_spawner.initialize();
        util_ref.feature_spawner.set_valid();
        util_ref.feature_spawner.spawn_exit();

        player = util_ref.p_manager.init_player();
        util_ref.e_manager.max_budget = 10;
        util_ref.e_manager.init_enemies();

        //I'm keeping this name and nobody can change my mind
        //(this adds all the actual features to the level)
        util_ref.feature_spawner.spice_it_up();
        //util_ref.feature_spawner.paint_valid();

        util_ref.d_manager.generate_player_map();

        util_ref.v_manager.get_fow();
        util_ref.v_manager.add_fog();
        util_ref.v_manager.refresh_fog();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
    }

    public void pause()
    {
        if (paused)
        {
            SceneManager.UnloadSceneAsync("Inventory");
            util_ref.p_manager.cur_player.GetComponent<player_script>().inv.update_hotbar();
            paused = false;
        }
        else if (!paused)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.LoadScene("Inventory", LoadSceneMode.Additive);
            paused = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        time_on_current_turn += Time.deltaTime;
        
        if(paused)
        {
            return;
        }

        //If it's not the player's turn, we need to handle enemy 
		if(!players_turn && !player.GetComponent<rpg_character>().is_locked())
        {
            //If we've killed all the enemies
            if (enemies_cleared || current_actor == null)
            {
                next_turn();
            }
            //If the current enemy is done with their turn
            else if(current_actor.GetComponent<rpg_character>().get_remaining_actions() <= 0 && !current_actor.GetComponent<rpg_character>().is_locked())
            {
                //Debug.Log("Calling next turn");
                next_turn();
            }
            //Set a timeout, if it's taken longer than a second and a half to decide, kill the enemy
            else if(time_on_current_turn > 1.5f)
            {
                Debug.Log("Killed " + current_actor.name + " due to timeout");
                current_actor.GetComponent<rpg_character>().set_unlocked("Force unlocked enemy due to timeout");
                current_actor.GetComponent<enemy_script>().die();
                next_turn();
            }
        }
	}

    public void player_end_turn()
    {
        //Debug.Log("Ended player's turn");
        float start_time = Time.realtimeSinceStartup;
        players_turn = false;
        util_ref.d_manager.generate_player_map();
        util_ref.d_manager.generate_player_sight_map();
        util_ref.b_manager.inc_turn();
        get_enemies();
        //Add the first enemy twice, since it will be removed on next_turn
        if (enemies.Count > 0)
        {
            enemies.Add(enemies[0]);
            current_actor = enemies[0];
        }

        float end_time = Time.realtimeSinceStartup;

        //Debug.Log("Completed post player turn processing in " + (end_time - start_time).ToString());
    }

    public void get_enemies()
    {
        //Debug.Log("Getting enemies");
        enemies.Clear();
        GameObject[] enemy_array = GameObject.FindGameObjectsWithTag("Enemy");
        //Debug.Log("Created array");

        if(enemy_array.Length == 0)
        {
            //Debug.Log("No enemies found");
            enemies_cleared = true;
            return;
        }
        else
        {
            enemies_cleared = false;
        }

        for(int i = 0; i < enemy_array.Length; i++)
        {
            enemies.Add(enemy_array[i]);
        }
        //Debug.Log("Added enemies");
    }

    public void next_turn()
    {
        time_on_current_turn = 0f;
        util_ref.d_manager.generate_enemy_location_map();
        //Debug.Log("Called next turn");
        if (!enemies_cleared)
        {
            enemies.RemoveAt(0);
        }
        if (enemies_cleared || enemies.Count <= 0)
        {
            //Debug.Log("Player started turn");
            players_turn = true;
            util_ref.e_manager.hide_enemies();
            current_actor = player;
            player.GetComponent<rpg_character>().start_turn();
        }
        else
        {
            if(enemies[0] == null)
            {
                return;
            }
            //Debug.Log(enemies[0].name + " started turn");
            enemies[0].GetComponent<rpg_character>().start_turn();
            //enemies[0].GetComponent<enemy_script>().AI_routine();
            current_actor = enemies[0];
        }
    }

    public void game_over()
    {
        enabled = false;
    }
}
