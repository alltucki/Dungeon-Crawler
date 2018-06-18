using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class direct_damage_component : attack_component
{

    public direct_damage_component()
    {
        component_name = "Direct damage";
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
        target.stats.init_stat("Damage");
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        bool made_attack = false;
        for (int i = 0; i < triggering_attack.target_squares.Count; i++)
        {
            //If the square is occupied, call the take damage method on the enemy at that point
            if (util_ref.e_manager.is_occupied(triggering_attack.target_squares[i]))
            {
                if(attacker.tag != "Enemy")
                {
                    made_attack = true;
                    attack_data data = new attack_data();
                    data.attacker = attacker;
                    data.attack_ref = triggering_attack;
                    util_ref.e_manager.get_at_square(triggering_attack.target_squares[i]).
                        GetComponent<rpg_character>().take_damage(data);
                }
            }
            else if (util_ref.p_manager.get_p_pos() == triggering_attack.target_squares[i])
            {
                if (attacker.gameObject.tag != "Player")
                {
                    made_attack = true;
                    attack_data data = new attack_data();
                    data.attacker = attacker;
                    data.attack_ref = triggering_attack;
                    util_ref.p_manager.cur_player.GetComponent<rpg_character>().take_damage(data);
                }
            }
        }
        return made_attack;
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        int damage = EditorGUILayout.IntField(attack_ref.stats.get_stat_value("Damage"));
        attack_ref.stats.set_stat("Damage", damage);
    }
}
