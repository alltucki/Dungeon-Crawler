using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum duration_enum
{
    permanent, for_action, turn_limited
}

public enum comparison
{
    greater_than_or_equal, less_than_or_equal, equal
}

public enum base_value
{
    set, relative, random
}

[CreateAssetMenu(menuName = "Perk", fileName = "Perk.asset")]
public class Perk : ScriptableObject
{
    public Sprite icon;                 //The perk's icon
    public AudioClip sfx;               //The sfx to play when triggered, if any
    public string description;          //A description of what this perk does
    public trigger_enum trigger;        //Trigger to check the condition
    public string stat_comp;            //The stat we'll be checking to see if the perk applies
    public base_value comp_type;        //Whether the value should be dependant on the relative value to another entity
    public string other_comp;           //The stat to reference on the other involved entity
    public comparison op;               //Operator; greater than, less than, equal
    public int base_comp;               //Base value that the condition is compared to
    public duration_enum duration;      //How long the perk will apply, if condition is true
    public int turn_duration;           //How many turns the perk will apply, if duration is turn_limited
    private int turns_remaining;        //The current number of turns left
    public string stat;                 //The stat that will be modified
    public Modifier mod;                //The operation applied to the stat

    public void add_perk()
    {
        switch (trigger)
        {
            case trigger_enum.always:
                activate_perk();
                break;
            case trigger_enum.turn_start:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_start_turn", activate_perk);
                break;
            case trigger_enum.on_attack:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_start_attack", activate_perk);
                break;
            case trigger_enum.on_attacked:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_start_attacked", activate_perk);
                break;
            default:
                break;
        }
    }

    public void activate_effect()
    {
        GameObject trigger_icon = GameObject.Instantiate(util_ref.sprite_prefab);
        trigger_icon.AddComponent<fade_effect>();
        trigger_icon.GetComponent<fade_effect>().speed = 1f;
        //trigger_icon.AddComponent<movement_effect>();
        //trigger_icon.GetComponent<movement_effect>().movement_speed = new Vector3(0f, 1f, 0f);
        trigger_icon.AddComponent<grow_effect>();
        trigger_icon.GetComponent<grow_effect>().speed = 2f;
        trigger_icon.GetComponent<SpriteRenderer>().sprite = icon;
        trigger_icon.GetComponent<SpriteRenderer>().sortingLayerName = "Effects";
        if(sfx != null)
        {
            trigger_icon.AddComponent<AudioSource>();
            trigger_icon.GetComponent<AudioSource>().clip = sfx;
            trigger_icon.GetComponent<AudioSource>().playOnAwake = true;
            trigger_icon.GetComponent<AudioSource>().Play();
        }

        trigger_icon.transform.position = util_ref.p_manager.cur_player.transform.position + new Vector3(.5f, .5f, 0f);
        trigger_icon.transform.localScale = new Vector3(.5f, .5f, 1f);
    }


    public void activate_perk()
    {
        Debug.Log("Activated perk " + name);

        switch(trigger)
        {
            case trigger_enum.always:
                activate_effect();
                util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.add_modifier(mod, stat);
                break;
            case trigger_enum.turn_start:
                attack_data dummy_data = new attack_data();
                dummy_data.attacker = util_ref.p_manager.cur_player.GetComponent<rpg_character>();
                if(check_condition(dummy_data))
                {
                    activate_effect();
                    util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.add_modifier(mod, stat);
                }
                break;
            case trigger_enum.on_attack:
                if (util_ref.events.last_attack.attacker.stats.has_stat(stat))
                {
                    activate_effect();
                    util_ref.events.last_attack.attacker.stats.add_modifier(mod, stat);
                }
                else if (util_ref.events.last_attack.attack_ref.stats.has_stat(stat))
                {
                    activate_effect();
                    util_ref.events.last_attack.attack_ref.stats.add_modifier(mod, stat);
                }
                else
                {
                    Debug.LogError("Could not find stat " + stat + " in attacker or attack");
                }
                break;
            case trigger_enum.on_attacked:
                if (util_ref.events.last_attack.target.stats.has_stat(stat))
                {
                    activate_effect();
                    util_ref.events.last_attack.target.stats.add_modifier(mod, stat);
                }
                else if (util_ref.events.last_attack.attack_ref.stats.has_stat(stat))
                {
                    activate_effect();
                    util_ref.events.last_attack.attack_ref.stats.add_modifier(mod, stat);
                }
                else
                {
                    Debug.LogError("Could not find stat" + stat + " in target or attack");
                }
                break;
            default:
                Debug.LogError("Activate perk expected nonargument event");
                break;
        }

        switch (duration)
        {
            case duration_enum.for_action:
                action_remove();
                break;
            case duration_enum.turn_limited:
                turn_remove();
                break;
            default:
                break;
        }
    }

