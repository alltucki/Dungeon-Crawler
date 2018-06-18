using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum hands
{
    one_hand, two_hand
}

[CreateAssetMenu (menuName = "Items/Equipment/Weapon")]
public class weapon : equipment {

    public hands handedness;

    private void OnEnable()
    {
        //Debug.Log("On Enable called for " + name);
        
    }
}
