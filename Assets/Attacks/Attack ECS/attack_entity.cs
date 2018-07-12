using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack_Entity")]
public class attack_entity : ScriptableObject {

    public Sprite icon;
    public attack_shape shape;
    [SerializeField]public Stat_Interface stats;
    [SerializeField]public attack_component[] components;
    [SerializeField][bitmask_attribute(typeof(skill_tags))] skill_tags tags;
    public bool activated_effect;
    public List<Vector2Int> target_squares;

	// Use this for initialization
	void OnEnable () {
        if(stats == null)
        {
            stats = new Stat_Interface();
            stats.initialize();
            stats.init_stat("Range");
            stats.init_stat("Radius");
        }
        target_squares = new List<Vector2Int>();

        if (components == null)
        {
            components = new attack_component[0];
        }
	}

    public bool do_attack(rpg_character attacker, Vector2Int target)
    {
        //Check conditions
        if(!util_ref.v_manager.is_visible[target.x, target.y])
        {
            return false;
        }
        Vector2Int start_pos = new Vector2Int((int)attacker.transform.position.x, (int)attacker.transform.position.y);
        if(Mathf.Abs(Vector2Int.Distance(start_pos, target)) > stats.get_stat_value("Range"))
        {
            return false;
        }

        switch(shape)
        {
            case attack_shape.line:
                return line_attack(attacker, target);
            case attack_shape.single:
                return single_attack(attacker, target);
            case attack_shape.square:
                return area_attack(attacker, target);
            default:
                Debug.LogError("Do attack encountered default case");
                return false;
        }
    }

    private bool activate_effects(rpg_character attacker)
    {
        activated_effect = false;
        for (int i = 0; i < components.Length; i++)
        {
            //Check the function first; if the value is set to true
            //the OR evaluation will short-circuit if it's first
            activated_effect = components[i].activate(attacker, this) || activated_effect;
        }
        return activated_effect;
    }

    public void add_component(attack_component new_component)
    {
        attack_component[] new_components = new attack_component[components.Length + 1];

        for(int i = 0; i < components.Length; i++)
        {
            new_components[i] = components[i];
        }
        new_components[new_components.Length - 1] = new_component;

        components = new_components;
    }

    public void remove_component(int delete_index)
    {
        attack_component[] new_components = new attack_component[components.Length - 1];

        int new_index = 0;
        for(int old_index = 0; old_index < components.Length; old_index++)
        {
            if(old_index != delete_index)
            {
                new_components[new_index++] = components[old_index];
            }
        }

        components = new_components;
    }

    private bool line_attack(rpg_character attacker, Vector2Int target)
    {
        //Get targets between attacker and chosen square
        Vector2Int start_pos = new Vector2Int((int)attacker.gameObject.transform.position.x,
            (int)attacker.gameObject.transform.position.y);
        target_squares.Clear();
        target_squares = line(start_pos.x, start_pos.y, target.x, target.y);

        return activate_effects(attacker);
    }   

    //https://stackoverflow.com/questions/11678693/all-cases-covered-bresenhams-line-algorithm
    public List<Vector2Int> line(int x, int y, int x2, int y2)
    {
        List<Vector2Int> line = new List<Vector2Int>();
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            line.Add(new Vector2Int(x, y));
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
        //Remove the first, origin, square
        line.RemoveAt(0);
        return line;
    }

    private bool single_attack(rpg_character attacker, Vector2Int target)
    {
        target_squares.Clear();
        target_squares.Add(target);

        return activate_effects(attacker);
    }

    private bool area_attack(rpg_character attacker, Vector2Int target)
    {
        target_squares.Clear();

        for(int x = target.x - stats.get_stat_value("Radius"); x < target.x + stats.get_stat_value("Radius"); x++)
        {
            for(int y = target.y - stats.get_stat_value("Radius"); y < target.y + stats.get_stat_value("Radius"); y++)
            {
                target_squares.Add(new Vector2Int(x, y));
            }
        }

        return activate_effects(attacker);
    }
}