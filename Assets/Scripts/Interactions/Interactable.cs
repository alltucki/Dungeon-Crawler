using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public List<Interaction> non_destructive, destructive;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void try_interact(rpg_character actor)
    {
        Debug.Log("Tried to interact wtih " + name);
        if(non_destructive.Count > 0)
        {
            for(int i = 0; i < non_destructive.Count; i++)
            {
                non_destructive[i].interact(actor);
                if(non_destructive[i].interact_effect != null)
                {
                    GameObject new_effect = GameObject.Instantiate(non_destructive[i].interact_effect);
                    new_effect.transform.position = transform.position;
                }
            }
        }
        else
        {
            for(int i = 0; i < destructive.Count; i++)
            {
                destructive[i].interact(actor);
                if (destructive[i].interact_effect != null)
                {
                    GameObject new_effect = GameObject.Instantiate(destructive[i].interact_effect);
                    new_effect.transform.position = transform.position;
                }
            }
        }
    }

    public void force_interact(rpg_character actor)
    {
        Debug.Log("Forced interaction with " + name);
        for (int i = 0; i < non_destructive.Count; i++)
        {
            non_destructive[i].interact(actor);
            if (non_destructive[i].interact_effect != null)
            {
                GameObject new_effect = GameObject.Instantiate(non_destructive[i].interact_effect);
                new_effect.transform.position = transform.position;
            }
        }
        for (int i = 0; i < destructive.Count; i++)
        {
            destructive[i].interact(actor);
            if (destructive[i].interact_effect != null)
            {
                GameObject new_effect = GameObject.Instantiate(destructive[i].interact_effect);
                new_effect.transform.position = transform.position;
            }
        }
    }

    public void add(Interaction i)
    {
        if(i.type == interact_type.destructive)
        {
            destructive.Add(i);
        }
        else if(i.type == interact_type.non_destructive)
        {
            non_destructive.Add(i);
        }
    }
}
