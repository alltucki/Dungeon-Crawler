using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Have_Target")]
public class Have_Target_Decision : Decision
{
    public override bool Decide(StateController controller)
    {
        return check_target(controller);
    }

    private bool check_target(StateController controller)
    {
        if (controller.attached_character.current_target_square != null)
        {
            return true;
        }
        return false;
    }
}
