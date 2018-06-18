using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum projectile_type
{
    straight, arc
}

//Controls the movement and eventual destruction of a projectile
public class projectile_script : MonoBehaviour {

    public Vector3 start, target, midpoint;
    public projectile_type type;                //Straight uses velocity; arc uses bezier curves
    public float mod_val, count, speed;
    public GameObject end_effect;
    public Sprite[] shadows;
    private SpriteRenderer shadow_renderer;

	// Use this for initialization
	void Start () {
        shadow_renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        transform.right = target - transform.position;
        if (type == projectile_type.straight)
        {
            GetComponent<Rigidbody2D>().velocity = transform.right * 30f;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Modify the child shadow to be larger as we move closer to the destination
        if (count < 1.0f)
        {
            shadow_renderer.sprite = shadows[Mathf.RoundToInt((5f * count) / 5f)];
            count += Time.deltaTime * speed;
            if (type == projectile_type.arc)
            {
                Vector3 m1 = Vector3.Lerp(start, midpoint, count);
                Vector3 m2 = Vector3.Lerp(midpoint, target, count);
                transform.position = Vector3.Lerp(m1, m2, count);
                transform.right = target - transform.position;
            }
        }
        else
        {
            GameObject end = GameObject.Instantiate(end_effect);
            end.transform.position = transform.position;
            Destroy(gameObject);
        }
	}

    public void generate_midpoint(Vector3 in_start, Vector3 in_end)
    {
        start = in_start;
        target = in_end;

        midpoint = start + (target - start) / 2f + Vector3.up * mod_val;

        count = 0f;
    }
        //Bezier curve reference
        /*
         * point[0] = start
         * point[2] = end

         * point[1] = point[0] + (point[2] - point[0])/2 + Vector3.up * mod_val

         * float count = 0f

        
         * update() {
         *  if(count < 1.0f) {
         *      count += Time.deltaTime;
         *      Vector3 m1 = Vector3.Lerp(point[0], point[1], count);
         *      Vector3 m2 = Vector3.Lerp(point[1], point[2], count);
         *      transform.position = Vector3.Lerp(m1, m2, count);
         */
}
