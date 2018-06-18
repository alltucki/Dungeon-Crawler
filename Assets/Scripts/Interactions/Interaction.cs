using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum interact_type
{
    non_destructive, destructive
}

public abstract class Interaction : MonoBehaviour {

    public interact_type type;
    public GameObject interact_effect;

    public abstract void interact(rpg_character actor);
}
