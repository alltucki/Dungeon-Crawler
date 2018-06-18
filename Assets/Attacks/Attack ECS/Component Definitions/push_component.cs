using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class push_component : attack_component
{

    public push_component()
    {
        component_name = "Push";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        bool activated_effect = false;
        for (int i = triggering_attack.target_squares.Count - 1; i >= 0; i--)
        {
            if (util_ref.e_manager.is_occupied(triggering_attack.target_squares[i]))
            {
                activated_effect = true;
                GameObject target = util_ref.e_manager.get_at_square(triggering_attack.target_squares[i]);
                attack_data data = new attack_data();
                data.attacker = attacker;
                data.target = target.GetComponent<rpg_character>();
                data.attack_ref = triggering_attack;
                Vector2Int push_square = get_furthest(data);
                //Debug.Log("Attempted to push enemy to " + push_square.ToString());
                //target.GetComponent<enemy_script>().move(push_square.x, push_square.y);
                target.GetComponent<rpg_character>().disable_hb();
                RaycastHit2D hit = Physics2D.Linecast(target.transform.position + new Vector3(.5f, .5f), push_square);
                if (hit.collider == null)
                {
                    target.GetComponent<rpg_character>().move_abs(push_square.x, push_square.y);
                    Debug.DrawLine(target.transform.position, new Vector3(push_square.x, push_square.y), Color.magenta, 2f);
                }
                else
                {
                    //Take damage based on distance to collider
                    float distance = Mathf.Abs(Vector3.Distance(target.transform.position, hit.point));
                    //target.GetComponent<rpg_character>().take_damage(Mathf.CeilToInt(distance), data);
                    target.GetComponent<rpg_character>().move_abs((int)hit.point.x, (int)hit.point.y);
                    Debug.DrawLine(target.transform.position, hit.point, Color.red, 2f);
                }
                target.GetComponent<rpg_character>().enable_hb();
            }

        }
        return activated_effect;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
        target.stats.init_stat("Push");
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        int push = EditorGUILayout.IntField(attack_ref.stats.get_stat_value("Push"));
        attack_ref.stats.set_stat("Push", push);
    }

    public Vector2Int get_furthest(attack_data data)
    {
        Vector3 offset = new Vector3(.5f, .5f);
        Vector3 push_dir = (data.attacker.transform.position + offset) - (data.target.transform.position + offset);
        push_dir = push_dir.normalized;
        push_dir *= data.attack_ref.stats.get_stat_value("Push");
        Vector3 end_point = (data.target.transform.position + offset) - push_dir;

        return new Vector2Int((int)end_point.x, (int)end_point.y);
    }
}
