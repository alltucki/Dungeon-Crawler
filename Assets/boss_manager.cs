using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum boss_event
{
    increase_budget, buff_health, buff_damage
}

public class boss_manager : MonoBehaviour {

    public boss_event_message_script event_panel;
    public boss_event next_event;
    public countdown_panel_script countdown_panel;
    public string[] events;
    public int countdown_to_next_event, max_countdown;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void inc_turn()
    {
        countdown_to_next_event--;
        countdown_panel.update_fill(countdown_to_next_event, max_countdown);
        if(countdown_to_next_event <= 0)
        {
            execute_current_event();
            start_new_event();
            max_countdown = Random.Range(0, 50) + 51;
            countdown_to_next_event = max_countdown;
        }
    }

    public void execute_current_event()
    {
        switch(next_event)
        {
            case boss_event.buff_damage:
                Debug.LogWarning("Buffed enemy damage");
                break;
            case boss_event.buff_health:
                Debug.LogWarning("Buffed enemy health");
                break;
            case boss_event.increase_budget:
                util_ref.e_manager.max_budget = (int)(util_ref.e_manager.max_budget * 1.5);
                Debug.LogWarning("Increased budget to " + util_ref.e_manager.max_budget);
                break;
            default:
                Debug.LogError("Boss execute current event encountered default case");
                break;
        }

        next_event = (boss_event)Random.Range(0, 3);
    }

    public void start_new_event()
    {
        event_panel.display_message(events[Random.Range(0, events.Length)]);
    }

    public string summon_minion_message()
    {
        int message = Random.Range(0, 3);

        switch(message)
        {
            case 0:
                return "Summmoning minions";
            case 1:
                return "Digging up the dead";
            case 2:
                return "Building an army";
            default:
                return "NULL";
        }
    }
}
