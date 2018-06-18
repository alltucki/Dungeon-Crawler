using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A generic loot_table wrapper class
[System.Serializable]
public class loot_table
{
    [SerializeField]private item[] items;
    [SerializeField]private int[] weights;
    private int[] weighted_array;

    public loot_table()
    {

    }

    public loot_table(int length)
    {
        items = new item[length];
        weights = new int[length];
    }

    public void set_length(int length)
    {
        items = new item[length];
        weights = new int[length];
    }

    public item get_weighted_item()
    {
        return items[weighted_array[Random.Range(0, weighted_array.Length)]];
    }

    /*
     * Sum the total of all weights, then create an array that contains
     * the indexes of the item
     * When we need to get an item, we can check the weighted array
     * which will have the correct ratio, and just pull a random index
     */
    public void create_weighted_array()
    {
        int sum_of_weights = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            sum_of_weights += weights[i];
        }

        weighted_array = new int[sum_of_weights];
        int index = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            for (int k = 0; k < weights[i]; k++)
            {
                weighted_array[index++] = i;
            }
        }
    }
}

