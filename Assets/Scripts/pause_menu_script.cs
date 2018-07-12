using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pause_menu_script : MonoBehaviour {

    public GameObject item_prefab;
    private GameObject inventory, equipment;

    public GameObject[] equip_slots, inv_slots;

    public GameObject description;

	// Use this for initialization
	void Start () {
        inventory = transform.GetChild(0).gameObject;
        equipment = transform.GetChild(1).gameObject;

        description = transform.GetChild(3).GetChild(0).gameObject;

        equip_slots = new GameObject[4];
        inv_slots = new GameObject[16];

        for(int i = 0; i < equip_slots.Length; i++)
        {
            equip_slots[i] = transform.GetChild(1).GetChild(i).gameObject;
        }
        for(int i = 0; i < inv_slots.Length; i++)
        {
            inv_slots[i] = transform.GetChild(0).GetChild(i).gameObject;
        }

        set_inv();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void unequip_ui(int start_equip, int end_inv)
    {
        DragAndDropItem temp = equip_slots[start_equip].GetComponent<DragAndDropCell>().GetItem();
        inv_slots[end_inv].GetComponent<DragAndDropCell>().AddItem(temp);
        equip_slots[start_equip].GetComponent<DragAndDropCell>().RemoveItem();
    }

    public void update_description(int equip_slot)
    {
        equipment ref_item = util_ref.p_manager.cur_player.GetComponent<player_script>().inv.get_equipped(equip_slot);
        string desc = "";
        if(ref_item != null && ref_item.skills.Length > 0)
        {
            desc += ref_item.skills[0].name;
        }

        desc += "\nWeapon limit: " + util_ref.p_manager.cur_player.GetComponent<player_script>().inv.get_weapon_limit().ToString();
        desc += "\nArmor limit: " + util_ref.p_manager.cur_player.GetComponent<player_script>().inv.get_armor_limit().ToString();
        desc += "\nTrinket limit: " + util_ref.p_manager.cur_player.GetComponent<player_script>().inv.get_trinket_limit().ToString();

        description.GetComponent<Text>().text = desc;
    }

    public void set_inv()
    {
        //Set equipment items
        for(int i = 0; i < 4; i++)
        {
            equipment ref_item = util_ref.p_manager.cur_player.GetComponent<player_script>().inv.get_equipped(i);
            if(ref_item != null)
            {
                GameObject new_item = GameObject.Instantiate(item_prefab);
                new_item.GetComponent<Image>().sprite = ref_item.icon;
                new_item.GetComponent<DragAndDropItem>().attached_item = ref_item;
                equipment.transform.GetChild(i).GetComponent<DragAndDropCell>().AddItem(new_item.GetComponent<DragAndDropItem>());
            }
        }

        //Set inventory items
        for(int i = 0; i < 16; i++)
        {
            equipment ref_item = util_ref.p_manager.cur_player.GetComponent<player_script>().inv.get_inventory(i);
            if(ref_item != null)
            {
                GameObject new_item = GameObject.Instantiate(item_prefab);
                new_item.GetComponent<Image>().sprite = ref_item.icon;
                new_item.GetComponent<DragAndDropItem>().attached_item = ref_item;
                inventory.transform.GetChild(i).GetComponent<DragAndDropCell>().AddItem(new_item.GetComponent<DragAndDropItem>());
            }
        }
    }

    public void test_end_drag(int i)
    {
        Debug.Log(i.ToString());
    }
}
