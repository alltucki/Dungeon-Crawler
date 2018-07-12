using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class add_charge_on_event_component : attack_component
{
    [SerializeField] string target_event, charge_name;
    [SerializeField] int max_charge;

    public add_charge_on_event_component()
    {
        component_name = "Add charge on event";
    }

    public override void on_equip()
    {
        util_ref.events.start_listening(target_event, add_charge);
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        return triggering_attack.activated_effect;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public void add_charge()
    {
        Debug.Log("Activated add_charge");
        if (!util_ref.events.last_actor.stats.has_stat(charge_name))
        {
            //Debug.Log(util_ref.events.last_actor + " did not have stat " + charge_name + ". Initializing...");
            util_ref.events.last_actor.stats.init_stat(charge_name);
            util_ref.events.last_actor.stats.set_stat_max(charge_name, max_charge);
        }
        util_ref.events.last_actor.stats.get_stat(charge_name).inc_value();
        //Debug.Log("Incremented " + charge_name + " on " + util_ref.events.last_actor);
        //Debug.Log("new value: " + util_ref.events.last_actor.stats.get_stat_value(charge_name));
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        target_event = EditorGUILayout.TextField(target_event);
        charge_name = EditorGUILayout.TextField(charge_name);
        max_charge = EditorGUILayout.IntField(max_charge);      //Probably helps to be able to set max value to non-zero...
    }
}
