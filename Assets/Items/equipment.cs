using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum equip_type
{
    weapon, armor, trinket
}

public class equipment : item {
    public equip_type type;
    public int limit_add;
    public attack_entity[] skills;
    public List<Perk> perks;

    public void init()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i] = Object.Instantiate(skills[i]);
        }
    }
}
