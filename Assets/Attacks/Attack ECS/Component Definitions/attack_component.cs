using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class attack_component : ScriptableObject
{
    public string component_name;

    public attack_component()
    {

    }

    public abstract void add(attack_entity target);

    public abstract bool activate(rpg_character attacker, attack_entity triggering_attack);

    public abstract void editor_layout(attack_entity attack_ref);
}