using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class hover_box : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Sprite on_hover, on_exit;
    public GameObject attached_player;
    public equipment[] equipment;
    private character_select char_select;

	// Use this for initialization
	void Start () {
        char_select = GameObject.Find("Character Select Canvas").GetComponent<character_select>();
        for(int i = 0; i < equipment.Length; i++)
        {
            equipment[i] = Object.Instantiate(equipment[i]);
            equipment[i].init();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerEnter(PointerEventData data)
    {
        GetComponent<Image>().sprite = on_hover;
        player_script player = attached_player.GetComponent<player_script>();
        //char_select.set_attack_one(player.skills[0]);
        //char_select.set_attack_two(player.skills[1]);
        /*
        if(player.always_on.Count != 0)
        {
            char_select.set_passive(player.always_on[0]);
        }
        if (player.on_attack_perm.Count != 0)
        {
            char_select.set_passive(player.on_attack_perm[0]);
        }
        if (player.on_attack_temp.Count != 0)
        {
            char_select.set_passive(player.on_attack_temp[0]);
        }
        */
    }

    public void OnPointerExit(PointerEventData data)
    {
        GetComponent<Image>().sprite = on_exit;
    }
}
