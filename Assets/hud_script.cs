using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hud_script : MonoBehaviour {

    public GameObject popup_prefab;

	// Use this for initialization
	void Start () {
        util_ref.hud = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void add_popup(string readout)
    {
        GameObject new_popup = GameObject.Instantiate(popup_prefab);
        new_popup.transform.SetParent(transform);
        new_popup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        new_popup.GetComponent<popup_script>().set_text(readout);
    }
}
