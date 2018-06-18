using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum operation
{
    add, sub, mult, div, set
}

//Modifies a stat
[System.Serializable]
public class Modifier {

    public operation op;
    public int argument;

    public int mod_value(int base_value)
    {
        switch(op)
        {
            case operation.add:
                return base_value + argument;
            case operation.sub:
                return base_value - argument;
            case operation.mult:
                return base_value * argument;
            case operation.div:
                return base_value / argument;
            case operation.set:
                return argument;
            default:
                Debug.LogError("Modifier returned default value");
                return 0;
        }
    }
}
