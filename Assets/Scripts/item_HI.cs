using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item_HI : MonoBehaviour
{
    public GameObject it;
    public float fast;
    //game_move player = new game_move();
    //public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //player.speedup(fast);
        it.SetActive(false);
    }
}
