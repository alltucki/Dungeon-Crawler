using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class palette : MonoBehaviour {

    public GameObject[] enemies;

    public loot_table container_drops, vendor_drops;

    public GameObject[] decorations;

	// Use this for initialization
	void Start () {
        util_ref.cur_palette = this;
        container_drops.create_weighted_array();
        vendor_drops.create_weighted_array();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}