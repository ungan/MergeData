using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMove_HI : MonoBehaviour
{
    public int jump_count;
    public GameObject it;
    public GameObject player;
    public TouchPosition_HI touch;
    //public GameObject touch;

    public float fast;
    public float speed;
    public float jump_power;
    public float speed_g;
    public float change_speed;
    public float a;

    private float item_basetime=0;
    private float item_starttime = 0;
    private float item_holdingtime=10;
    public Rigidbody2D ridgid;

    bool use_item= false;
    bool r_sight = true;
    bool tree_climbing = false;
    bool isRope = false;

    FixedJoint2D fixjoint;
    // Start is called before the first frame update
    void Start()
    {
        //Rigidbody2D ridgid = GetComponent<Rigidbody2D>();
        //Rigidbody2D ridgid = GetComponent<Rigidbody2D>();

        // Add a force to the Rigidbody.
        fixjoint = GetComponent<FixedJoint2D>();
        //touch = new touch_position();
        
    }



    // Update is called once per frame
    void Update()
    {
       
       
        Debug.Log("move m_code" + touch.m_code);
        item_basetime += Time.deltaTime;
        a = item_basetime - item_starttime;
        //Debug.Log("item_holdingtime" + item_holdingtime);
        //Debug.Log("item_basetime" + item_basetime);
        //Debug.Log("item_starttime" + item_starttime);
        //Debug.Log("item_basetime- item_starttime" + a);
        if(use_item)
        {
            timecheck();
        }
        if (Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow) || (touch.m_code == 1))
        {
            Debug.Log("A");
            ridgid.AddForce(Vector3.left * speed * Time.deltaTime);
            r_sight = true;
            //player.transform.localScale = Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow) || (touch.m_code == 0))
        {
            Debug.Log("D");
            ridgid.AddForce(Vector3.right * speed * Time.deltaTime);
            r_sight = false;
            //player.transform.localScale = Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Space) && jump_count > 0 || jump_count >0 && (touch.j_code == 0))
        {
            jump_count--;
            //ridgid.AddForce(Vector3.up * jump_power);
            ridgid.velocity = new Vector2(ridgid.velocity.x,jump_power);
        }
        
        if (Input.GetKey(KeyCode.G))
        {
            ridgid.velocity = new Vector2(ridgid.velocity.x,speed_g);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            fixjoint.connectedBody = null;
            fixjoint.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "ground" || collision.gameObject.tag == "tree")
        {
            jump_count = 2;

        }
        if (collision.gameObject.tag == "tree")
        {
            tree_climbing = true;
        }
        /*
        if(collision.gameObject.tag == "item")
        {
            speedup(fast);
            it.SetActive(false);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "item")
        {
            speedup(fast);
            it.SetActive(false);
            //speed = speed + fast;
        }
        else if(collision.gameObject.tag == "Rope" && !isRope)
        {
            Rigidbody2D rig = collision.gameObject.GetComponent<Rigidbody2D>();
            fixjoint.enabled = true;
            fixjoint.connectedBody = rig;
            isRope = true;

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "tree")
        {
            tree_climbing = false;
        }
    }


    public void speedup(float s)
    {
        use_item = true;
        change_speed = s;
        item_starttime = item_basetime;
        Debug.Log("a");
        speed += change_speed;
    }

    public void speed_down()
    {
        Debug.Log("speed_down실행");
        use_item = false;
        speed = speed -change_speed;
    }
    public void timecheck()
    {
        if(item_holdingtime< a)
        {
            speed_down();
            Debug.Log("시간완료!");
            //Debug.Log("item_basetime- item_starttime"+a);
        }
    }
}

/*
        if (Input.GetKeyDown(KeyCode.Space) && jump_count > 0 && tree_climbing)
        {
            if(r_sight)
            {
                jump_count--;
                ridgid.velocity = Vector2.one * jump_power;
                Debug.Log("오른쪽 위로 점프");
            }
            else if(!r_sight)
            {
                jump_count--;
                ridgid.velocity = Vector2.left * jump_power;
                ridgid.velocity = Vector2.up * jump_power;
                Debug.Log("왼쪽 위로 점프");
            }
        }
        */