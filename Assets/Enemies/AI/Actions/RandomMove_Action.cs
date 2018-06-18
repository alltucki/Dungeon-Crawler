using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/RandomMove")]
public class RandomMove_Action : Action {

    public override int get_utility(StateController controller)
    {
        return 1;
    }

    public override void Act(StateController controller)
    {
        move_random(controller);
    }

    private void move_random(StateController controller)
    {
        if (controller.attached_character.get_move_remaining() <= 0)
        {
            return;
        }

        int total_squares = 0;
        Vector3 pos_ref = controller.attached_character.transform.position;
        Vector2Int cur_pos = new Vector2Int((int)pos_ref.x, (int)pos_ref.y);
        List<Vector2> possible_squares = new List<Vector2>();

        if (util_ref.okay_to_move(cur_pos + util_ref.up))
        {
            possible_squares.Add(util_ref.up);
            total_squares++;
        }

        if (util_ref.okay_to_move(cur_pos + util_ref.down))
        {
            possible_squares.Add(util_ref.down);
            total_squares++;
        }

        if (util_ref.okay_to_move(cur_pos + util_ref.left))
        {
            possible_squares.Add(util_ref.left);
            total_squares++;
        }

        if (util_ref.okay_to_move(cur_pos + util_ref.right))
        {
            possible_squares.Add(util_ref.right);
            total_squares++;
        }

        if (total_squares == 0)
        {
            return;
        }
        Vector2 target = possible_squares[UnityEngine.Random.Range(0, total_squares)];
        controller.attached_character.decrease_move();
        controller.attached_character.move_relative((int)target.x, (int)target.y);
    }
}
