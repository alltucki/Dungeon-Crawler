using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class effects_on_enemy_component : attack_component
{
    public GameObject effect_prefab;

    public effects_on_enemy_component()
    {
        component_name = "Effect on enemy";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        bool has_triggered = false;
        for (int i = 0; i < triggering_attack.target_squares.Count; i++)
        {
            if (util_ref.e_manager.is_occupied(triggering_attack.target_squares[i]))
            {
                if(attacker.gameObject.tag != "Enemy")
                {
                    has_triggered = true;
                    GameObject new_effect = GameObject.Instantiate(effect_prefab);
                    new_effect.transform.position = new Vector3(triggering_attack.target_squares[i].x, triggering_attack.target_squares[i].y);
                    new_effect.GetComponent<effects_script>().parent = attacker.gameObject;
                    attacker.set_locked("Locked " + attacker.name + " to allow effect to play");
                }
                
            }
            else if(util_ref.p_manager.get_p_pos() == triggering_attack.target_squares[i])
            {
                if(attacker.gameObject.tag != "Player")
                {
                    has_triggered = true;
                    GameObject new_effect = GameObject.Instantiate(effect_prefab);
                    new_effect.transform.position = new Vector3(triggering_attack.target_squares[i].x, triggering_attack.target_squares[i].y);
                }
            }
        }

        return has_triggered;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        effect_prefab = (GameObject)EditorGUILayout.ObjectField(
                effect_prefab,
                typeof(GameObject),
                false);
    }
}