    //Function used when perk is activated via attacking or being attacked
    public void activate_perk(attack_data attack)
    {
        Debug.LogError("Activated perk expecting attack argument");
        Debug.Log("Activated perk " + name);
        bool condition = check_condition(attack);
        Debug.Log("Condition check is " + condition);

        if (condition)
        {
            activate_effect();
            switch (trigger)
            {
                case trigger_enum.on_attack:
                    if (attack.attacker.stats.has_stat(stat))
                    {
                        attack.attacker.stats.add_modifier(mod, stat);
                    }
                    else if(attack.attack_ref.stats.has_stat(stat))
                    {
                        attack.attack_ref.stats.add_modifier(mod, stat);
                    }
                    else
                    {
                        Debug.LogError("Could not find stat " + stat + " in attacker or attack");
                    }
                    break;
                case trigger_enum.on_attacked:
                    if (attack.target.stats.has_stat(stat))
                    {
                        attack.target.stats.add_modifier(mod, stat);
                    }
                    else if(attack.attack_ref.stats.has_stat(stat))
                    {
                        attack.attack_ref.stats.add_modifier(mod, stat);
                    }
                    else
                    {
                        Debug.LogError("Could not find stat" + stat + " in target or attack");
                    }
                    break;
                default:
                    Debug.LogError("Activate perk expected attack data");
                    break;
            }

            switch (duration)
            {
                case duration_enum.for_action:
                    action_remove();
                    break;
                case duration_enum.turn_limited:
                    turn_remove();
                    break;
                default:
                    break;
            }
        }
    }

    private void action_remove()
    {
        switch (trigger)
        {
            case trigger_enum.turn_start:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_end_turn", remove_perk);
                break;
            case trigger_enum.on_attack:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_end_attack", remove_perk);
                break;
            case trigger_enum.on_attacked:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_end_attacked", remove_perk);
                break;
            default:
                break;
        }
    }

    //Remove after the action that triggered the Perk has ended
    private void remove_action_listener()
    {
        switch (trigger)
        {
            case trigger_enum.turn_start:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_end_turn", remove_perk);
                break;
            case trigger_enum.on_attack:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_end_attack", remove_perk);
                break;
            case trigger_enum.on_attacked:
                util_ref.events.start_listening(util_ref.p_manager.cur_player.name + "_end_attacked", remove_perk);
                break;
            default:
                break;
        }
    }

    private void remove_turn_listener()
    {
        util_ref.events.stop_listening(util_ref.p_manager.cur_player.name + "_start_turn", inc_turn);
    }

    //Expire after a certain number of moves
    private void turn_remove()
    {
        turns_remaining = turn_duration + 1;
        util_ref.events.stop_listening(util_ref.p_manager.cur_player.name + "_start_turn", inc_turn);
    }

    public void inc_turn()
    {
        turns_remaining--;
        Debug.Log("Incremented turn counter. Turns remaining on " + name + ": " + turns_remaining.ToString());
        if(turns_remaining <= 0)
        {
            remove_perk();
            remove_turn_listener();
        }
    }

    public void remove_perk()
    {
        //Debug.Log("Removed bonus of perk " + name);

        switch (trigger)
        {
            case trigger_enum.always:
                util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.remove_modifier(mod, stat);
                break;
            case trigger_enum.turn_start:
                util_ref.p_manager.cur_player.GetComponent<rpg_character>().stats.remove_modifier(mod, stat);
                break;
            case trigger_enum.on_attack:
                if (util_ref.events.last_attack.attacker.stats.has_stat(stat))
                {
                    util_ref.events.last_attack.attacker.stats.remove_modifier(mod, stat);
                }
                else if (util_ref.events.last_attack.attack_ref.stats.has_stat(stat))
                {
                    util_ref.events.last_attack.attack_ref.stats.remove_modifier(mod, stat);
                }
                else
                {
                    Debug.LogError("Could not find stat " + stat + " in attacker or attack");
                }
                break;
            case trigger_enum.on_attacked:
                if (util_ref.events.last_attack.target.stats.has_stat(stat))
                {
                    util_ref.events.last_attack.target.stats.remove_modifier(mod, stat);
                }
                else if (util_ref.events.last_attack.attack_ref.stats.has_stat(stat))
                {
                    util_ref.events.last_attack.attack_ref.stats.remove_modifier(mod, stat);
                }
                else
                {
                    Debug.LogError("Could not find stat" + stat + " in target or attack");
                }
                break;
            default:
                Debug.LogError("Activate perk expected nonargument event");
                break;
        }
    }

