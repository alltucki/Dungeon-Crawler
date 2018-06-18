using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class applies_condition_attacker_component : attack_component {

    public temporary_condition condition;

    public applies_condition_attacker_component()
    {
        component_name = "Applies condition attacker";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        if (triggering_attack.activated_effect)
        {
            temporary_condition new_condition = Object.Instantiate(condition) as temporary_condition;
            new_condition.source_character = attacker;
            new_condition.source_attack = triggering_attack;
            new_condition.add_effect(attacker);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        condition = (temporary_condition)EditorGUILayout.ObjectField(
                condition,
                typeof(temporary_condition),
                false);
    }
}
