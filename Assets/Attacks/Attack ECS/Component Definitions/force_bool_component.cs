using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class force_bool_component : attack_component {

    public bool return_val;

    public force_bool_component()
    {
        component_name = "Force bool";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        return return_val;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        return_val = EditorGUILayout.Toggle(return_val);
    }
}
