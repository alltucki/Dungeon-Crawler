using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum trigger_enum
{
    always, turn_start, on_attack, on_attacked
}

public class event_manager : MonoBehaviour {

    private Dictionary<string, UnityEvent> events;
    public attack_data last_attack;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        events = new Dictionary<string, UnityEvent>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void start_listening(string event_name, UnityAction action)
    {
        if(events.ContainsKey(event_name))
        {
            events[event_name].AddListener(action);
        }
        else
        {
            UnityEvent new_event = new UnityEvent();
            new_event.AddListener(action);
            events.Add(event_name, new_event);
        }
    }

    public void stop_listening(string event_name, UnityAction listener)
    {
        if(events.ContainsKey(event_name))
        {
            events[event_name].RemoveListener(listener);
        }
    }

    public void trigger_event(string event_name)
    {
        //Debug.Log("Triggered " + event_name);
        if (events.ContainsKey(event_name))
        { 
            events[event_name].Invoke();
        }
    }
}
