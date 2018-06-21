using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

//Script that interfaces with rpg_character
//and adds behavior needed for character control
public class player_script : MonoBehaviour
{
    private rpg_character attached_character;       //The rpg_character script on the same gameobject
    public int cur_skill, queued_index;

    public player_inventory inv;

    public LayerMask blocking_layer;                //TODO: move this to util_ref
    private Camera main_camera;
    private bool has_attacked, has_paused, has_passed;  //Used to prevent chain activation of actions
    private bool initialized;
    private float time_since_move;

    // Use this for initialization
    void Start () {
        attached_character = GetComponent<rpg_character>();

        //manager = GameObject.Find("Game Manager").GetComponent<game_manager>();
        attached_character.stats.init_character();
        attached_character.stats.init_stat("Gold");
        attached_character.stats.set_stat_max("Health", 10);
        attached_character.stats.set_stat_to_max("Health");
        attached_character.stats.set_stat("Max Shared", 1);

        main_camera = Camera.main;

        //Equip starting items
        util_ref.p_manager.equip_starting_items();
        util_ref.p_manager.equip_starting_perks();

        //Set hotbar
        inv.init(this);
    }

    // Update is called once per frame
    void Update()
    {
        time_since_move += Time.deltaTime;

        check_pause();
        if (!util_ref.g_manager.paused)
        {
            check_hotbar_queue();
            check_movement();
            check_attack();
            check_pass();
        }

        if(util_ref.g_manager.players_turn && (attached_character.get_attack_remaining() <= 0 && attached_character.get_move_remaining() <= 0))
        {
            end_turn();
        }
    }

    public void end_turn()
    {
        attached_character.end_turn();
        util_ref.g_manager.player_end_turn();  
    }

    //Check if we need to open the inventory screen
    private void check_pause()
    {
        if (Input.GetAxis("Cancel") != 0f)
        {
            if (!has_paused)
            {
                util_ref.g_manager.pause();
                has_paused = true;
            }
        }
        else {
            has_paused = false;
        }
    }

    //Check movement
    private void check_movement()
    {
        bool move_input = false;

        if (!util_ref.g_manager.players_turn || attached_character.get_move_remaining() <= 0)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            move_input = true;
        }

        //If we've released all movement buttons,
        //reset the delay timer for movement
        if (!move_input)
        {
            time_since_move = 1f;
        }
        if (time_since_move > .25f && move_input)
        {
            time_since_move = 0f;
            attempt_move(horizontal, vertical);
        }
    }

    /*
     * Raycast to the square in front of the character
     * If we don't his anything, great! Move to the square
     * If we do hit something, try interacting with it
     */ 
    public void attempt_move(int x_dir, int y_dir)
    {
        RaycastHit2D hit;
        Vector2 start = new Vector2(transform.position.x + .5f, transform.position.y + .5f);
        Vector2 end = new Vector2(start.x + x_dir, start.y + y_dir);
        Raycast(start, end, out hit);

        if (hit.collider == null)
        {
            attached_character.move_abs((int)transform.position.x + x_dir, (int)transform.position.y + y_dir);
            attached_character.decrease_move();
            util_ref.v_manager.refresh_fog();
        }
        else
        {
            Collision(hit, x_dir, y_dir);
        }
    }

    //Raycast method, used so we can add in the drawline when we raycast
    private void Raycast(Vector2 start, Vector2 end, out RaycastHit2D hit)
    {
        attached_character.disable_hb();
        hit = Physics2D.Linecast(start, end, blocking_layer);
        Color ray_color = Color.red;
        if (hit.collider == null)
        {
            ray_color = Color.blue;
        }

        Debug.DrawLine(start, end, ray_color, 5f);
        attached_character.enable_hb();
    }

    //Queue up a skill if the user presses a hotbar button
    private void check_hotbar_queue()
    {
        int hotbar_queue = -1;

        for(int i = 0; i < inv.get_skill_count(); i++)
        {
            if(Input.GetAxis("Hotbar " + i.ToString()) != 0f)
            {
                hotbar_queue = i;
                cur_skill = i;
            }
        }

        if(hotbar_queue == -1)
        {
            return;
        }

        inv.queue_skill(cur_skill);
    }

    //This function does most of the heavy lifting for interactions with the world
    private void check_attack()
    {
        bool attack_input = false;

        //If it's not our turn, or we've used all our actions, get out of here!
        if (!util_ref.g_manager.players_turn || attached_character.get_attack_remaining() <= 0)
        {
            return;
        }

        //Raycast to the point at z index 0
        int attack = (int)Input.GetAxisRaw("Fire1");
        Vector3 target = main_camera.ScreenToWorldPoint(Input.mousePosition);
        target = new Vector3(target.x, target.y, 0f);

        //If we got attack input, keep moving
        if (attack != 0)
        {
            attack_input = true;
        }
        //See if there's an interactable object at the point
        if (!has_attacked && attack_input)
        {
            //Raycast to check the point
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(main_camera.ScreenToWorldPoint(Input.mousePosition).x,
                main_camera.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);

            //If we hit something...
            if(hit.transform != null)
            {
                //First check if we're even in range to care
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if(dist == 1)
                {
                    //Check if there's an interactable component on the target
                    Interactable interact = hit.transform.gameObject.GetComponent<Interactable>();
                    if (interact != null && attached_character.get_remaining_actions() > 0)
                    {
                        interact.force_interact(attached_character);
                        has_attacked = true;
                        if(attached_character.stats.get_stat_value("Move Actions") > 1)
                        {
                            attached_character.decrease_move();
                        }
                        else
                        {
                            attached_character.decrease_attack();
                        }
                    }
                }
            }
        }


        //If we've already attacked and don't have any input
        if (has_attacked && !attack_input)
        {
            //Mark that we can attack again
            has_attacked = false;
        }
        //If we haven't attacked and do have input
        if (!has_attacked && attack_input)
        {
            //Returns true if an attack was successful
            has_attacked = inv.do_attack(cur_skill, gameObject, target);
            if(has_attacked)
            {
                //manager.player_end_turn();
                attached_character.decrease_attack();
                //set_locked("Locked player to allow effect to play");
            }
        }
    }

    private void check_pass()
    {
        float pass = Input.GetAxis("Jump");
        if (has_passed)
        {
            if(pass == 0f)
            {
                has_passed = false;
            }
            else
            {
                return;
            }
        }
        if(util_ref.g_manager.players_turn && pass != 0f)
        {
            //Setting to an extreme negative value to accommodate possible bonuses to actions
            attached_character.stats.set_stat("Move Actions", -10);
            attached_character.stats.set_stat("Attack Actions", -10);
            attached_character.stats.set_stat("Shared Actions", -10);
            has_passed = true;
        }
    }

    public void Collision(RaycastHit2D hit_info, int x_dir, int y_dir)
    {
        Debug.Log("Collided with " + hit_info.transform.gameObject.name);
        GameObject hit_go = hit_info.transform.gameObject;
        if (hit_go.tag == "Enemy")
        {
            //attack(hit_info.transform.gameObject);
            //stats.get_stat("Attack Actions").dec_value();
        }
        else if(hit_go.GetComponent<Interactable>() != null)
        {
            hit_go.GetComponent<Interactable>().try_interact(attached_character);

            if(hit_go.GetComponent<walkable>() != null)
            {
                attached_character.move_abs((int)transform.position.x + x_dir, (int)transform.position.y + y_dir);
                attached_character.decrease_move();
            }
        }
    }

    public rpg_character get_character()
    {
        return attached_character;
    }
}