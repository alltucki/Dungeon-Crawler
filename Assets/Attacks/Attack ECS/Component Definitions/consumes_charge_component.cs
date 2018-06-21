using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class consumes_charge_component : attack_component
{
    [SerializeField] string charge_name;

    public consumes_charge_component()
    {
        component_name = "Consumes charge";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        int cost = triggering_attack.stats.get_stat_value(triggering_attack.name + "_cost");
        int charges = triggering_attack.stats.get_stat_value(charge_name);
        if (charges > cost)
        {
            triggering_attack.stats.set_stat(charge_name, charges - cost);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void add(attack_entity target)
    {
        target.stats.init_stat(target.name + "_cost");
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        string old_name = charge_name;
        EditorGUILayout.BeginHorizontal();
        charge_name = EditorGUILayout.TextField(charge_name);
        int cost = EditorGUILayout.IntField(attack_ref.stats.get_stat_value(attack_ref.name + "_cost"));
        attack_ref.stats.set_stat(attack_ref.name + "_cost", cost);
        EditorGUILayout.EndHorizontal();

        if (attack_ref.stats.has_stat(old_name))
        {
            attack_ref.stats.remove_stat(attack_ref.stats.get_stat(old_name));
        }

        attack_ref.stats.init_stat(charge_name);
    }
}
