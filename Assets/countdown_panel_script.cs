using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Control radial fill of a target image
public class countdown_panel_script : MonoBehaviour {

    Image filled_image;

	// Use this for initialization
	void Start () {
        filled_image = GetComponent<Image>();
        util_ref.b_manager.countdown_panel = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void update_fill(float cur_value, float max_value)
    {
        filled_image.fillAmount = (cur_value / max_value);
    }
}
