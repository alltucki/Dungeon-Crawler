using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "temp_effect", fileName = "temp_effect.asset")]
public class temp_effect : ScriptableObject {

    public string effect_name;
    public int turns_remaining;

    public GameObject sfx;
    public Modifier per_turn_effect, on_apply_effect;
    public int mod_value;
    public bool stop_apply;
    private bool initialized;

    private void init()
    {
        /*
        if (per_turn_effect != null)
        {
            per_turn_effect = Object.Instantiate(per_turn_effect);
        }
        if (on_apply_effect != null)
        {
            on_apply_effect = Object.Instantiate(on_apply_effect);
        }
        */
        initialized = true;
    }

    public void do_effect(rpg_character target)
    {
        if(!initialized)
        {
            init();
        }

        Debug.Log("Do effect called for " + effect_name + " on " + target.name);
        turns_remaining--;
        attack_data dummy_data = new attack_data();
        dummy_data.target = target;

        if (turns_remaining <= 0)
        {
            //per_turn_effect.remove_modifier(dummy_data, mod_value);
            target.stat_cond.Remove(this);
        }
        else if(per_turn_effect != null)
        {
            play_sfx(dummy_data);
            //per_turn_effect.set_modifier(dummy_data, mod_value);
            //per_turn_effect.has_applied = false;
        }
    }

    public void on_apply(rpg_character target)
    {
        if (!initialized)
        {
            init();
        }

        Debug.Log("Called on_apply");
        attack_data dummy_data = new attack_data();
        dummy_data.target = target;

        if(on_apply_effect != null)
        {
            play_sfx(dummy_data);
            //on_apply_effect.set_modifier(dummy_data, mod_value);
        }
    }

    public void play_sfx(attack_data data)
    {
        if (sfx != null)
        {
            GameObject new_effect = GameObject.Instantiate(sfx);
            new_effect.transform.position = data.target.transform.position;
        }
    }
}
