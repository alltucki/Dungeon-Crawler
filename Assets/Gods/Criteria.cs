using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum check_target
{
    attacker, target
}

[System.Serializable]
public abstract class Criteria : ScriptableObject {

    public abstract bool check_criteria();
}
