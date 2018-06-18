using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class applies_condition_target_component : attack_component
{
    public temporary_condition condition;

    public applies_condition_target_component()
    {
        component_name = "Applies condition target";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        bool did_apply = false;
        for(int i = 0; i < triggering_attack.target_squares.Count; i++)
        {
            if(util_ref.e_manager.is_occupied(triggering_attack.target_squares[i]))
            {
                temporary_condition new_condition = Object.Instantiate(condition) as temporary_condition;
                new_condition.source_character = attacker;
                new_condition.source_attack = triggering_attack;
                new_condition.add_effect(util_ref.e_manager.get_at_square(triggering_attack.target_squares[i]).GetComponent<rpg_character>());
                did_apply = true;
            }
        }
        return did_apply;
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
