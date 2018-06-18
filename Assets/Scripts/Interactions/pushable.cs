using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class pushable : Interaction {

    private void Start()
    {
        GetComponent<Interactable>().add(this);
    }

    public override void interact(rpg_character actor)
    {
        Debug.Log("Pushed pushable");

        Vector3 adjacent = actor.transform.position - transform.position;
        adjacent = adjacent.normalized;
        adjacent = transform.position - adjacent;
        adjacent = new Vector3(Mathf.Round(adjacent.x), Mathf.Round(adjacent.y), 0);

        GetComponent<Rigidbody2D>().MovePosition(new Vector2(adjacent.x, adjacent.y));
    }
}
