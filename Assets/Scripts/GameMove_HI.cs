using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GameMove_HI : MonoBehaviour
{
    // 게임오브젝트 목록
    public GameObject mouse_down;   // 마우스 시작점 오브젝트
    public GameObject mouse_up;     // 마우스 끝점 오브젝트
    public Rigidbody2D ridgid;
    public GameObject it;           // 아이템 게임오브젝트를 불러옴
    FixedJoint2D fixjoint;          // 

    // 변수 목록
    public bool rsight;         //좌우 방향체크
    public bool isjump;         //점프 체크 true 점프 아닐경우 false
    public int isglidng = 0;        //몇번쨰 글라이딩 체크
    public float speed_up;      // 아이템 먹었을때 올라가는 스피드
    public float walking_speed; // 캐릭터 걸을떄 스피드
    public float running_speed; // 캐릭터 달릴때 스피드
    public float speed;         // 캐릭터 스피드
    public bool swipe = false;         // 스와이프 체크
    public bool stop = true;    // 캐릭터 정지
    public bool touch = false;  // mouse_down true, mouse_up true; 시간값 계산이용
    public bool short_touch = false;    // 짧은 터치 긴터치인 멈춤입력의 경우 false 맞는경우 true
    public bool use_item = false;       // 아이템 사용시 true
    bool isRope = false;                // 로프에 탔을때 true 아닐때 false
    bool tree_climbing = false;         // 나무에 올라탔을때 true 값 아닐떄 false
    bool Running = false;        // 달릴떄 true;
    bool gliding_up = false;    // 상승비행중 true 아닐떄 false
    bool height = false;        // true면 위로 스와이프 false면 아래로 스와이프
    bool climbing_up = false;   // true면 나무위로 올라감
    bool climbing_rope = true; // 만약 true면 로프를 탈수있음 로프 쿨타임을 위한 변수

    public float gliding2_power = 5;
    public float jump_power = 500;
    public float jump_power_rope = 5;
    public int jump_count = 2;
    float r_count = 0;            // 로프시간
    float r_max_count = 1;        // 다시 로프를 타기위한 쿨타임
    float t_count = 0;              // 터치 눌렀을떄 시간
    float max_count = 1;            // 시간판정 이시간보다 크면 true
    float distance;             // mouse_down mouse_up 거리저장
    float tx;                   // 거리 계산
    float ty;                   // 거리 계산용
    float radian;               // tan값 이용 범위 조절용
    float t_cos;
    float t_sin;
    float j_radian;             // 점프 각도 계산
    public float angle = 30;         // 각도 조절
    bool width;         // true 우측 false 좌측

    private Vector3 mousePos;   // 마우스 위치값

    // Start is called before the first frame update
    void Start()
    {
        radian = angle * Mathf.PI / 180; //라디안값
        mouse_down.SetActive(true);
        mouse_up.SetActive(true);
        fixjoint = GetComponent<FixedJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("stop"+stop);
        Debug.Log("isgliding : " + isglidng);
        input_check();       // 입력체크
        mouse_position(); //마우스 입력값 받아줌
        if (touch)
        {
            //Debug.Log("touch true");
            time_check();
            if (max_count > t_count)                // max>t인경우 1초이내 즉 멈춤입력이 아니므로 short_touch true
            {
                short_touch = true;
            }
            else if (max_count < t_count)            // 멈춤 입력이므로 short_touch false
            {
                short_touch = false;
            }
        }
        else if (rsight && !stop && isglidng == 0)     // 우측이동 
        {
            Debug.Log("우측이동");
            player_rmove();
        }
        else if (!rsight && !stop && isglidng == 0)   // 좌측이동
        {
            Debug.Log("좌측이동");
            player_lmove();
        }
        if (climbing_up && !stop)
        {
            player_climbing();
        }
        if (tree_climbing && stop)
        {
            Debug.Log("멈춰!");
            ridgid.velocity = new Vector2(0, 0);
        }
        if (!climbing_rope)
        {
            rope_cooltime();
        }
        switch (isglidng)
        {
            case 0:                                     // 글라이딩 해제 상태
                ridgid.gravityScale = 1;
                break;
            case 1:
                gliding_direction();                    // 글라이딩 1
                break;
            case 2:                                     // 글라이딩 1 이후 case 2상태가됨
                break;
            case 3:
                gliding2();                             // 글라이딩 2 case 2 에서 case 3이가능
                break;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "item")
        {
            //speedup();
            //it.SetActive(false);
            //speed = speed + fast;
        }
        else if (collision.gameObject.tag == "Rope" && !isRope && climbing_rope)
        {
            Rigidbody2D rig = collision.gameObject.GetComponent<Rigidbody2D>();
            fixjoint.enabled = true;
            fixjoint.connectedBody = rig;
            isRope = true;
            stop = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "tree")
        {
            isjump = false;
            jump_count = 2;
            isglidng = 0;

        }
        if (collision.gameObject.tag == "tree")
        {
            tree_climbing = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "tree")
        {
            tree_climbing = false;
        }
    }

    void player_jump()
    {
        isjump = true;
        jump_count--;
        ridgid.velocity = new Vector2(ridgid.velocity.x, jump_power);
    }

    void player_rmove()
    {
        //Debug.Log("A");
        ridgid.AddForce(Vector3.right * speed * Time.deltaTime);

    }

    void player_lmove()
    {
        //Debug.Log("D");
        ridgid.AddForce(Vector3.left * speed * Time.deltaTime);
    }

    void player_gliding()
    {
        ridgid.velocity = new Vector2(ridgid.velocity.x, 0);
    }

    void player_climbing()
    {
        ridgid.velocity = new Vector2(0, 3);
    }
    void input_check()      // 어떤게 입력이되었는가 touch,swipe 구분
    {

        if (Input.GetMouseButtonDown(0))
        {
            //mouse_down.SetActive(true);
            mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z)
            );                                                      // down되었을떄 위치값 받아와서 mouse_down 오브젝트 그위치로 이동
            mouse_down.transform.position = mousePos;
            //Debug.Log("mouse_down");
            touch = true;                                           // mouse_down touch true 손가락이 닿았을떄 touch true
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("mouse_up");
            //mouse_up.SetActive(true);
            mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z)
            );                                                  // up되었을때 위치값 받아와서 mouse_up 오브젝트 그위치로 이동
            mouse_up.transform.position = mousePos;
            mouse_position();                                   // tx,ty 마우스 up down 위치값 저장
            touch = false;                                      // mouse_up touch false
            distancecheck();                                    // up되어있을때 distancecheck를 이용해서 swipe체크를 해주어야함
            Debug.Log("swipe : " + swipe);                                                    // 그이유는 체크를 항상해줄경우 1 down 1 up 2 down 2 up이 존재할떄
                                                                                              // 1-1끼리 swipe체크를 하여야하지만 항상 해주어야할경우
                                                                                              // 1 up 2down 끼리 체크할경우가 있어 이렇게함
            if (!swipe && short_touch && jump_count > 0 && isglidng == 0)    // !swipe인 경우 이경우는 일반 touch임
            {
                Debug.Log("jump함수 호출");
                player_jump();
                // 점프함수를 호출해줄것
            }
            else if (!swipe && short_touch && isglidng == 2)     // 추가적인 비행
            {
                isglidng = 3;
                ridgid.gravityScale = 1;
                ridgid.velocity = new Vector2(ridgid.velocity.x, gliding2_power);
                //ridgid.AddForce(Vector3.up * gliding2_power);
            }

            if (swipe && !isjump && !isRope)           // swipe이면서 jump상태 아닐경우 swipe 방향 체크
            {
                touch_direction();
            }
            else if (swipe && isjump && isglidng != 2 && !isRope && isglidng !=3)        // swipe 이면서 jump상태일경우 이미 터치로다음동작으로 넘어간경우 다시넘어올수없음
            {
                Debug.Log("점프중 스와이프");
                isglidng = 1;
            }

            else if (isRope && swipe)
            {
                Debug.Log("rope위에서 점프");
                fixjoint.enabled = false;
                isRope = false;
                climbing_rope = false;
                ropejump_direction();
            }
        }
        //Debug.Log("swipe : " + swipe);



        //Debug.Log("swipe" + swipe + " short_touch" + short_touch);
        short_touch = false;    // 이전값이 true일떄 false로 바꿔주지 않으면 계속 점프함
        //mouse_up.SetActive(false);
        //mouse_down.SetActive(false);


    }

    void distancecheck()        // 스와이프인지 아닌지 체크
    {
        //Debug.Log("distancecheck");
        distance = Vector3.Distance(mouse_down.transform.position, mouse_up.transform.position);    // mouse_down,mouse_up 두가지의 게임오브젝트를
                                                                                                    // 불러와서 거리를 측정 거리가 1이하인경우에는 
                                                                                                    // 두 오브젝트가 닿거나 겹친경우 touch로 판정
                                                                                                    // 아닌경우에는 swipe로 판정
                                                                                                    //Debug.Log("distance : "+ distance);
        if (distance <= 1)
        {
            swipe = false;
        }
        else if (distance > 1)
        {
            swipe = true;
        }
    }

    void touch_direction()                      // touch 방향을 판정 좌,우
    {
        if (tx < 0)
        {
            tx = -1 * tx;
            width = false;
        }
        else if (tx >= 0)
        {
            width = true;
        }
        if (ty < 0)
        {
            ty = -1 * ty;

        }
        else if (ty >= 0)
        {
            height = true;
        }
        //Debug.Log("tx : "+ tx + "ty : " +ty );
        if (tx > ty)
        {
            //Debug.Log("tx>ty"+tx/ty + "radian" + radian);
            //Debug.Log("touch width" + width);
            if (width)
            {
                if ((ty / tx) < radian)
                {
                    //Debug.Log("좌측으로 이동");
                    //Debug.Log("좌측 rsight : " + rsight + "stop : " + stop);
                    if (rsight && !stop)      // 만약 이전에 오른쪽이동이 활성화된상태이며 동시에 stop상태가 아닌 true 상태라면 질주활성화
                    {
                        //Debug.Log("우측으로 질주");
                        running();          // 달리는 함수 speed = running_speed; speed 값 교체
                    }
                    else// 우측 이동 함수 부를것
                    {
                        //Debug.Log("우측으로 걷기");
                        walking();          // 걷는 함수 speed값이 걷는 값으로 설정
                        rsight = true;      // 좌측방향이동
                        stop = false;       // stop인경우 stop 해제

                    }
                }
            }
            if (!width)
            {
                if ((ty / tx) < radian)
                {
                    //Debug.Log("우측으로 이동");
                    //Debug.Log("우측 rsight : " + rsight + "stop : " + stop);
                    if (!rsight && !stop)
                    {
                        //Debug.Log("좌측으로 질주");
                        running();          // 달리는 함수 speed = running_speed; speed 값 교체
                    }
                    else    // 좌측 이동 함수 부를것
                    {
                        //Debug.Log("좌측으로 걷기");
                        walking();          // 걷는 함수 speed값이 걷는 값으로 설정
                        rsight = false;     // 우측 방향이동
                        stop = false;       // stop인경우 stop 해제
                    }


                }
            }
        }

        else if (tx <= ty)       // swipe 세로 입력
        {
            Debug.Log("세로입력");
            if (height)          // 위로 입력일때
            {
                if ((tx / ty) < radian)
                {
                    if (tree_climbing)
                    {
                        climbing_up = true;
                    }
                    Debug.Log("위쪽으로 슬라이딩");
                }
            }
        }

    }

    void gliding_direction()
    {

        if (tx < 0)
        {
            tx = -1 * tx;
            width = false;
        }
        else if (tx >= 0)
        {
            width = true;
        }
        if (ty < 0)
        {
            ty = -1 * ty;
        }
        //Debug.Log("tx : "+ tx + "ty : " +ty );
        if (tx > ty)
        {
            //Debug.Log("tx>ty"+tx/ty + "radian" + radian);
            if (width)
            {
                if ((ty / tx) < radian)
                {
                    Debug.Log("오른쪽으로 글라이딩");
                    if (ridgid.velocity.x < 0)
                    {
                        ridgid.velocity = new Vector2(ridgid.velocity.x * -1, 0);
                    }
                    ridgid.velocity = new Vector2(ridgid.velocity.x, 0);
                    ridgid.gravityScale = 0;
                    isglidng = 2;
                    stop = true;
                }
            }
            if (!width)
            {
                if ((ty / tx) < radian)
                {
                    Debug.Log("왼쪽으로 글라이딩");
                    if (ridgid.velocity.x > 0)
                    {
                        ridgid.velocity = new Vector2(ridgid.velocity.x * -1, 0);
                    }
                    ridgid.velocity = new Vector2(ridgid.velocity.x, 0);
                    ridgid.gravityScale = 0;
                    isglidng = 2;
                    stop = true;
                }
            }
        }

    }

    void ropejump_direction()
    {
        //Debug.Log("touch_direction");
        //Debug.Log("ty : " + ty);
        if (tx < 0)
        {
            tx = -1 * tx;
            width = false;
        }
        else if (tx >= 0)
        {
            width = true;
        }
        if (ty < 0)
        {
            ty = -1 * ty;
            height = false;
        }
        else if (ty >= 0)
        {
            height = true;
        }
        //Debug.Log("width : " + width + "height : " + height + "tx : " + tx + "ty : " + ty);

        //Debug.Log("tx : "+ tx + "ty : " +ty );
        if (tx > ty)                    // 가로에대한 입력이 들어왔다/
        {

            //Debug.Log("tx>ty"+tx/ty + "radian" + radian);
            if (width)      // 오른쪽 <
            {
                if (height)                 // 오른쪽 가로부분 1사분면
                {
                    Debug.Log("1");
                    t_cos = tx / math.sqrt(tx * tx + ty * ty);
                    t_sin = ty / math.sqrt(tx * tx + ty * ty);
                    ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }
                else if (!height)           // 오른쪽 가로부분 4사분면 
                {
                    Debug.Log("2");
                    //t_cos = tx / math.sqrt(tx * tx + ty * ty);
                    //t_sin = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos  * jump_power_rope * -1);
                }
            }
            if (!width)     // 왼쪽 >
            {
                if (height)
                {
                    Debug.Log("6");
                    //t_cos = tx / math.sqrt(tx * tx + ty * ty);
                    //t_sin = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * -1 * jump_power_rope, t_cos * jump_power_rope);
                }
                else if (!height)
                {
                    Debug.Log("5");
                    //t_cos = tx / math.sqrt(tx * tx + ty * ty);
                    //t_sin = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * -1 * jump_power_rope, t_cos * -1 * jump_power_rope);
                }
            }
        }

        else if (tx <= ty)       // swipe 세로 입력
        {
            //Debug.Log("세로입력");
            if (height)          // 위로 입력일때
            {
                if (width)       // 위쪽의 1사분면
                {
                    Debug.Log("8");
                    t_sin = tx / math.sqrt(tx * tx + ty * ty);
                    t_cos = ty / math.sqrt(tx * tx + ty * ty);
                    ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }
                else if (!width)     // 위쪽의 2사분면
                {
                    Debug.Log("7");
                    //t_sin = tx / math.sqrt(tx * tx + ty * ty);
                    //t_cos = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }

            }
            else if (!height)
            {
                if (width)       // 아래쪽의 4사사분면
                {
                    Debug.Log("3");
                    //t_sin = tx / math.sqrt(tx * tx + ty * ty);
                    //t_cos = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }
                else if (!width)     // 아래쪽의 3사분면
                {
                    Debug.Log("4");
                    //t_sin = tx / math.sqrt(tx * tx + ty * ty);
                    //t_cos = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }
            }
        }
    }
    void time_check()
    {
        //Debug.Log("time check");
        t_count += Time.deltaTime;
        //Debug.Log("t_count : " + t_count);
        if (max_count < t_count)
        {

            Debug.Log("stop!");
            stop = true;
            game_stop();
            t_count = 0;
        }

    }

    void game_stop()                //게임을 멈췄을시에 이전입력값이 swipe on 일경우 
    {
        swipe = false;
    }

    void running()
    {
        speed = running_speed;
    }

    void walking()
    {
        speed = walking_speed;
    }

    void gliding2()
    {
        if (ridgid.velocity.y > 0)
        {
            gliding_up = true;
        }
        else if (ridgid.velocity.y <= 0)
        {
            gliding_up = false;
            ridgid.velocity = new Vector2(ridgid.velocity.x, 0);
        }
    }

    void rope_cooltime()
    {
        r_count += Time.deltaTime;
        if (r_max_count < r_count)
        {
            climbing_rope = true;
            r_count = 0;
        }
    }
    void mouse_position()
    {
        tx = mouse_up.transform.position.x - mouse_down.transform.position.x;       // mouse_down 위치값 저장
        ty = mouse_up.transform.position.y - mouse_down.transform.position.y;       // mouse_up 위치값 저장
    }


}

