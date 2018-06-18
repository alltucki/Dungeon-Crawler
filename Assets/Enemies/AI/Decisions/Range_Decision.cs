using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="PluggableAI/Decisions/In-Range")]
public class Range_Decision : Decision {

    public override bool Decide(StateController controller)
    {
        bool in_range = check_range(controller);
        return in_range;
    }

    private bool check_range(StateController controller)
    {
        float dist = Vector2Int.Distance(controller.attached_character.get_pos(), util_ref.p_manager.get_p_pos());
        dist = Mathf.Abs(dist);

        if(dist < controller.attached_character.queued_attack.stats.get_stat_value("Range"))
        {
            return true;
        }
        return false;
    }
}
