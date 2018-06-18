using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openable : Interaction {

    public GameObject replacement;
    public bool locked, block_sight, supress_instantiate;

    // Use this for initialization
    void Start () {
        GetComponent<Interactable>().add(this);

        if(block_sight)
        {
            util_ref.v_manager.transparent[(int)transform.position.x, (int)transform.position.y] = false;
        }

        if (!supress_instantiate)
        {
            replacement = GameObject.Instantiate(replacement);
            replacement.transform.position = new Vector3(0f, 0f);
            if (replacement.GetComponent<openable>() != null)
            {
                replacement.GetComponent<openable>().replacement = gameObject;
                replacement.GetComponent<openable>().supress_instantiate = true;
            }
            util_ref.feature_spawner.features.Add(replacement);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void interact(rpg_character actor)
    {
        if(!locked)
        {
            Vector3 temp_pos = replacement.transform.position;
            replacement.transform.position = transform.position;
            transform.position = temp_pos;

            openable is_openable = replacement.GetComponent<openable>();

            //If we're currently blocking sight, check to see if we should change
            if (block_sight)
            {
                if(is_openable == null)
                {
                    Debug.Log("Set transparent to true");
                    util_ref.v_manager.transparent[(int)replacement.transform.position.x, (int)replacement.transform.position.y] = true;
                    util_ref.v_manager.refresh_fog();
                }
                if(is_openable != null && !is_openable.block_sight)
                {
                    Debug.Log("Set transparent to true");
                    util_ref.v_manager.transparent[(int)replacement.transform.position.x, (int)replacement.transform.position.y] = true;
                    util_ref.v_manager.refresh_fog();
                }
            }

            if(is_openable != null && is_openable.block_sight)
            {
                Debug.Log("Set transparent to false");
                util_ref.v_manager.transparent[(int)replacement.transform.position.x, (int)replacement.transform.position.y] = false;
                util_ref.v_manager.refresh_fog();
            }
        }
    }

    public void set_locked()
    {
        locked = true;
    }

    public void set_unlocked()
    {
        locked = false;
    }

    public void swap_locked()
    {
        locked = !locked;
    }
}
