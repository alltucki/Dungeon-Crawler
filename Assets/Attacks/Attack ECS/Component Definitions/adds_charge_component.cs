using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class adds_charge_component : attack_component
{
    [SerializeField]string charge_name;
    [SerializeField]int max_charge;

    public adds_charge_component()
    {
        component_name = "Adds charge";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        //Need to set stat in character so other attacks can reference the charges
        if (triggering_attack.activated_effect)
        {
            if(!attacker.stats.has_stat(charge_name))
            {
                attacker.stats.init_stat(charge_name);
                attacker.stats.set_stat_max(charge_name, max_charge);
            }
            if (attacker.stats.get_stat_value(charge_name) == max_charge)
            {
                triggering_attack.activated_effect = false;
                return false;
            }
            attacker.stats.get_stat(charge_name).inc_value();
            //triggering_attack.stats.get_stat(charge_name).inc_value();
            Debug.Log(charge_name + " is now " + attacker.stats.get_stat_value(charge_name));
        }
        return triggering_attack.activated_effect;
    }

    //Wait to add the charge name until we have actually set the string
    //If we do it here, it'll just be null
    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        //string old_name = charge_name;
        charge_name = EditorGUILayout.TextField(charge_name);
        max_charge = EditorGUILayout.IntField(max_charge);
        /*
        if(attack_ref.stats.has_stat(old_name))
        {
            attack_ref.stats.remove_stat(attack_ref.stats.get_stat(old_name));
        }

        attack_ref.stats.init_stat(charge_name);
        attack_ref.stats.set_stat_max(charge_name, max_charge);
        */
    }
}