    private bool check_condition(attack_data attack)
    {
        switch(comp_type)
        {
            case base_value.random:
                return random_comparison();
            case base_value.relative:
                return relative_comparison(attack);
            case base_value.set:
                return set_comparison(attack);
            default:
                return false;
        }
    }

    private bool random_comparison()
    {
        int random_value = Random.Range(0, 100);
        switch(op)
        {
            case comparison.equal:
                return random_value == base_comp;
            case comparison.greater_than_or_equal:
                return random_value >= base_comp;
            case comparison.less_than_or_equal:
                return random_value <= base_comp;
            default:
                break;
        }
        return false;
    }

    private bool relative_comparison(attack_data attack)
    {
        int self_stat = 0;
        int other_stat = 0;
        //If the trigger is the player attacking
        if (trigger == trigger_enum.on_attack) {
            //Check whether the referenced stat is on the player or the attack
            //and get the value
            if (attack.attacker.stats.has_stat(stat_comp))
            {
                self_stat = attack.attacker.stats.get_stat_value(stat_comp);
            }
            else if(attack.attack_ref.stats.has_stat(stat_comp))
            {
                self_stat = attack.attack_ref.stats.get_stat_value(stat_comp);
            }
            //Then get the comparison stat from the target
            other_stat = attack.target.stats.get_stat_value(other_comp);
        }
        //If the trigger is the player being attacked
        else if(trigger == trigger_enum.on_attacked)
        {
            //Get the comparison stat from the player
            self_stat = attack.target.stats.get_stat_value(stat_comp);
            //Check whether the referenced stat is on the enemy attacking
            //or the attack, and get the value
            if (attack.attacker.stats.has_stat(other_comp))
            {
                other_stat = attack.attacker.stats.get_stat_value(other_comp);
            }
            else if(attack.attack_ref.stats.has_stat(other_comp))
            {
                other_stat = attack.attack_ref.stats.get_stat_value(other_comp);
            }
        }

        switch (op)
        {
            case comparison.equal:
                return self_stat - other_stat == base_comp;
            case comparison.greater_than_or_equal:
                return self_stat - other_stat >= base_comp;
            case comparison.less_than_or_equal:
                return self_stat - other_stat <= base_comp;
            default:
                break;
        }

        return false;
    }

    private bool set_comparison(attack_data attack)
    {
        int comp_val = 0;
        //If the trigger is the player making an attack
        if(trigger == trigger_enum.on_attack)
        {
            //Pull the data from attacker
            if (attack.attacker.stats.has_stat(stat_comp))
            {
                comp_val = attack.attacker.stats.get_stat_value(stat_comp);
            }
            else if (attack.attack_ref.stats.has_stat(stat_comp))
            {
                comp_val = attack.attack_ref.stats.get_stat_value(stat_comp);
            }
        }
        //if the trigger is the player being attacked
        else if(trigger == trigger_enum.on_attacked)
        {
            //Pull the data from the target
            if (attack.target.stats.has_stat(stat_comp))
            {
                comp_val = attack.target.stats.get_stat_value(stat_comp);
            }
            else if (attack.attack_ref.stats.has_stat(stat_comp))
            {
                comp_val = attack.attack_ref.stats.get_stat_value(stat_comp);
            }
        }
        else if(trigger == trigger_enum.turn_start)
        {
            comp_val = attack.attack_ref.stats.get_stat_value(stat_comp);
        }

        switch (op)
        {
            case comparison.equal:
                return comp_val == base_comp;
            case comparison.greater_than_or_equal:
                return comp_val >= base_comp;
            case comparison.less_than_or_equal:
                return comp_val <= base_comp;
            default:
                break;
        }

        return false;
    }
}
