using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour {

    public State current_state;
    public State remain_state;
    public rpg_character attached_character;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(attached_character.get_remaining_actions() > 0)
        {
            current_state.update_state(this);
        }
	}

    public void transition_to_state(State next_state)
    {
        if(next_state != remain_state)
        {
            //Debug.Log(gameObject.name + " transitioned to " + next_state.name);
            current_state = next_state;
        }
    }
}
