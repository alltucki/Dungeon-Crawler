using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exit_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player touched exit");
        
        if(collision.gameObject.tag == "Player")
        {
            GameObject.Find("Game Manager").GetComponent<game_manager>().unload_level();
        }
        
    }
}
