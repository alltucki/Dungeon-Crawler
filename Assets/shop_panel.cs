using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shop_panel : MonoBehaviour {

    public Shopkeeper clerk;
    public GameObject[] stock;

	// Use this for initialization
	void Start () {
        stock = new GameObject[3];
        for(int i = 0; i < stock.Length; i++)
        {
            stock[i] = transform.GetChild(i).gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void set_clerk(Shopkeeper new_keeper)
    {
        clerk = new_keeper;
        for(int i = 0; i < clerk.stock.Length; i++)
        {
            transform.GetChild(i).GetComponent<shop_item_panel>().set_item(clerk.stock[i], clerk.costs[i]);
        }
    }

    public void checkout(int index)
    {
        clerk.try_buy(index);
    }

    public void close_shop()
    {
        Destroy(transform.parent.gameObject);
    }
}
