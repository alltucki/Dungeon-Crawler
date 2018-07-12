using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class consumes_charge_component : attack_component
{
    [SerializeField]string charge_name;

    public consumes_charge_component()
    {
        component_name = "Consumes charge";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        if(triggering_attack.target_squares.Count <= 0)
        {
            return false;
        }
        int cost = triggering_attack.stats.get_stat_value("cost");
        int charges = attacker.stats.get_stat_value(charge_name);
        //Debug.Log("Attempting to remove " + cost + " charges from " + charges + " " + charge_name);
        if (charges >= cost)
        {
            attacker.stats.set_stat(charge_name, charges - cost);
            //Debug.Log(charge_name + " is now " + attacker.stats.get_stat_value(charge_name));
            return true;
        }
        else
        {
            triggering_attack.target_squares.Clear();
            return false;
        }
    }

    public override void add(attack_entity target)
    {
        target.stats.init_stat("cost");
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        EditorGUILayout.BeginHorizontal();
        charge_name = EditorGUILayout.TextField(charge_name);
        int cost = EditorGUILayout.IntField(attack_ref.stats.get_stat_value("cost"));
        attack_ref.stats.set_stat("cost", cost);
        EditorGUILayout.EndHorizontal();
    }
}
