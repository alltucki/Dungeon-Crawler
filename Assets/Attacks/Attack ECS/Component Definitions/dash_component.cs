using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class dash_component : attack_component
{

    public dash_component()
    {
        component_name = "Dash";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        bool has_dashed = false;
        for (int i = 0; i < triggering_attack.target_squares.Count; i++)
        {
            if (util_ref.e_manager.is_occupied(triggering_attack.target_squares[i]))
            {
                GameObject target = util_ref.e_manager.get_at_square(triggering_attack.target_squares[i]);
                Vector3 adjacent = attacker.transform.position - target.transform.position;
                adjacent = adjacent.normalized;
                adjacent = target.transform.position + adjacent;
                adjacent = new Vector3(Mathf.Round(adjacent.x), Mathf.Round(adjacent.y), 0);


                GameObject afterimage = GameObject.Instantiate(util_ref.sprite_prefab);
                afterimage.transform.position = attacker.transform.position;
                afterimage.GetComponent<SpriteRenderer>().sprite = attacker.GetComponent<SpriteRenderer>().sprite;
                afterimage.AddComponent<fade_effect>();
                afterimage.GetComponent<fade_effect>().speed = .75f;

                //Debug.Log("Adjacent square is: " + adjacent.ToString());
                attacker.transform.position = adjacent;

                util_ref.v_manager.refresh_fog();
                has_dashed = true;
            }
        }
        return has_dashed;
    }

    public override void add(attack_entity target)
    {
        target.stats.init_stat("Dash");
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        return;
    }
}
