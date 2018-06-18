using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class effect_on_attacker_component : attack_component
{
    public GameObject effect_prefab;

    public effect_on_attacker_component()
    {
        component_name = "Effect on attacker";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        GameObject new_effect = GameObject.Instantiate(effect_prefab);
        new_effect.transform.position = attacker.transform.position;
        return true;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        effect_prefab = (GameObject)EditorGUILayout.ObjectField(
                effect_prefab,
                typeof(GameObject),
                false);
    }
}
