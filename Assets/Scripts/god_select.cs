using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class god_select : MonoBehaviour {

    GameObject launch_game_button;
    GameObject god_name, god_encourage_1, god_encourage_2, god_discourage_1;

	// Use this for initialization
	void Start () {
        launch_game_button = GameObject.Find("Launch Game");
        launch_game_button.SetActive(false);

        god_name = GameObject.Find("God Name");
        god_encourage_1 = GameObject.Find("Positive 1");
        god_encourage_2 = GameObject.Find("Positive 2");
        god_discourage_1 = GameObject.Find("Negative 1");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void select_god(string god)
    {
        god_name.GetComponent<Text>().text = god;
        launch_game_button.SetActive(true);
    }
}
