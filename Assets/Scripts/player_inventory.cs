using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Contains the items in inventory,
 * as well as the skills that the items have attached
 */ 
[System.Serializable]
public class player_inventory
{
    private List<attack_entity> skills;
    [SerializeField]private equipment[] equipped = new equipment[4];
    private equipment[] equipment_list = new equipment[16];

    private int weapon_limit, armor_limit, trinket_limit;

    public player_script attached_character;

    public GameObject hotbar;

    public void init(player_script character)
    {
        attached_character = character;

        hotbar = GameObject.Find("Hotbar");
        skills = new List<attack_entity>();

        update_hotbar();
    }

    public void equip(equipment new_equip, int slot)
    {
        change_equip_limit(new_equip, 1);

        if (equipped[slot] != null)
        {
            unequip(slot);
        }
        equipped[slot] = new_equip;
        if (new_equip.perks.Count > 0)
        {
            for (int i = 0; i < new_equip.perks.Count; i++)
            {
                new_equip.perks[i].add_perk();
            }
        }

        check_equip_limit(slot);
    }

    public void equip(equipment new_equip)
    {
        change_equip_limit(new_equip, 1);

        bool did_equip = false;
        for(int i = 0; i < equipped.Length; i++)
        {
            if(!did_equip && equipped[i] == null)
            {
                equipped[i] = new_equip;
                if (new_equip.perks.Count > 0)
                {
                    for (int j = 0; j < new_equip.perks.Count; j++)
                    {
                        new_equip.perks[j].add_perk();
                    }
                }
                did_equip = true;
                //Debug.Log("Equipped " + new_equip.name + " to slot " + i.ToString());
            }
        }

        if(!did_equip)
        {
            equip(new_equip, 0);
        }
    }

    public int unequip(int slot)
    {
        Debug.Log("Unequipped slot " + slot.ToString());
        if(equipped[slot] == null)
        {
            return -1;
        }

        equipment temp = equipped[slot];
        change_equip_limit(equipped[slot], -1);
        if (temp.perks.Count > 0)
        {
            for (int i = 0; i < temp.perks.Count; i++)
            {
                temp.perks[i].remove_perk();
            }
        }

        /*
        for(int i = 0; i < temp.skills.Length; i++)
        {
            skills.Remove(temp.skills[i]);
        }
        */

        //Find the first empty spot
        int found_slot = pickup(temp);
        equipped[slot] = null;
        Debug.Log("Unequipped target");
        print_inv();

        return found_slot;
    }

    public int pickup(equipment new_equip)
    {
        //Find the first empty spot
        for (int i = 0; i < equipment_list.Length; i++)
        {
            if (equipment_list[i] == null || equipment_list[i].GetInstanceID() == new_equip.GetInstanceID())
            {
                equipment_list[i] = new_equip;
                return i;
            }
        }
        return -1;
    }

    public void pickup(equipment new_equip, int slot)
    {
        if (equipment_list[slot] != null)
        {
            Debug.LogWarning("Overwrote " + equipment_list[slot].name + " with " + new_equip.name);
        }
        equipment_list[slot] = new_equip;
    }

    public void use_consumable(item new_item)
    {

    }

    public void pickup(item new_item)
    {
        switch(new_item.i_type)
        {
            case item_type.consumable:
                Debug.Log("Picked up consumable");
                break;
            case item_type.equipment:
                pickup((equipment)new_item);
                break;
            default:
                Debug.LogError("New item did not have attached type");
                break;
        }
    }

    public void drop(int slot)
    {
        equipment_list[slot] = null;
    }

    private void change_equip_limit(equipment new_equip, int change_mod)
    {
        switch (new_equip.type)
        {
            case equip_type.weapon:
                weapon_limit += (new_equip.limit_add * change_mod);
                break;
            case equip_type.armor:
                armor_limit += (new_equip.limit_add * change_mod);
                break;
            case equip_type.trinket:
                trinket_limit += (new_equip.limit_add * change_mod);
                break;
            default:
                Debug.LogError("Change equip limit did not have equip type");
                break;
        }
    }

