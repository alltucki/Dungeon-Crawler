using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack_Target")]
public class Attack_Target_Action : Action
{
    public override void Act(StateController controller)
    {
        Debug.Log("Attempting attack");
        Vector3 starting_position = controller.transform.position;
        if (controller.attached_character.is_moving)
        {
            starting_position = controller.attached_character.target_position;
        }

        Vector3 target = controller.attached_character.current_target_square.transform.position;
        Vector3 offset = new Vector3(.5f, .5f);

        int range_to_player = (int)Mathf.Abs(Vector2.Distance(starting_position + offset, target + offset));
        Debug.Log("Found range as " + range_to_player.ToString());
        if (range_to_player <= controller.attached_character.queued_attack.stats.get_stat_value("Range"))
        {
            Debug.Log("Do attack called");
            controller.attached_character.disable_hb();
            Vector2Int offset_target = new Vector2Int((int)(target.x + offset.x), (int)(target.y + offset.y));
            Debug.Log("Sending target as " + offset_target);
            controller.attached_character.queued_attack.do_attack(controller.attached_character, offset_target);
            controller.attached_character.decrease_attack();
            Destroy(controller.attached_character.current_target_square);
            controller.attached_character.enable_hb();
        }
        else
        {
            Debug.Log("Range is greater than " + controller.attached_character.queued_attack.stats.get_stat_value("Range").ToString());
            Destroy(controller.attached_character.current_target_square);
        }
    }

    public override int get_utility(StateController controller)
    {
        return 5;
    }
}
