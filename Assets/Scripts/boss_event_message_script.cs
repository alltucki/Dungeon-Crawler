using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boss_event_message_script : MonoBehaviour {

    RectTransform rect_transform;
    Text child_text;
    public bool withdrawing, displaying;
    public float wait_time;

	// Use this for initialization
	void Start () {
        child_text = transform.GetChild(0).GetComponent<Text>();
        rect_transform = GetComponent<RectTransform>();
        util_ref.b_manager.event_panel = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(withdrawing && displaying)
        {
            Debug.LogWarning("Boss message has contradictory conditions");
        }
        if(wait_time > 0f)
        {
            wait_time -= Time.deltaTime;
            if(wait_time <= 0f)
            {
                withdrawing = true;
            }
        }
        if(withdrawing)
        {
            withdraw();
        }
        if(displaying)
        {
            display();
        }
	}

    public void display_message(string message)
    {
        child_text.text = message;
        displaying = true;
    }

    private void withdraw()
    {
        rect_transform.position = new Vector3(rect_transform.position.x + (Time.deltaTime * 100f), rect_transform.position.y);
        if (rect_transform.anchoredPosition.x >= 500f)
        {
            withdrawing = false;
        }
    }

    private void display()
    {
        rect_transform.position = new Vector3(rect_transform.position.x - (Time.deltaTime * 100f), rect_transform.position.y);
        if(rect_transform.anchoredPosition.x <= 0f)
        {
            wait_time = 1f;
            displaying = false;
        }
    }
}
