using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap_script : MonoBehaviour {

    public GameObject effect_prefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trap was tripped!");
        /*
        if(collision.gameObject.tag == "Player")
        {
            explode();
            collision.gameObject.GetComponent<player_script>().lose_health(5, true);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            explode();
            collision.gameObject.GetComponent<enemy_script>().take_damage(5);
            Destroy(gameObject);
        }
        */
        
    }

    private void explode()
    {
        GameObject explosion_effect = GameObject.Instantiate(effect_prefab);
        explosion_effect.transform.position = transform.position;
        //explosion_effect.GetComponent<effects_script>().set_effect(effect.fire);
        //explosion_effect.GetComponent<effects_script>().set_loops(1);
        //explosion_effect.GetComponent<effects_script>().set_sfx(sfx_index.explosion_02);
    }
}
