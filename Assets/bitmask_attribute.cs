using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Referencing https://answers.unity.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html

public class bitmask_attribute : PropertyAttribute {

    public System.Type prop_type;

    public bitmask_attribute(System.Type type)
    {
        prop_type = type;
    }
}
