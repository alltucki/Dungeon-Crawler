using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effects_script : MonoBehaviour {

    public Animator anim;
    public AudioSource source;
    public GameObject parent;

	// Use this for initialization
	void Start () {
        set_anim();
        set_audio();
    }
	
	// Update is called once per frame
	void Update () {
        set_anim();
        set_audio();

        if (!source.isPlaying)
        {
            if (parent != null)
            {
                parent.GetComponent<rpg_character>().set_unlocked("Unlocked " + parent.name + " after effect ended");
            }
            Destroy(gameObject);
        }

	}

    private void set_anim()
    {
        if(anim == null)
            anim = GetComponent<Animator>();
    }

    private void set_audio()
    {
        if (source == null)
            source = GetComponent<AudioSource>();
    }
}
