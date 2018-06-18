using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class character_select : MonoBehaviour {

    public GameObject attack_one, attack_two, passive;

	// Use this for initialization
	void Start () {
        attack_one = GameObject.Find("Attack 1");
        attack_two = GameObject.Find("Attack 2");
        passive = GameObject.Find("Passive");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void set_attack_one(attack_entity template)
    {
        attack_one.GetComponent<Image>().sprite = template.icon;
        attack_one.transform.GetChild(0).GetComponent<Text>().text = attack_description(template);
    }

    public void set_attack_two(attack_entity template)
    {
        attack_two.GetComponent<Image>().sprite = template.icon;
        attack_two.transform.GetChild(0).GetComponent<Text>().text = attack_description(template);
    }

    /*
    public void set_passive(Conditional condition)
    {
        passive.GetComponent<Text>().text = condition.conditional_name;
    }
    */

    private string attack_description(attack_entity template)
    {
        string return_string = template.name;
        return_string += "\n" + template.stats.get_stat_value("Damage").ToString() + " Dmg / ";
        return_string += template.stats.get_stat_value("Range") + " Range";

        return return_string;
    }
}
