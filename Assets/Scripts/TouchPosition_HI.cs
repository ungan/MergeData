using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPosition_HI : MonoBehaviour
{
    public GameObject mouse_down;
    public GameObject mouse_up;

    public int m_code=100;
    public int j_code = 100;

    bool swipe = false;
    float distance;
    float tx;
    float ty;
    private Vector3 mousePos;
    float radian;
    bool width;         // true 우측 false 좌측
    // Use this for initialization
    void Start()
    {
        radian = 30 * Mathf.PI / 180; //라디안값
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(swipe);
        Debug.Log("touch m_code" + m_code);

        
        if (Input.GetMouseButtonDown(0))
        {
            mouse_down.SetActive(true);
            mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z)
            );
            //print(mousePos);
            mouse_down.transform.position = mousePos;
            
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouse_up.SetActive(true);
            mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z)
            );
            //print(mousePos);
            mouse_up.transform.position = mousePos;
            distancecheck();
        }
        if(swipe)
        {
            //Debug.Log("swipe");
            touch_direction();
        }
        else if(!swipe)
        {
            j_code = 0;
        }
        mouse_up.SetActive(false);
        mouse_down.SetActive(false);
        j_code = 100;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "touch_point")
        {
            swipe = false;
        }
    }
    void distancecheck()
    {
        distance = Vector3.Distance(mouse_down.transform.position, mouse_up.transform.position);
        //Debug.Log(distance);
        if (distance <= 1)
        {
            swipe = false;
        }
        else if (distance > 1)
        {
            swipe = true;
        }
    }
    void touch_direction()
    {
        //Debug.Log("touch_direction");
        tx = mouse_down.transform.position.x - mouse_up.transform.position.x;
        ty = mouse_down.transform.position.y - mouse_up.transform.position.y;
        if(tx<0)
        {
            tx = -1 * tx;
            width = true;
        }
        else if(tx >= 0)
        {
            width = false;
        }
        if (ty < 0)
        {
            ty = -1 * ty;
        }
        if(tx>ty)
        {

            //Debug.Log("tx>ty"+tx/ty + "radian" + radian);
            if(width)
            {
                if ((ty / tx) < radian)
                {
                   // Debug.Log("우측으로 이동");
                    m_code = 0;
                }
            }
            if(!width)
            {
                if((ty/ tx ) < radian)
                {
                    //Debug.Log("좌측으로 이동");
                    m_code = 1;
                }
            }
        }
        

    }
}

