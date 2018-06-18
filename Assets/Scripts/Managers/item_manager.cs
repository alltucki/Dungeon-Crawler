using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item_manager : MonoBehaviour {

    public GameObject pickup_prefab;
    public item[] items;
    public List<GameObject> cur_items;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void gen_items()
    {
        Debug.Log("Generating items");
        for(int i = 0; i < 5; i++)
        {
            GameObject new_item = GameObject.Instantiate(pickup_prefab);
            new_item.GetComponent<pickupable>().attached_item = items[Random.Range(0, items.Length)];
            cur_items.Add(new_item);
            util_ref.feature_spawner.spawn_feature(new_item);
        }
    }

    public GameObject get_item()
    {
        GameObject new_item = GameObject.Instantiate(pickup_prefab);
        new_item.GetComponent<pickupable>().attached_item = items[Random.Range(0, items.Length)];
        cur_items.Add(new_item);
        return new_item;
    }

    public item get_item_obj()
    {
        return items[Random.Range(0, items.Length)];
    }

    public void unload_item()
    {
        int count = cur_items.Count;
        for(int i = 0; i < count; i++)
        {
            Destroy(cur_items[i]);
        }
        cur_items.RemoveRange(0, count);
    }
}
