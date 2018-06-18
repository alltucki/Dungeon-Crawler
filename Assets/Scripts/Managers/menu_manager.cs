using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_manager : MonoBehaviour {

    private GameObject main_menu, character_menu, god_menu;
    private game_manager manager;

	// Use this for initialization
	void Start () {
        main_menu = GameObject.Find("Main Menu Canvas");
        character_menu = GameObject.Find("Character Select Canvas");
        god_menu = GameObject.Find("God Select Canvas");
        character_menu.SetActive(false);
        god_menu.SetActive(false);

        manager = GameObject.Find("Game Manager").GetComponent<game_manager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void launch_character_menu()
    {
        main_menu.SetActive(false);
        character_menu.SetActive(true);
    }

    public void launch_god_menu()
    {
        character_menu.SetActive(false);
        god_menu.SetActive(true);
    }

    public void choose_god(int index)
    {

    }

    public void launch_game()
    {
        Debug.Log("Launched game");
        manager.init_game();
    }
}
