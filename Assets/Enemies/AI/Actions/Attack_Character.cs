using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack_Character")]
public class Attack_Character : Action
{
    public override void Act(StateController controller)
    {
        Vector3 target = controller.attached_character.current_target_character.transform.position;
        Vector3 offset = new Vector3(.5f, .5f);

        controller.attached_character.disable_hb();
        //DEBUG: Linecast attacks are not triggering properly
        controller.attached_character.queued_attack.shape = attack_shape.single;
        controller.attached_character.queued_attack.do_attack(controller.attached_character, new Vector2Int((int)(target.x + offset.x), (int)(target.y + offset.y)));
        controller.attached_character.decrease_attack();
        controller.attached_character.enable_hb();
    }

    public override int get_utility(StateController controller)
    {
        Vector3 starting_position = controller.transform.position;
        if (controller.attached_character.is_moving)
        {
            starting_position = controller.attached_character.target_position;
        }
        Vector3 target = controller.attached_character.current_target_character.transform.position;
        Vector3 offset = new Vector3(.5f, .5f);

        int range_to_player = (int)Mathf.Abs(Vector2.Distance(starting_position + offset, target + offset));

        if(range_to_player <= controller.attached_character.queued_attack.stats.get_stat_value("Range"))
        {
            return 100;
        }
        return -100;
    }
}
