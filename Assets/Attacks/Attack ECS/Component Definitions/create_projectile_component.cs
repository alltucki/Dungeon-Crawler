using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class create_projectile_component : attack_component
{
    public GameObject projectile;
    public projectile_type type;

    public create_projectile_component()
    {
        component_name = "Create Projectile";
    }

    public override bool activate(rpg_character attacker, attack_entity triggering_attack)
    {
        Debug.Log("Looping through target squares list of length " + triggering_attack.target_squares.Count);
        for(int i = 0; i < triggering_attack.target_squares.Count; i++)
        {
            GameObject new_projectile = GameObject.Instantiate(projectile);
            new_projectile.transform.position = attacker.transform.position + new Vector3(.5f, .5f, 0f);
            Vector3 target_square = new Vector3(triggering_attack.target_squares[i].x + .5f, triggering_attack.target_squares[i].y + .5f);
            new_projectile.GetComponent<projectile_script>().generate_midpoint(attacker.transform.position, target_square);
            Debug.Log("Created projectile targeting " + target_square.ToString());
            new_projectile.GetComponent<projectile_script>().target = target_square;
            new_projectile.GetComponent<projectile_script>().type = type;
        }
        if(triggering_attack.target_squares.Count > 0)
        {
            return true;
        }
        return false;
    }

    public override void add(attack_entity target)
    {
        target.add_component(this);
    }

    public override void editor_layout(attack_entity attack_ref)
    {
        projectile = (GameObject)EditorGUILayout.ObjectField(
                projectile,
                typeof(GameObject),
                false);
        type = (projectile_type)EditorGUILayout.EnumPopup(type);
    }
}
