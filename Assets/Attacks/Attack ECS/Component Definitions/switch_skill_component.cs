using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class switch_skill_component : attack_component
{
    public attack_entity replacement_skill;

    public switch_skill_component()
    {
        component_name = "Switch skill";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        if (triggering_attack.activated_effect)
        {
            if (attacker.type == character_type.enemy)
            {
                attacker.queued_attack = replacement_skill;
            }
            else if (attacker.type == character_type.player)
            {
                attacker.GetComponent<player_script>().inv.swap_skill(triggering_attack, replacement_skill);
            }
        }

        return triggering_attack.activated_effect;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        replacement_skill = (attack_entity)EditorGUILayout.ObjectField(
                replacement_skill,
                typeof(attack_entity),
                false);
    }
}
