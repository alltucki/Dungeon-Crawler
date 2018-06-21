using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class cooldown_component : attack_component
{
    int cur_cooldown;
    attack_entity attack_reference;
    string target_event;        //Stores the name of the event, so we can stop listening

    public cooldown_component()
    {
        component_name = "Cooldown";
    }

    //Cur_cooldown gets serialized, reset to 0 on game start
    private void OnEnable()
    {
        cur_cooldown = 0;
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        if(cur_cooldown > 0)
        {
            triggering_attack.target_squares.Clear();
            return false;
        }
        else
        {
            cur_cooldown = triggering_attack.stats.get_stat_value("Cooldown");
            target_event = attacker.name + "_start_turn";
            util_ref.events.start_listening(target_event, tick);

            //Get the index of the appropriate skill and set the fill to none
            attack_reference = triggering_attack;
            int index_reference = util_ref.p_manager.p_script.inv.get_skill_index(attack_reference);
            util_ref.p_manager.p_script.inv.hotbar.transform.GetChild(index_reference).GetComponent<Image>().fillAmount = 0f;

            return true;
        }
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
        target.stats.init_stat("Cooldown");
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        int max_cooldown = EditorGUILayout.IntField(attack_ref.stats.get_stat_value("Cooldown"));
        attack_ref.stats.set_stat("Cooldown", max_cooldown);
    }

    public void tick()
    {
        cur_cooldown--;
        //Need to check the reference again in case the equipment gets swapped around
        int index_reference = util_ref.p_manager.p_script.inv.get_skill_index(attack_reference);
        float max_cooldown = attack_reference.stats.get_stat_value("Cooldown");
        util_ref.p_manager.p_script.inv.hotbar.transform.GetChild(index_reference).GetComponent<Image>().fillAmount = (float)(max_cooldown - cur_cooldown) / (max_cooldown);

        if (cur_cooldown <= 0)
        {
            util_ref.events.stop_listening(target_event, tick);
        }
    }
}
