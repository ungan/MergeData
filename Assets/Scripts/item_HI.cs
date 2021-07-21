using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_HI : MonoBehaviour
{
    public GameObject it;


    public GameObject item_manager;
    Item_manager_HI it_mg;
    public bool item_use = false;
    public float speed_up;          // 올라갈 속도
    public float max_count = 5;         // max_count값까지 시간을 잼

    float t_count;           // 시간을 잴 변수

    //public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        it_mg = item_manager.GetComponent<Item_manager_HI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            it_mg.speed_up(speed_up, max_count);
            it.SetActive(false);
        }

    }

}
