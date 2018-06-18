using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class popup_script : MonoBehaviour {

    Text child_text;
    float alpha;
    bool has_initiated;

	// Use this for initialization
	void Start () {
        if(!has_initiated)
        {
            init();
        }
	}

    void init()
    {
        alpha = 1f;
        child_text = transform.GetChild(0).GetComponent<Text>();
        has_initiated = true;
    }
	
	// Update is called once per frame
	void Update () {
        alpha -= (Time.deltaTime * .5f);
        GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        child_text.color = new Color(0f, 0f, 0f, alpha);
        GetComponent<RectTransform>().position = GetComponent<RectTransform>().position + new Vector3(0f, Time.deltaTime * 50f, 0f);

        if(alpha <= 0f) {
            Destroy(gameObject);
        }
	}

    public void set_text(string new_string)
    {
        if(!has_initiated)
        {
            init();
        }
        child_text.text = new_string;
    }
}
