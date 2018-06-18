using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Target")]
public class Target_Action : Action
{
    /*
     * TODO
     * Need to queue attack as well
     */ 
    public override void Act(StateController controller)
    {
        GameObject new_target = GameObject.Instantiate(util_ref.e_manager.target_square_prefab);
        controller.GetComponent<enemy_script>().current_target = new_target;
        controller.attached_character.current_target_square = new_target;

        Vector2Int target_pos = controller.attached_character.current_target_character.get_pos();
        List<Vector2> possible_squares = new List<Vector2>();

        if (okay_target(target_pos + util_ref.up))
        {
            possible_squares.Add(target_pos + util_ref.up);
        }

        if (okay_target(target_pos + util_ref.down))
        {
            possible_squares.Add(target_pos + util_ref.down);
        }

        if (okay_target(target_pos + util_ref.left))
        {
            possible_squares.Add(target_pos + util_ref.left);
        }

        if (okay_target(target_pos + util_ref.right))
        {
            possible_squares.Add(target_pos + util_ref.right);
        }

        Vector2 target = possible_squares[UnityEngine.Random.Range(0, possible_squares.Count)];


        Debug.Log("Targeting " + target.ToString());
        new_target.transform.position = new Vector3(target.x, target.y);
        controller.attached_character.decrease_attack();
    }

    private bool okay_target(Vector2Int square)
    {
        if(util_ref.in_bounds(square.x, square.y) && util_ref.m_gen.is_floor[square.x, square.y])
        {
            return true;
        }
        return false;
    }

    public override int get_utility(StateController controller)
    {
        //If we're out of range, return -1
        //Otherwise, base utility on damage and how close other allies are to the target

        //Basic equation:
        //Increase utility based on distance to target
        //y=-((distance-half range)^2)+10
        float distance_to_target = Vector2.Distance(controller.attached_character.get_pos(), controller.attached_character.current_target_character.get_pos());
        distance_to_target = Mathf.Abs(distance_to_target);

        float half_range = controller.attached_character.queued_attack.stats.get_stat_value("Range") / 2f;

        float n = (distance_to_target - half_range);

        int utility = (int)(-1 * (n * n) + 10);

        return utility * 10;
    }
}
