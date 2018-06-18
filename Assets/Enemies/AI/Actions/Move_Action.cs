using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum dijkstra_maps
{
    player_distance, player_sight, enemy_location
}

[CreateAssetMenu (menuName = "PluggableAI/Actions/Move")]
public class Move_Action : Action {

    public dijkstra_maps[] consult_maps;
    public float[] dijkstra_mods;
    private Vector2Int best_square;
    private float lowest_cost;

    public override int get_utility(StateController controller)
    {
        float up, down, left, right;
        Vector2Int pos = controller.attached_character.get_pos();
        lowest_cost = int.MaxValue;

        if(util_ref.okay_to_move(util_ref.up + pos))
        {
            up = dijkstra_calc(util_ref.up + pos);
            if(up < lowest_cost)
            {
                lowest_cost = up;
                best_square = util_ref.up + pos;
            }
        }
        if (util_ref.okay_to_move(util_ref.down + pos))
        {
            down = dijkstra_calc(util_ref.down + pos);
            if (down < lowest_cost)
            {
                lowest_cost = down;
                best_square = util_ref.down + pos;
            }
        }
        if (util_ref.okay_to_move(util_ref.left + pos))
        {
            left = dijkstra_calc(util_ref.left + pos);
            if (left < lowest_cost)
            {
                lowest_cost = left;
                best_square = util_ref.left + pos;
            }
        }
        if (util_ref.okay_to_move(util_ref.right + pos))
        {
            right = dijkstra_calc(util_ref.right + pos);
            if (right < lowest_cost)
            {
                lowest_cost = right;
                best_square = util_ref.right + pos;
            }
        }

        return (int)(-10 * lowest_cost) + 10;
    }

    public override void Act(StateController controller)
    {
        controller.attached_character.move_abs(best_square.x, best_square.y);
        controller.attached_character.decrease_move();
    }

    private float dijkstra_calc(Vector2Int target_square)
    {
        float score = 0;

        for(int i = 0; i < consult_maps.Length; i++)
        {
            score += (get_dijkstra(consult_maps[i], target_square) * dijkstra_mods[i]);
        }

        return score / consult_maps.Length;
    }

    private int get_dijkstra(dijkstra_maps ref_map, Vector2Int t)
    {
        switch(ref_map)
        {
            case dijkstra_maps.enemy_location:
                return util_ref.d_manager.enemy_location[t.x, t.y];
            case dijkstra_maps.player_distance:
                return util_ref.d_manager.player_distance[t.x, t.y];
            case dijkstra_maps.player_sight:
                return util_ref.d_manager.player_sight[t.x, t.y];
            default:
                Debug.LogError("Get_Dijkstra returned -1 for map " + ref_map.ToString() + " and square " + t.ToString());
                return -1;
        }
    }
}
