using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/State")]
public class State : ScriptableObject {

    public Action[] actions;
    public Action best_choice;
    public Transition[] transitions;

    public void update_state(StateController controller)
    {
        //Debug.Log(controller.attached_character.name + " updating state " + name);
        calculate_utility(controller);
        CheckTransitions(controller);
    }

    private void calculate_utility(StateController controller)
    {
        int highest_utility = int.MinValue;

        for(int i = 0; i < actions.Length; i++)
        {
            int cur_util = actions[i].get_utility(controller);
            //Debug.Log("Calculated utility of " + actions[i].name + " as " + cur_util.ToString());
            if(cur_util > highest_utility)
            {
                best_choice = actions[i];
                highest_utility = cur_util;
            }
        }
        //Debug.Log(controller.attached_character.gameObject.name + " did action " + best_choice.name);
        best_choice.Act(controller);
    }

    private void CheckTransitions(StateController controller)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decision_succeed = transitions[i].decision.Decide(controller);
            if(decision_succeed)
            {
                controller.transition_to_state(transitions[i].trueState);
            }
            else {
                controller.transition_to_state(transitions[i].falseState);
            }
        }
    }
}
