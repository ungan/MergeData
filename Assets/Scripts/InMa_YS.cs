using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InMa_YS : MonoBehaviour
{
    public Text skilltxt;
    bool isClick;
    public int[] inventory = new int[22];
    public int size;
    public GameObject[] aix = new GameObject[20];
    public int aixL;
    public Sprite[] items = new Sprite[72];
    double pushTime;
    public GameObject ItemsP;
    public Transform ItemP;
    int mix1, mix2, mix3, con;
    public GameObject m1, m2, m3, c;


    void viewItems()
    {
        for(int i=0;i<aixL;i++)
        {
            aix[i].transform.GetChild(0).GetComponent<Image>().sprite = items[inventory[i]];
            aix[i].GetComponent<Item_YS>().Desc(inventory[i]);
        }
    }
    void refresh()
    {
        for(int i=0;i<size;i++)
        {
            if (inventory[i] == 0)
            {
                inventory[i] = inventory[i + 1];
                inventory[i + 1] = 0;
            }
        }
    }
    void del(int x)
    {
        inventory[x] = 0;
        size--;
        refresh();
    }
    // Start is called before the first frame update
    void Start()
    {
        skilltxt.text = " ";
        isClick = false;
        aixL = 10;
        size = 0;
        for(int i=0;i<aixL;i++)
        {
            aix[i] = Instantiate(ItemsP);
            aix[i].transform.SetParent(ItemP); //원래문장: aix[i].transform.SetParent = ItemP;
            aix[i].transform.localPosition = new Vector3(-50 + (i%5) * 25, 37.5f - (25 * (int)(i / 5)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnButtonDown(int x)
    {
        isClick = true;
        pushTime = 0;
    }
    public void OnButtonUp(int x)
    {
        isClick = false;
        if (pushTime < 0.5f)
        {
            if (mix1 == 0)
            {
                mix1 = inventory[x];
                m1.GetComponent<Image>().sprite = items[inventory[x]];
                del(x);
            }
            else if (mix2 == 0)
            {
                mix2 = inventory[x];
                m2.GetComponent<Image>().sprite = items[inventory[x]];
                del(x);
            }
            else if (mix3 == 0)
            {
                mix3 = inventory[x];
                m3.GetComponent<Image>().sprite = items[inventory[x]];
                del(x);
            }
            else
            {
                inventory[size++] = mix1;
                inventory[size++] = mix2;
                inventory[size++] = mix3;
                mix1 = inventory[x];
                mix2 = mix3 = 0;
                m1.GetComponent<Image>().sprite = items[inventory[x]];
                m2.GetComponent<Image>().sprite = null;
                m3.GetComponent<Image>().sprite = null;
            }
        }
    }

    public void ClickAction(int x)
    {

    }

    public void SkillAct(int x)
    {
        switch (x)
        {
            case 1:
                skilltxt.text = "이동 소모 감소 2";
                break;
            case 2:
                skilltxt.text = "도토리 소모 감소 40%";
                break;
            case 3:
                skilltxt.text = "클라이밍 거리 증가 100%";
                break;
            case 4:
                skilltxt.text = "적 탐지 가능";
                break;
            case 5:
                skilltxt.text = "시야 증가 2";
                break;
            case 6:
                skilltxt.text = "아이템 등장률 식별";
                break;
            case 7:
                skilltxt.text = "제작 소모량 감소 60%";
                break;
            case 8:
                skilltxt.text = "제작 시간 단축 70%";
                break;
            case 9:
                skilltxt.text = "아이템 영구 사용";
                break;
        }
    }
}
