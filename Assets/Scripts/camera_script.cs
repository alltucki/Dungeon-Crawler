using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handle camera movement
public class camera_script : MonoBehaviour {

    private GameObject player;
    private bool offset;
    private float time_since_input;

    public float damp_time = 0.15f;
    private Vector3 vel = Vector3.zero;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        move_camera();
        time_since_input += Time.deltaTime;

        if (!offset && player != null)
        {
            //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

            Vector3 point = GetComponent<Camera>().WorldToViewportPoint(player.transform.position);
            Vector3 delta = player.transform.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(.5f, .5f, point.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref vel, damp_time);
        }
    }


    //Move the camera around
    public void move_camera()
    {
        float vertical = Input.GetAxis("Camera Vertical");
        float horizontal = Input.GetAxis("Camera Horizontal");
        if(horizontal == 0f && vertical == 0f)
        {
            time_since_input = 1f;
        }
        if(time_since_input > .25f)
        {
            if(vertical != 0f || horizontal != 0f)
            {
                transform.position += new Vector3(horizontal, vertical);
                offset = true;
                time_since_input = 0f;
            }
        }
        //Reset the camera position
        if(Input.GetAxis("Jump") != 0f)
        {
            //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            offset = false;
        }
    }
}
