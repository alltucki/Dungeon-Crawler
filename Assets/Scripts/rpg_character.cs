using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum character_type
{
    player, enemy
}

//
public class rpg_character : MonoBehaviour {

    //public int health, armor;
    //public int move_actions, attack_actions, shared_actions;
    //public int max_health, max_move, max_attack, max_shared;
    public Stat_Interface stats;
    public bool is_moving;
    public Vector3 target_position;
    public attack_entity queued_attack;
    public BoxCollider2D hitbox;
    public Rigidbody2D rb2d;

    private bool locked;
    public character_type type;
    //public List<Conditional> always_on, on_attack_temp, on_attack_perm;
    public List<temp_effect> stat_cond;

    public GameObject current_target_square, damage_prefab;
    public rpg_character current_target_character;

    // Use this for initialization
    void Start () {
        hitbox = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void start_turn()
    {
        util_ref.events.trigger_event(name + "_start_turn", this);
        stats.set_stat("Move Actions", stats.get_stat_value("Max Move"));
        stats.set_stat("Attack Actions", stats.get_stat_value("Max Attack"));
        stats.set_stat("Shared Actions", stats.get_stat_value("Max Shared"));
        
        check_status();
    }

    public void end_turn()
    {
        util_ref.events.trigger_event(name + "_end_turn", this);
    }

    public void take_damage(attack_data data)
    {
        data.target = this;

        //Log the details of this attack for access by other events
        //Then trigger the events
        util_ref.events.last_attack = data;
        util_ref.events.trigger_event(name + "_start_attacked", this);
        util_ref.events.trigger_event(data.attacker.name + "_start_attack", data.attacker);

        int damage = data.attack_ref.stats.get_stat_value("Damage");
        if(damage <= stats.get_stat_value("Armor"))
        {
            util_ref.events.trigger_event("Block", this);
        }
        damage -= stats.get_stat_value("Armor");
        if (damage < 0) damage = 0;
        stats.get_stat("Health").set_value(stats.get_stat_value("Health") - damage);

        //Spawn a damage number
        if (damage > 0)
        {
            //Debug.Log("Took damage and called damage number with " + damage.ToString());
            GameObject damage_number = GameObject.Instantiate(damage_prefab);
            damage_number.transform.position = transform.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.5f, 1.5f), 0);
            damage_number.GetComponent<damage_number_script>().set_number(damage);
            damage_number.GetComponent<damage_number_script>().set_ttl(1);
        }

        if (stats.get_stat_value("Health") <= 0)
        {
            if(type == character_type.enemy)
            {
                GetComponent<enemy_script>().die();
            }
        }
        util_ref.events.trigger_event(name + "_end_attacked", this);
        util_ref.events.trigger_event(data.attacker.name + "_end_attack", data.attacker);
    }

    //Generic take_damage method to handle damage outside of
    //regular attacks
    public void take_damage(attack_data data, int damage)
    {
        data.target = this;

        //Log the details of this attack for access by other events
        //Then trigger the events
        util_ref.events.last_attack = data;
        util_ref.events.trigger_event(name + "_start_attacked", this);
        util_ref.events.trigger_event(data.attacker.name + "_start_attack", data.attacker);

        stats.get_stat("Health").set_value(stats.get_stat_value("Health") - damage);

        //Spawn a damage number
        if (damage > 0)
        {
            //Debug.Log("Took damage and called damage number with " + damage.ToString());
            GameObject damage_number = GameObject.Instantiate(damage_prefab);
            damage_number.transform.position = transform.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.5f, 1.5f), 0);
            damage_number.GetComponent<damage_number_script>().set_number(damage);
            damage_number.GetComponent<damage_number_script>().set_ttl(1);
        }

        if (stats.get_stat_value("Health") <= 0)
        {
            if (type == character_type.enemy)
            {
                GetComponent<enemy_script>().die();
            }
        }
        util_ref.events.trigger_event(name + "_end_attacked", this);
        util_ref.events.trigger_event(data.attacker.name + "_end_attack", data.attacker);
    }

    public void decrease_attack()
    {
        if (stats.get_stat_value("Attack Actions") > 0)
        {
            stats.get_stat("Attack Actions").dec_value();
        }
        else
        {
            stats.get_stat("Shared Actions").dec_value();
        }
    }

    public void decrease_move()
    {
        if (stats.get_stat_value("Move Actions") > 0)
        {
            stats.get_stat("Move Actions").dec_value();
        }
        else
        {
            stats.get_stat("Shared Actions").dec_value();
        }
    }

    public int get_move_remaining()
    {
        return stats.get_stat_value("Shared Actions") + stats.get_stat_value("Move Actions");
    }

    public int get_attack_remaining()
    {
        return stats.get_stat_value("Shared Actions") + stats.get_stat_value("Attack Actions");
    }

    public int get_remaining_actions()
    {
        return stats.get_stat_value("Move Actions") + stats.get_stat_value("Attack Actions") + stats.get_stat_value("Shared Actions");
    }

    public void check_status()
    {
        for(int i = 0; i < stat_cond.Count; i++)
        {
            stat_cond[i].do_effect(this);
        }
    }

    public void set_locked(string reason)
    {
        //Debug.Log(reason);
        locked = true;
    }

    public void set_unlocked(string reason)
    {
        //Debug.Log(reason);
        locked = false;
    }

    public bool is_locked()
    {
        return locked;
    }

    public void move_relative(int x, int y)
    {
        is_moving = true;
        target_position = transform.position + new Vector3(x, y, 0);
        rb2d.MovePosition(new Vector2(transform.position.x + x, transform.position.y + y));
    }

    public void move_relative(Vector2Int pos)
    {
        is_moving = true;
        target_position = transform.position + new Vector3(pos.x, pos.y, 0);
        rb2d.MovePosition(new Vector2(transform.position.x + pos.x, transform.position.y + pos.y));
    }

    public void move_abs(int x, int y)
    {
        is_moving = true;
        target_position = new Vector3(x, y, 0);
        rb2d.MovePosition(new Vector2(x, y));
    }

    public void move_abs(Vector2Int pos)
    {
        is_moving = true;
        target_position = new Vector3(pos.x, pos.y, 0);
        rb2d.MovePosition(new Vector2(pos.x, pos.y));
    }

    public Vector2Int get_pos()
    {
        if(is_moving)
        {
            return new Vector2Int((int)target_position.x, (int)target_position.y);
        }
        else
        {
            return new Vector2Int((int)transform.position.x, (int)transform.position.y);
        }
    }

    public GameObject get_t_char_object()
    {
        return current_target_character.gameObject;
    }

    public void enable_hb()
    {
        hitbox.enabled = true;
    }

    public void disable_hb()
    {
        hitbox.enabled = false;
    }
}
