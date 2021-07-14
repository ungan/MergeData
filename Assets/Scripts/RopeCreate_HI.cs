using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCreate_HI : MonoBehaviour
{
    public GameObject ropePrefab;
    public int ropeCnt;
    public Rigidbody2D pointRig;
    FixedJoint2D exjoint;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i < ropeCnt; i++)
        {
            FixedJoint2D currentJoint = Instantiate(ropePrefab, transform).GetComponent<FixedJoint2D>();
            currentJoint.transform.localPosition = new Vector3(0, (i + 1) * -0.5f, 0);
            if (i == 0)
                currentJoint.connectedBody = pointRig;
            else
                currentJoint.connectedBody = exjoint.GetComponent<Rigidbody2D>();
            exjoint = currentJoint;

            if(i == ropeCnt-1)
            {
                currentJoint.GetComponent<Rigidbody2D>().mass = 10;
                currentJoint.GetComponent<SpriteRenderer>().enabled = false;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