    private void check_equip_limit(int lock_slot)
    {
        if(get_weapon_limit() > 2)
        {
            bool removed = false;
            for(int i = 0; i < equipped.Length; i++)
            {
                if (equipped[i] != null)
                {
                    if (!removed && equipped[i].type == equip_type.weapon && i != lock_slot)
                    {
                        Debug.Log("Unequipping slot " + i + ", " + equipped[i].name + " to reduce weapon limit");
                        int slot = unequip(i);
                        GameObject.Find("Pause Menu Canvas").GetComponent<pause_menu_script>().unequip_ui(i, slot);
                        if (get_weapon_limit() <= 2)
                        {
                            removed = true;
                        }
                        Debug.Log("Weapon limit is now " + get_weapon_limit().ToString());
                    }
                }
            }
            if(!removed)
            {
                Debug.Log("Could not make enough room in inventory. Unequipping original item");
                unequip(lock_slot);
            }
        }
        if (get_armor_limit() > 1)
        {
            bool removed = false;
            for (int i = 0; i < equipped.Length; i++)
            {
                if (equipped[i] != null)
                {
                    if (!removed && equipped[i].type == equip_type.armor && i != lock_slot)
                    {
                        int slot = unequip(i);
                        GameObject.Find("Pause Menu Canvas").GetComponent<pause_menu_script>().unequip_ui(i, slot);
                        if (get_armor_limit() <= 1)
                        {
                            removed = true;
                        }
                    }
                }
            }
            if (!removed)
            {
                unequip(lock_slot);
            }
        }
        if (get_trinket_limit() > 2)
        {
            bool removed = false;
            for (int i = 0; i < equipped.Length; i++)
            {
                if (equipped[i] != null)
                {
                    if (!removed && equipped[i].type == equip_type.trinket && i != lock_slot)
                    {
                        int slot = unequip(i);
                        GameObject.Find("Pause Menu Canvas").GetComponent<pause_menu_script>().unequip_ui(i, slot);
                        if (get_trinket_limit() <= 2)
                        {
                            removed = true;
                        }
                    }
                }
            }
            if (!removed)
            {
                unequip(lock_slot);
            }
        }
        
    }

    public void add_attack_to_hotbar(attack_entity new_attack)
    {
        GameObject new_hotbar = GameObject.Instantiate(util_ref.hotbar_prefab);
        new_hotbar.GetComponent<Image>().sprite = new_attack.icon;

        new_hotbar.transform.SetParent(hotbar.transform);
    }

    public bool do_attack(int i, GameObject attacker, Vector3 target)
    {
        bool has_attacked = skills[i].do_attack(attached_character.get_character(), new Vector2Int((int)target.x, (int)target.y));

        return has_attacked;
    }

    public void queue_skill(int i)
    {
        hotbar.GetComponent<hotbar_script>().set_all_white();
        attached_character.get_character().queued_attack = get_skill(i);
        hotbar.GetComponent<hotbar_script>().set_highlight(i);
    }

    public void update_hotbar()
    {
        set_skills();

        int count = hotbar.transform.childCount;
        foreach(Transform t in hotbar.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        for (int i = 0; i < skills.Count; i++)
        {
            Debug.Log("Adding " + skills[i].name + " to hotbar");
            add_attack_to_hotbar(skills[i]);
        }
        hotbar.GetComponent<hotbar_script>().recalculate_numbers();
    }

    public void set_skills()
    {
        skills.RemoveRange(0, skills.Count);
        Debug.Log("Removed all skills");

        Debug.Log("Going through equipped of length " + equipped.Length);
        for(int i = 0; i < equipped.Length; i++)
        {
            if(equipped[i] != null)
            {
                Debug.Log("Slot " + i.ToString() + " is not null");
                for(int k = 0; k < equipped[i].skills.Length; k++)
                {
                    skills.Add(equipped[i].skills[k]);
                }
            }
        }

        Debug.Log("Skills count: " + skills.Count.ToString());
    }

    public void swap_skill(attack_entity current_skill, attack_entity new_skill)
    {
        int index = skills.IndexOf(current_skill);
        skills[index] = new_skill;
        if(attached_character.get_character().queued_attack == current_skill)
        {
            attached_character.get_character().queued_attack = new_skill;
        }
        hotbar.GetComponent<hotbar_script>().change_sprite(index, skills[index].icon);
        hotbar.GetComponent<hotbar_script>().set_all_white();
    }

    public attack_entity get_skill(int i)
    {
        return skills[i];
    }

    public int get_skill_index(attack_entity reference)
    {
        return skills.IndexOf(reference);
    }

    public List<attack_entity> get_weapon_skills()
    {
        return skills;
    }

    public equipment get_equipped(int slot)
    {
        //Debug.Log("Getting equipped for slot " + slot.ToString());
        //Debug.Log("Returning " + equipped[slot].ToString());
        return equipped[slot];
    }

    public equipment get_inventory(int slot)
    {
        return equipment_list[slot];
    }

    public int get_skill_count()
    {
        return skills.Count;
    }

    public int get_weapon_limit()
    {
        return weapon_limit;
    }

    public int get_armor_limit()
    {
        return armor_limit;
    }

    public int get_trinket_limit()
    {
        return trinket_limit;
    }

    public void print_inv()
    {
        Debug.Log("Equipped:");
        for(int i = 0; i < equipped.Length; i++)
        {
            if(equipped[i] != null)
            {
                Debug.Log(i.ToString() + ": " + equipped[i].name);
            }
        }

        Debug.Log("In inventory: ");
        for (int i = 0; i < equipment_list.Length; i++)
        {
            if (equipment_list[i] != null)
            {
                Debug.Log(i.ToString() + ": " + equipment_list[i].name);
            }
        }
    }
}

