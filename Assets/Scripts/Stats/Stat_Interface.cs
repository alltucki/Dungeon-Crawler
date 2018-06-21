using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A wrapper class for keeping track of stats for characters and attacks
 */ 

[System.Serializable]
public class Stat_Interface {

    [SerializeField]private List<string> keys;  //Since Unity can't serialize dictionarys, and we need to
    [SerializeField]private List<Stat> values;  //serialize for attacks, we're making a jury-rigged dictionary
    [SerializeField]private bool init;          //Serialize this so we don't init and overwrite the existing stats

    /*
     *  public int health, armor;
     *  public int move_actions, attack_actions, shared_actions;
     *  public int max_health, max_move, max_attack, max_shared;
     */
    public void init_character()
    {
        keys = new List<string>();
        values = new List<Stat>();
        //Debug.Log("Created stat interface for character");
        init_stat("Health");
        init_stat("Armor");
        init_stat("Move Actions");
        init_stat("Attack Actions");
        init_stat("Shared Actions");
        init_stat("Max Health");
        init_stat("Max Move");
        init_stat("Max Attack");
        init_stat("Max Shared");
        init = true;
    }
    
    /*
    public void init_attack()
    {
        keys = new List<string>();
        values = new List<Stat>();
        //Debug.Log("Created stat interface for attack");
        init_stat("Range");
        init_stat("Radius");
        init_stat("Damage");
        init_stat("Pierce");
        init_stat("Slide");
        init_stat("Push");
        init = true;
    }
    */

    public void initialize()
    {
        keys = new List<string>();
        values = new List<Stat>();
        init = true;
    }

    //Print the values, formatted key : value
    public void print_values()
    {
        for(int key = 0; key < keys.Count; key++)
        {
            Debug.Log(keys[key] + ": " + values[key].get_value());
        }
    }

    //Add a new modifier to a given stat
    public void add_modifier(Modifier new_mod, string index)
    {
        if(!keys.Contains(index))
        {
            Debug.LogError("Could not find key " + index);
            return;
        }
        //Debug.Log("Added " + new_mod.op.ToString() + new_mod.argument.ToString() + " modifier to " + index);
        //Debug.Log("Current value: " + get_stat_value(index) + " with " + get_stat(index).get_num_modifiers() + " modifiers");
        get_stat(index).add_modifier(new_mod);
        //Debug.Log("New value: " + get_stat_value(index) + " with " + get_stat(index).get_num_modifiers() + " modifiers");
    }

    //Remove a modifier from a stat
    public void remove_modifier(Modifier old_mod, string index)
    {
        //Debug.Log("Removed " + old_mod.op.ToString() + old_mod.argument.ToString() + " modifier from " + index);
        //Debug.Log("Current value: " + get_stat_value(index) + " with " + get_stat(index).get_num_modifiers() + " modifiers");
        get_stat(index).remove_modifier(old_mod);
        //Debug.Log("New value: " + get_stat_value(index) + " with " + get_stat(index).get_num_modifiers() + " modifiers");
    }

    //Returns the actual value of the requested stat, with modifiers taken into account
    public int get_stat_value(string index)
    {
        if(!keys.Contains(index))
        {
            return 0;
        }
        return get_stat(index).get_value();
    }

    public bool has_stat(string index)
    {
        return keys.Contains(index);
    }

    public Stat get_stat(string index)
    {
        if(!has_stat(index))
        {
            Debug.LogError("Searching for stat " + index + " but stat was not found");
            return null;
        }
        return values[keys.IndexOf(index)];
    }

    public void set_stat(string index, int value)
    {
        get_stat(index).set_value(value);
    }

    //Create a new stat with the given name
    public void init_stat(string stat_name)
    {
        if (!init)
        {
            initialize();
        }
        add_stat(new Stat(stat_name, 0));
    }

    //Add a new stat that has already been created
    public void add_stat(Stat new_stat)
    {
        if(!init)
        {
            initialize();
        }
        keys.Add(new_stat.stat_name);
        values.Add(new_stat);
    }

    //Get rid of a stat
    public void remove_stat(Stat removed_stat)
    {
        int index = values.IndexOf(removed_stat);
        keys.RemoveAt(index);
        values.RemoveAt(index);
    }

    //Set the max value of a stat
    public void set_stat_max(string index, int max_value)
    {
        get_stat(index).enable_max(max_value);
    }

    public void set_stat_to_max(string index)
    {
        get_stat(index).set_to_max();
    }
}
