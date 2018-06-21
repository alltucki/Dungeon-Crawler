using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    public string stat_name;
    [SerializeField]private int value, max_value;
    [SerializeField][HideInInspector]private bool has_max;
    [SerializeField]private List<Modifier> active_modifiers;

    public Stat(string name, int initial_value)
    {
        stat_name = name;
        value = initial_value;
        active_modifiers = new List<Modifier>();
    }

    public void add_modifier(Modifier new_mod)
    {
        //Debug.Log("Added modifier " + new_mod.ToString() + " to " + stat_name);
        //Debug.Log("Current value is " + get_value());
        active_modifiers.Add(new_mod);
        //Debug.Log("Modified value is " + get_value());
    }

    public void remove_modifier(Modifier old_mod)
    {
        active_modifiers.Remove(old_mod);
    }

    public int get_num_modifiers()
    {
        return active_modifiers.Count;
    }

    public int get_value()
    {
        
        int mod_value = value;
        if(active_modifiers.Count <= 0)
        {
            return mod_value;
        }
        for (int i = 0; i < active_modifiers.Count; i++)
        {
            mod_value = active_modifiers[i].mod_value(mod_value);
        }
        return mod_value;
    }

    public void inc_value()
    {
        value++;
        if(has_max && value > max_value)
        {
            value = max_value;
        }
    }

    public void dec_value()
    {
        value--;
    }

    public void set_value(int new_value)
    {
        /*
        if(stat_name == "Health")
        {
            Debug.Log("Set value of health to " + new_value);
        }
        */
        value = new_value;
        if (has_max && value > max_value)
        {
            value = max_value;
        }
    }

    public void set_to_max()
    {
        /*
        if (stat_name == "Health")
        {
            Debug.Log("Set value of health to " + max_value);
        }
        */
        value = max_value;
    }

    public void enable_max(int new_max)
    {
        if(has_max)
        {
            Debug.LogWarning("Set max value for " + stat_name + " which already has max value");
            if(value > new_max)
            {
                value = new_max;
            }
        }

        has_max = true;
        max_value = new_max;
    }
}
