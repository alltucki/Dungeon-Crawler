using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Has_Stat_Criteria : Criteria
{
    public check_target criteria_target;
    public string stat_check;

    public override bool check_criteria()
    {
        if(criteria_target == check_target.attacker)
        {
            return (util_ref.events.last_attack.attacker.stats.has_stat(stat_check));
        }
        else if(criteria_target == check_target.target)
        {
            return (util_ref.events.last_attack.target.stats.has_stat(stat_check));
        }

        return false;
    }
}
