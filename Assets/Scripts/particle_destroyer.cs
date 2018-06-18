using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_destroyer : MonoBehaviour {

    ParticleSystem particle;

	// Use this for initialization
	void Start () {
        particle = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!particle.IsAlive())
        {
            Destroy(gameObject);
        }
	}
}
