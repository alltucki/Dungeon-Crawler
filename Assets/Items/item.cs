using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum item_type
{
    consumable, equipment
}

[System.Serializable]
public class item : ScriptableObject {
    public Sprite icon;
    public item_type i_type;
}
