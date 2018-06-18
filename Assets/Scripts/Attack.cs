using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum attack_shape
{
    single, line, square
}

//DEPRECIATED
//Use attack_entity
[CreateAssetMenu(menuName = "Attack", fileName = "Attack.asset")]
public class Attack : ScriptableObject {
    /*
    public string atk_name;
    public int range, radius, damage, pierce;
    public int range_mod, radius_mod, damage_mod, pierce_mod, slide_mod, push_mod;
    public int slide, push;
    public Stat_Interface stats;
    public bool ok_no_target, dash;
    public Sprite icon;
    public attack_shape shape;
    public GameObject projectile, effect;
    public Attack replacement_attack;

    private void OnEnable()
    {
        stats.init_attack();
        stats.set_stat("Damage", damage);
    }

    public int get_range()
    {
        return range + range_mod;
    }

    public int get_base_range()
    {
        return range;
    }

    public int get_radius()
    {
        return radius + radius_mod;
    }

    public int get_base_radius()
    {
        return radius;
    }

    public int get_damage()
    {
        return stats.get_stat_value("Damage");
    }

    public int get_base_damage()
    {
        return damage;
    }

    public int get_pierce()
    {
        return pierce + pierce_mod;
    }

    public int get_base_pierce()
    {
        return pierce;
    }

    public int get_slide()
    {
        return slide + slide_mod;
    }

    public int get_base_slide()
    {
        return slide;
    }

    public int get_push()
    {
        return push + push_mod;
    }

    public int get_base_push()
    {
        return push;
    }

    public void reset_mods()
    {
        range_mod = 0;
        radius_mod = 0;
        damage_mod = 0;
        pierce_mod = 0;
        slide_mod = 0;
        push_mod = 0;
    }

    public void make_attack(GameObject attacker, GameObject target)
    {
        if (effect != null)
        {
            GameObject new_effect = GameObject.Instantiate(effect);
            new_effect.transform.position = target.transform.position + new Vector3(0, 0f, 0f);
            new_effect.GetComponent<effects_script>().parent = attacker;
            attacker.GetComponent<rpg_character>().set_locked("Locked " + attacker.name + " to allow effect to play");
        }

        if(dash)
        {
            Vector3 adjacent = attacker.transform.position - target.transform.position;
            adjacent = adjacent.normalized;
            adjacent = target.transform.position + adjacent;
            adjacent = new Vector3(Mathf.Round(adjacent.x), Mathf.Round(adjacent.y), 0);

            
            GameObject afterimage = GameObject.Instantiate(util_ref.sprite_prefab);
            afterimage.transform.position = attacker.transform.position;
            afterimage.GetComponent<SpriteRenderer>().sprite = attacker.GetComponent<SpriteRenderer>().sprite;
            afterimage.AddComponent<fade_effect>();
            afterimage.GetComponent<fade_effect>().speed = .75f;


            //Debug.Log("Adjacent square is: " + adjacent.ToString());
            attacker.transform.position = adjacent;

            util_ref.v_manager.refresh_fog();
        }

        if(target.tag == "Player")
        {
            attack_data new_data = new attack_data();
            new_data.attacker = attacker.GetComponent<enemy_script>();
            new_data.target = target.GetComponent<player_script>();
            //new_data.attack_ref = this;
            target.GetComponent<player_script>().lose_health(get_damage(), false, new_data);
        }
        else if(target.tag == "Enemy")
        {
            attack_data new_data = new attack_data();
            new_data.attacker = attacker.GetComponent<player_script>();
            new_data.target = target.GetComponent<enemy_script>();
            //new_data.attack_ref = this;
            //attacker.GetComponent<player_script>().pre_attack_check(new_data);
            //Debug.Log("Called take damage on " + target.name);
            //Debug.Log("Push is " + push.ToString() + " on attack " + atk_name);
            //Debug.Log("Damaged " + target.name + " for " + damage.ToString());
            target.GetComponent<enemy_script>().take_damage(get_damage(), new_data);
            if(push != 0)
            {
                Vector2Int push_square = get_furthest(attacker, target);
                //Debug.Log("Attempted to push enemy to " + push_square.ToString());
                //target.GetComponent<enemy_script>().move(push_square.x, push_square.y);
                target.GetComponent<enemy_script>().disable_hb();
                RaycastHit2D hit = Physics2D.Linecast(target.transform.position + new Vector3(.5f, .5f), push_square);
                if (hit.collider == null)
                {
                    target.GetComponent<enemy_script>().move_to(push_square.x, push_square.y);
                    Debug.DrawLine(target.transform.position, new Vector3(push_square.x, push_square.y), Color.magenta, 2f);
                }
                else {
                    //Take damage based on distance to collider
                    float distance = Mathf.Abs(Vector3.Distance(target.transform.position, hit.point));
                    target.GetComponent<enemy_script>().take_damage(Mathf.CeilToInt(distance), new_data);
                    target.GetComponent<enemy_script>().move_to((int)hit.point.x, (int)hit.point.y);
                    Debug.DrawLine(target.transform.position, hit.point, Color.red, 2f);
                }
                target.GetComponent<enemy_script>().enable_hb();
            }
            //attacker.GetComponent<player_script>().post_attack_check(new_data);
        }
    }

    //Returns the relative distance furthest away from the attacker position
    public Vector2Int get_furthest(GameObject attacker, GameObject target)
    {
        Vector3 offset = new Vector3(.5f, .5f);
        Vector3 push_dir = (attacker.transform.position + offset) - (target.transform.position + offset);
        push_dir = push_dir.normalized;
        push_dir *= get_push();
        Vector3 end_point = (target.transform.position + offset) - push_dir;

        return new Vector2Int((int)end_point.x, (int)end_point.y);
    }

    public bool do_attack(GameObject attacker, Vector3 target)
    {
        switch(shape)
        {
            case attack_shape.single:
                return point_attack(attacker, target);
            case attack_shape.line:
                return linecast_attack(attacker, target);
            default:
                return false;
        }
    }

    private bool point_attack(GameObject attacker, Vector3 target)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target, get_radius());

        Debug.DrawLine(attacker.transform.position, target, Color.red, 5f);

        if(colliders.Length == 0)
        {
            if(ok_no_target && projectile != null)
            {
                GameObject new_projectile = GameObject.Instantiate(projectile);
                new_projectile.transform.position = attacker.transform.position + new Vector3(.5f, .5f, 0f);
                new_projectile.GetComponent<projectile_script>().target = target;
                new_projectile.GetComponent<projectile_script>().generate_midpoint(attacker.transform.position, target);
                if (attacker.tag == "Enemy")
                {
                    attacker.GetComponent<enemy_script>().set_unlocked("Unlocked without attack");
                }
                else if (attacker.tag == "Player")
                {
                    attacker.GetComponent<player_script>().set_unlocked("Unlocked without attack");
                }
            }
            return false;
        }

        bool made_attack = false;
        int pierce_remain = get_pierce();
        for (int i = 0; i < colliders.Length; i++)
        {
            //Debug.Log(line[i].transform.gameObject.tag);
            //Debug.Log("Pierce: " + pierce.ToString());
            bool opposite_tag = false;
            string attacker_tag, target_tag;
            attacker_tag = attacker.tag;
            target_tag = colliders[i].transform.gameObject.tag;
            if (attacker_tag == "Player" && target_tag == "Enemy")
            {
                opposite_tag = true;
            }
            if (attacker_tag == "Enemy" && target_tag == "Player")
            {
                opposite_tag = true;
            }


            if (opposite_tag && pierce_remain > -1)
            {
                pierce_remain--;
                Debug.Log("Made attack on " + colliders[i].transform.gameObject.name);
                make_attack(attacker, colliders[i].transform.gameObject);

                if (projectile != null)
                {
                    GameObject new_projectile = GameObject.Instantiate(projectile);
                    new_projectile.transform.position = attacker.transform.position + new Vector3(.5f, .5f, 0f);
                    new_projectile.GetComponent<projectile_script>().target = target;
                    new_projectile.GetComponent<projectile_script>().generate_midpoint(attacker.transform.position, target);
                }

                made_attack = true;
            }  
        }
        if (!made_attack && ok_no_target)
        {
            made_attack = true;

            if (projectile != null)
            {
                GameObject new_projectile = GameObject.Instantiate(projectile);
                new_projectile.transform.position = attacker.transform.position + new Vector3(.5f, .5f, 0f);
                new_projectile.GetComponent<projectile_script>().target = target;
                new_projectile.GetComponent<projectile_script>().generate_midpoint(attacker.transform.position, target);
                if (attacker.tag == "Enemy")
                {
                    attacker.GetComponent<enemy_script>().set_unlocked("Unlocked without attack");
                }
                else if (attacker.tag == "Player")
                {
                    attacker.GetComponent<player_script>().set_unlocked("Unlocked without attack");
                }
            }

            GameObject new_effect = GameObject.Instantiate(effect);
            new_effect.transform.position = attacker.transform.position;
            new_effect.GetComponent<effects_script>().parent = attacker;
        }
        return made_attack;
    }

    private bool linecast_attack(GameObject attacker, Vector3 target)
    {
        Ray attack_ray = new Ray(attacker.transform.position + new Vector3(.5f, .5f, 0), target - attacker.transform.position);
        RaycastHit2D[] line = Physics2D.RaycastAll(attack_ray.origin, attack_ray.direction, range);

        Debug.DrawLine(attack_ray.origin, target, Color.cyan, 2f);

        //Debug.Log("Hit " + line.Length + " targets");
        //Debug.Log(line.ToString());
        if(line.Length == 0)
        {
            if(ok_no_target && projectile != null)
            {
                GameObject new_projectile = GameObject.Instantiate(projectile);
                new_projectile.transform.position = attacker.transform.position + new Vector3(.5f, .5f, 0f);
                new_projectile.GetComponent<projectile_script>().target = target;
                new_projectile.GetComponent<projectile_script>().generate_midpoint(attacker.transform.position, target);
                attacker.GetComponent<enemy_script>().set_unlocked("Unlocked without attack");
            }
            return false;
        }
        bool made_attack = false;
        int pierce_remain = get_pierce();
        for(int i = 0; i < line.Length; i++)
        {
            //Debug.Log(line[i].transform.gameObject.tag);
            //Debug.Log("Pierce: " + pierce.ToString());
            bool opposite_tag = false;
            string attacker_tag, target_tag;
            attacker_tag = attacker.tag;
            target_tag = line[i].transform.gameObject.tag;
            if(attacker_tag == "Player" && target_tag == "Enemy")
            {
                opposite_tag = true;
            }
            if(attacker_tag == "Enemy" && target_tag == "Player")
            {
                opposite_tag = true;
            }

            if (opposite_tag && pierce_remain > -1)
            {
                pierce_remain--;
                //Debug.Log("Made attack on " + line[i].transform.gameObject.name);
                make_attack(attacker, line[i].transform.gameObject);

                if(projectile != null)
                {
                    GameObject new_projectile = GameObject.Instantiate(projectile);
                    new_projectile.transform.position = attacker.transform.position + new Vector3(.5f, .5f, 0f);
                    new_projectile.GetComponent<projectile_script>().target = target;
                    new_projectile.GetComponent<projectile_script>().generate_midpoint(attacker.transform.position, target);
                }

                made_attack = true;
            }
            else if(line[i].transform.gameObject.tag == "Wall")
            {
                pierce_remain = -1;
            }
        }
        if(!made_attack && ok_no_target)
        {
            made_attack = true;

            if (projectile != null)
            {
                GameObject new_projectile = GameObject.Instantiate(projectile);
                new_projectile.transform.position = attacker.transform.position + new Vector3(.5f, .5f, 0f);
                new_projectile.GetComponent<projectile_script>().target = target;
                new_projectile.GetComponent<projectile_script>().generate_midpoint(attacker.transform.position, target);
                attacker.GetComponent<enemy_script>().set_unlocked("Unlocked without attack");
            }

            GameObject new_effect = GameObject.Instantiate(effect);
            new_effect.transform.position = attacker.transform.position;
            new_effect.GetComponent<effects_script>().parent = attacker;
        }
        return made_attack;
    }
    */
}
