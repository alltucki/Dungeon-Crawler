using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perk_selection_manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        set_perks();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void set_perks()
    {
        Perk[] options = util_ref.perks.perk_ref;
        //Shuffle the array
        for(int i = 0; i < options.Length; i++)
        {
            int random_index = Random.Range(0, options.Length);
            Perk temp = options[random_index];
            options[random_index] = options[i];
            options[i] = temp;
        }
        util_ref.perks.option_a = options[0];
        util_ref.perks.option_b = options[1];
        util_ref.perks.option_c = options[2];
        transform.GetChild(0).GetChild(0).GetComponent<perk_selector>().set_perk(util_ref.perks.option_a);
        transform.GetChild(0).GetChild(1).GetComponent<perk_selector>().set_perk(util_ref.perks.option_b);
        transform.GetChild(0).GetChild(2).GetComponent<perk_selector>().set_perk(util_ref.perks.option_c);
    }

    public void select_perk(int index)
    {
        Debug.Log("Selected option " + index.ToString());
        switch(index)
        {
            case 0:
                util_ref.perks.option_a.add_perk();
                break;
            case 1:
                util_ref.perks.option_b.add_perk();
                break;
            case 2:
                util_ref.perks.option_c.add_perk();
                break;
            default:
                Debug.LogError("Select perk hit default case");
                break;
        }
        Destroy(gameObject);
    }
}
