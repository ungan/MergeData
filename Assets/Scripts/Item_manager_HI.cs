using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_manager_HI : MonoBehaviour
{
    public GameObject player;
    GameMove_HI m_player;
    
    public bool speedup_use = false;
    bool Speed_up_time_check = false;

    float t_count;           // 시간을 잴 변수
    public float max_count = 5;         // max_count값까지 시간을 잼
    float speedup;
    // Start is called before the first frame update
    void Start()
    {
        m_player = player.GetComponent<GameMove_HI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Speed_up_time_check)
        {
            speed_up_time_check();
        }
    }

    public void speed_up(float _speedup ,float max_count)
    {
        speedup = _speedup;
        speedup_use = true;
        m_player.speed += speedup;
        Speed_up_time_check = true;

    }
    void speed_up_time_check()               // 시간 체크 함수
    {
        //Debug.Log("time check");
        t_count += Time.deltaTime;
        //Debug.Log("t_count : " + t_count);
        if (max_count < t_count)
        {
           
            speedup_use = false;
            t_count = 0;
            m_player.speed -= speedup;
            Speed_up_time_check = false;
        }

    }
    
}
