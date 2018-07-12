using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum effect_length
{
    temporary, permanent
}

[CreateAssetMenu(menuName ="Temporary Condition")]
public class temporary_condition : ScriptableObject {

    public rpg_character target_character, source_character;
    public attack_entity source_attack;
    public int turns_remaining, max_turns;

    public string on_start_s, on_turn_s, on_remove_s;
    public Modifier on_start_m, on_turn_m, on_remove_m;
    public effect_length on_start_l, on_turn_l;
    public GameObject on_start_e, on_turn_e, on_remove_e;

    public void add_effect(rpg_character target)
    {
        target_character = target;
        turns_remaining = max_turns;
        Debug.Log("Started listening to " + target.name + "_start_turn");
        util_ref.events.start_listening(target.name + "_start_turn", on_turn);
        on_start();
    }

    public void on_start()
    {
        //Simple, possibly need to refactor. Just need to check whether we should even
        //be bothering with the on_start method
        if (on_start_s != "")
        {
            /*
             * If we are aiming to reduce health, need to actually call the 
             * take_damage method to interact with perks, trigger the die()
             * function
             */
            if (on_start_s == "Health" && on_start_m.op == operation.sub)
            {
                attack_data new_data = new attack_data();
                new_data.target = target_character;
                new_data.attacker = source_character;
                new_data.attack_ref = source_attack;
                target_character.take_damage(new_data, Mathf.Abs(on_start_m.argument));
            }
            else
            {
                Debug.Log("Called start of " + name);
                if(on_start_l == effect_length.permanent)
                {
                    int cur_value = target_character.stats.get_stat_value(on_start_s);
                    target_character.stats.set_stat(on_start_s, on_start_m.mod_value(cur_value));
                }
                else if(on_start_l == effect_length.temporary)
                {
                    target_character.stats.add_modifier(on_start_m, on_start_s);
                }
            }
            turns_remaining = max_turns;
            if (on_start_e != null)
            {
                GameObject new_effect = GameObject.Instantiate(on_start_e);
                new_effect.transform.position = target_character.transform.position;
            }
        }
    }

    public void on_turn()
    {
        turns_remaining--;
        if(turns_remaining <= 0)
        {
            on_remove();
        }
        else
        {
            if (on_turn_s != "")
            {
                if (on_turn_s == "Health" && on_turn_m.op == operation.sub)
                {
                    attack_data new_data = new attack_data();
                    new_data.target = target_character;
                    new_data.attacker = source_character;
                    new_data.attack_ref = source_attack;
                    target_character.take_damage(new_data, Mathf.Abs(on_turn_m.argument));
                }
                else
                {
                    Debug.Log("Called on turn of " + name);
                    if (on_turn_l == effect_length.permanent)
                    {
                        int cur_value = target_character.stats.get_stat_value(on_turn_s);
                        target_character.stats.set_stat(on_turn_s, on_turn_m.mod_value(cur_value));
                    }
                    else if (on_turn_l == effect_length.temporary)
                    {
                        target_character.stats.add_modifier(on_turn_m, on_turn_s);
                    }
                }
                if (on_turn_e != null)
                {
                    GameObject new_effect = GameObject.Instantiate(on_turn_e);
                    new_effect.transform.position = target_character.transform.position;
                }
            }
        }
    }

    public void on_remove()
    {
        //We'll need to change this if we ever want to temporarially
        //remove health
        if(on_start_s != "" && on_start_l == effect_length.temporary)
        {
            target_character.stats.remove_modifier(on_start_m, on_start_s);
        }
        if(on_turn_s != "" && on_turn_l == effect_length.temporary)
        {
            target_character.stats.remove_modifier(on_turn_m, on_turn_s);
        }
        if (on_remove_s != "")
        {
            if (on_remove_s == "Health" && on_remove_m.op == operation.sub)
            {
                attack_data new_data = new attack_data();
                new_data.target = target_character;
                new_data.attacker = source_character;
                new_data.attack_ref = source_attack;
                target_character.take_damage(new_data, Mathf.Abs(on_remove_m.argument));
            }
            else
            {
                target_character.stats.add_modifier(on_remove_m, on_remove_s);
            }
            if(on_remove_e != null) {
                GameObject new_effect = GameObject.Instantiate(on_remove_e);
                new_effect.transform.position = target_character.transform.position;
            }
        }
        Destroy(this);
    }
}
