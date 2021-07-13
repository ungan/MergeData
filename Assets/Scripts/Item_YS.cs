using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_YS : MonoBehaviour
{
    public GameObject P;
    public int num;
    GameObject Ma;
    // Start is called before the first frame update
    void Start()
    {
        Ma = GameObject.Find("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Desc(int ItemNum)
    {
        if (ItemNum == 1)
            P.transform.GetChild(0).GetComponent<Text>().text = "엮은 나뭇가지\n가방";
        if (ItemNum == 11)
            P.transform.GetChild(0).GetComponent<Text>().text = "가방";
        if (ItemNum == 21)
            P.transform.GetChild(0).GetComponent<Text>().text = "엮은 풀\n가방";
        if (ItemNum == 31)
            P.transform.GetChild(0).GetComponent<Text>().text = "아이템 보관";
        if (ItemNum == 41)
            P.transform.GetChild(0).GetComponent<Text>().text = "절벽하강\n사다리";
        if (ItemNum == 51)
            P.transform.GetChild(0).GetComponent<Text>().text = "사다리\n강";
        if (ItemNum == 61)
            P.transform.GetChild(0).GetComponent<Text>().text = "절벽등반";
    }

    public void OnButtonDown()
    {
        P.SetActive(true);
        Ma.GetComponent<InMa_YS>().OnButtonDown(num);
    }
    public void OnButtonUp()
    {
        P.SetActive(false);
        Ma.GetComponent<InMa_YS>().OnButtonUp(num);
    }
}
