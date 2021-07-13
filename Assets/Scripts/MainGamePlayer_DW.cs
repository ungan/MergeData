using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class MainGamePlayer_DW : MonoBehaviour
{
    private Vector2 velocity;   //플레이어설정
    private Vector3 direction;  //이동방향 계산
    private bool hasMoved;      //움직임 검사

    public Tilemap landTilemap; //땅 타일맵
    public Tilemap fogTilemap;  //안개 타일맵

    private string objName;     //오브젝트 감지했을 때 오브젝트 이름반환
    private GameObject beforeTile, afterTile;   //이동할 타일 오브젝트
    Vector3 MosuePosition;      //터치했을때 그 위치 저장하는 것
    public int vision = 2;      //안개효과 범위

    [SerializeField] private int movementSpeed;    //이동속도

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())    //왼쪽 마우스 입력 받으면
        {
            MosuePosition = Input.mousePosition;    //마우스 좌표 받고
            MosuePosition = Camera.main.ScreenToWorldPoint(MosuePosition);  //전역좌표로 변환
            MosuePosition.z = 0f;   //2d라 z축 좌표는 필요없음
            touchTomove();          //플레이어 움직이는 함수 실행
        }
    }

    //안개효과
    private void UpdateFog()
    {
        Vector3Int currentPlayerPos = fogTilemap.WorldToCell(transform.position);  //cell position converts to world position
        Debug.Log(currentPlayerPos);
        for (int i = -vision; i <= vision; i++)
        {
            for (int j = -vision; j <= vision; j++)
            {
                if (i == 1 && (j == -1 || j == 1)) continue;
                fogTilemap.SetTile(currentPlayerPos + new Vector3Int(i, j, 0), null);
            }
        }
    }

    //플레이어 움직이는 함수
    private void touchTomove()
    {
        RaycastHit2D hit = Physics2D.Raycast(MosuePosition, transform.forward, 15f);    //레이케스트를 2d 좌표로 변환

        if (hit)    objName = hit.transform.gameObject.name;    //레이케스트에 맞았다면 오브젝트 이름 저장
        else        Debug.Log("none");                          //일단은 아무행동도 안했음. ui 감지에 이용할 듯
        
        if (objName == "MainGamePlayer") {                      //맞은 오브젝트가 플레이어라면
            if (GameObject.Find("HexPreFab(Clone)") != null)    //타일 오브젝트가 있는지
                GameObject.Find("MainGamePlayerBox").GetComponent<HexPrefabDrawer_DW>().DestoryHexTile();  //타일 만드는 함수에서 타일없애기
            else                                                //맞은 오브젝트가 없다면
                GameObject.Find("MainGamePlayerBox").GetComponent<HexPrefabDrawer_DW>().HexGen();          //타일 만드는 함수에서 타일만들기
        }
        else if (objName == "HexPreFab(Clone)") {   //맞은 오브젝트가 이동하는 타일이라면
            afterTile = hit.transform.gameObject;   //이동할 좌표를 받은 오브젝트

            if (Input.GetMouseButton(0)) {          //터치 입력을 받았다면
                if(beforeTile==null) {              //이전에 타일 정보를 받은 적이 없다면
                    afterTile.GetComponent<SpriteRenderer>().material.color = Color.blue;
                }
                else if(afterTile != beforeTile) {  //이전에 받은 타일정보와 현재 타일정보가 다르다면
                    beforeTile.GetComponent<SpriteRenderer>().material.color = Color.white; 
                    afterTile.GetComponent<SpriteRenderer>().material.color = Color.blue;
                }
                else {                              //이전에 받은 타일정보와 현재 타일정보가 같다면 이동함
                    transform.position = hit.transform.gameObject.GetComponent<Transform>().transform.position; //타일 선택됐으니 이동
                    GameObject.Find("MainGamePlayerBox").GetComponent<HexPrefabDrawer_DW>().DestoryHexTile();      //이동한 후에 타일 지워내기
                }
            }
            beforeTile = hit.transform.gameObject;  //현재 타일정보를 과거의 타일정보로 바꾸기 위해 저장함
        }
        objName = "";   //모든 작업이 끝났으면 오브젝트 이름 지워서 같은 행동 반복하지 않게 하기
    }

}