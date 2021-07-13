using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexPrefabDrawer_DW : MonoBehaviour
{
    public GameObject hexOrigin, hexContainer, hexChild;  //모체, 자식 프리팹
    public Tilemap landTile;    //기준이 될 타일맵
    public Tilemap ObstacleTile;//장애물이 있는 타일맵

    public float round = 4f;    //갈 수 있는 거리
    private float up = 0.5f;    //타일 사이즈, 전역좌표에 맞게 설정했음
    private Vector3 playerPos;  //플레이어 위치 벡터 저장

    //시작할때 영역 표시해줌
    private void Start()
    {
        HexGen();
    }

    //육각형 타일 만드는 함수
    public void HexGen()
    {
        //플레이어 위치 받아내기
        playerPos = GameObject.Find("MainGamePlayerBox").GetComponent<Transform>().transform.position;
        playerPos.z = 0f;
        //반복문으로 영역표시
        for (float i = -round; i <= round; i += up)
        {   //x축
            for (float j = Mathf.Abs(i) - round; j <= round - Mathf.Abs(i); j += up)
            {  //y축
                Vector3 pos = new Vector3(i, j, 0) + playerPos;    //월드좌표

                if (Mathf.Abs(j) > round * up)          //y축 제한
                    continue;

                if (Mathf.Abs(i) % 1 == 0.5f && Mathf.Abs(j) % 1 == 0.5f)
                {
                    hexChild = (GameObject)Instantiate(hexOrigin, pos, Quaternion.Euler(50, 0, 0));
                    IsTile(hexChild);
                }
                    

                if (Mathf.Abs(i) % 1 == 0 && Mathf.Abs(j) % 1 == 0)
                {
                    hexChild = (GameObject)Instantiate(hexOrigin, pos, Quaternion.Euler(50, 0, 0));
                    IsTile(hexChild);
                }
                    
                //자식으로 생성
                hexChild.transform.parent = hexContainer.transform;
            }
        }
    }

    //육각형 타일 제거 함수
    public void DestoryHexTile()
    {
        var hextiles = new List<GameObject>();
        foreach (Transform child in hexContainer.transform) hextiles.Add(child.gameObject);
        hextiles.ForEach(child => Destroy(child));
    }

    //장애물 타일맵 좌표 받아오는 함수
    public void IsTile(GameObject tileHex)
    {
        Vector3 localpos = tileHex.GetComponent<Transform>().transform.position;    //프리팹 오브젝트의 좌표를 가져옴
        Vector3Int localcell = landTile.LocalToCell(localpos);                      //좌표를 셀좌표로 전환
        if (ObstacleTile.HasTile(localcell))                                        //만약 장애물 타일맵에 타일이 있다면
        {
            Debug.Log("TRUE");
            tileHex.GetComponent<SpriteRenderer>().material.color = Color.red;      //일단 색을 빨강으로 변경
        }
    }

}
