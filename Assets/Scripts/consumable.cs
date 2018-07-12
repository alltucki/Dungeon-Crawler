using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable")]
public class consumable : item {
    public temp_effect effect;

    private void Awake()
    {
        //effect = Object.Instantiate(effect);
    }
}
