using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class line_limit_targets_component : attack_component {

    public line_limit_targets_component()
    {
        component_name = "Line Limit";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        //Loop through the list of target squares and limit targets to only the first
        //pierce + 1 targets
        int remain_pierce = triggering_attack.stats.get_stat_value("Pierce");
        List<Vector2Int> new_targets = new List<Vector2Int>();

        bool has_targets = false;

        for(int i = 0; i < triggering_attack.target_squares.Count; i++)
        {
            if(remain_pierce > 0)
            {
                if (util_ref.e_manager.is_occupied(triggering_attack.target_squares[i]) || util_ref.p_manager.get_p_pos() == triggering_attack.target_squares[i])
                {
                    new_targets.Add(triggering_attack.target_squares[i]);
                    remain_pierce--;
                    has_targets = true;
                }
            }
        }
        triggering_attack.target_squares = new_targets;

        return has_targets;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
        target.stats.init_stat("Pierce");
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        int limit = EditorGUILayout.IntField(attack_ref.stats.get_stat_value("Pierce"));
        attack_ref.stats.set_stat("Pierce", limit);
    }
}
