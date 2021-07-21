using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GameMove_HI : MonoBehaviour
{
    // ���ӿ�����Ʈ ���
    public GameObject mouse_down;   // ���콺 ������ ������Ʈ
    public GameObject mouse_up;     // ���콺 ���� ������Ʈ
    public Rigidbody2D ridgid;
    public GameObject it;           // ������ ���ӿ�����Ʈ�� �ҷ���
    FixedJoint2D fixjoint;          // 

    // ���� ���
    public bool rsight;         //�¿� ����üũ
    public bool isjump;         //���� üũ true ���� �ƴҰ�� false
    public int isglidng = 0;        //����� �۶��̵� üũ
    public float speed_up;      // ������ �Ծ����� �ö󰡴� ���ǵ�
    public float walking_speed; // ĳ���� ������ ���ǵ�
    public float running_speed; // ĳ���� �޸��� ���ǵ�
    public float speed;         // ĳ���� ���ǵ�
    public bool swipe = false;         // �������� üũ
    public bool stop = true;    // ĳ���� ����
    public bool touch = false;  // mouse_down true, mouse_up true; �ð��� ����̿�
    public bool short_touch = false;    // ª�� ��ġ ����ġ�� �����Է��� ��� false �´°�� true
    public bool use_item = false;       // ������ ���� true
    bool isRope = false;                // ������ ������ true �ƴҶ� false
    bool tree_climbing = false;         // ������ �ö������� true �� �ƴҋ� false
    bool Running = false;        // �޸��� true;
    bool gliding_up = false;    // ��º����� true �ƴҋ� false
    bool height = false;        // true�� ���� �������� false�� �Ʒ��� ��������
    bool climbing_up = false;   // true�� �������� �ö�
    bool climbing_rope = true; // ���� true�� ������ Ż������ ���� ��Ÿ���� ���� ����

    public float gliding2_power = 5;
    public float jump_power = 500;
    public float jump_power_rope = 5;
    public int jump_count = 2;
    float r_count = 0;            // �����ð�
    float r_max_count = 1;        // �ٽ� ������ Ÿ������ ��Ÿ��
    float t_count = 0;              // ��ġ �������� �ð�
    float max_count = 1;            // �ð����� �̽ð����� ũ�� true
    float distance;             // mouse_down mouse_up �Ÿ�����
    float tx;                   // �Ÿ� ���
    float ty;                   // �Ÿ� ����
    float radian;               // tan�� �̿� ���� ������
    float t_cos;
    float t_sin;
    float j_radian;             // ���� ���� ���
    public float angle = 30;         // ���� ����
    bool width;         // true ���� false ����

    private Vector3 mousePos;   // ���콺 ��ġ��

    // Start is called before the first frame update
    void Start()
    {
        radian = angle * Mathf.PI / 180; //���Ȱ�
        mouse_down.SetActive(true);
        mouse_up.SetActive(true);
        fixjoint = GetComponent<FixedJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("stop"+stop);
        Debug.Log("isgliding : " + isglidng);
        input_check();       // �Է�üũ
        mouse_position(); //���콺 �Է°� �޾���
        if (touch)
        {
            //Debug.Log("touch true");
            time_check();
            if (max_count > t_count)                // max>t�ΰ�� 1���̳� �� �����Է��� �ƴϹǷ� short_touch true
            {
                short_touch = true;
            }
            else if (max_count < t_count)            // ���� �Է��̹Ƿ� short_touch false
            {
                short_touch = false;
            }
        }
        else if (rsight && !stop && isglidng == 0)     // �����̵� 
        {
            Debug.Log("�����̵�");
            player_rmove();
        }
        else if (!rsight && !stop && isglidng == 0)   // �����̵�
        {
            Debug.Log("�����̵�");
            player_lmove();
        }
        if (climbing_up && !stop)
        {
            player_climbing();
        }
        if (tree_climbing && stop)
        {
            Debug.Log("����!");
            ridgid.velocity = new Vector2(0, 0);
        }
        if (!climbing_rope)
        {
            rope_cooltime();
        }
        switch (isglidng)
        {
            case 0:                                     // �۶��̵� ���� ����
                ridgid.gravityScale = 1;
                break;
            case 1:
                gliding_direction();                    // �۶��̵� 1
                break;
            case 2:                                     // �۶��̵� 1 ���� case 2���°���
                break;
            case 3:
                gliding2();                             // �۶��̵� 2 case 2 ���� case 3�̰���
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
    void input_check()      // ��� �Է��̵Ǿ��°� touch,swipe ����
    {

        if (Input.GetMouseButtonDown(0))
        {
            //mouse_down.SetActive(true);
            mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z)
            );                                                      // down�Ǿ����� ��ġ�� �޾ƿͼ� mouse_down ������Ʈ ����ġ�� �̵�
            mouse_down.transform.position = mousePos;
            //Debug.Log("mouse_down");
            touch = true;                                           // mouse_down touch true �հ����� ������� touch true
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
            );                                                  // up�Ǿ����� ��ġ�� �޾ƿͼ� mouse_up ������Ʈ ����ġ�� �̵�
            mouse_up.transform.position = mousePos;
            mouse_position();                                   // tx,ty ���콺 up down ��ġ�� ����
            touch = false;                                      // mouse_up touch false
            distancecheck();                                    // up�Ǿ������� distancecheck�� �̿��ؼ� swipeüũ�� ���־����
            Debug.Log("swipe : " + swipe);                                                    // �������� üũ�� �׻����ٰ�� 1 down 1 up 2 down 2 up�� �����ҋ�
                                                                                              // 1-1���� swipeüũ�� �Ͽ��������� �׻� ���־���Ұ��
                                                                                              // 1 up 2down ���� üũ�Ұ�찡 �־� �̷�����
            if (!swipe && short_touch && jump_count > 0 && isglidng == 0)    // !swipe�� ��� �̰��� �Ϲ� touch��
            {
                Debug.Log("jump�Լ� ȣ��");
                player_jump();
                // �����Լ��� ȣ�����ٰ�
            }
            else if (!swipe && short_touch && isglidng == 2)     // �߰����� ����
            {
                isglidng = 3;
                ridgid.gravityScale = 1;
                ridgid.velocity = new Vector2(ridgid.velocity.x, gliding2_power);
                //ridgid.AddForce(Vector3.up * gliding2_power);
            }

            if (swipe && !isjump && !isRope)           // swipe�̸鼭 jump���� �ƴҰ�� swipe ���� üũ
            {
                touch_direction();
            }
            else if (swipe && isjump && isglidng != 2 && !isRope && isglidng !=3)        // swipe �̸鼭 jump�����ϰ�� �̹� ��ġ�δ����������� �Ѿ��� �ٽóѾ�ü�����
            {
                Debug.Log("������ ��������");
                isglidng = 1;
            }

            else if (isRope && swipe)
            {
                Debug.Log("rope������ ����");
                fixjoint.enabled = false;
                isRope = false;
                climbing_rope = false;
                ropejump_direction();
            }
        }
        //Debug.Log("swipe : " + swipe);



        //Debug.Log("swipe" + swipe + " short_touch" + short_touch);
        short_touch = false;    // �������� true�ϋ� false�� �ٲ����� ������ ��� ������
        //mouse_up.SetActive(false);
        //mouse_down.SetActive(false);


    }

    void distancecheck()        // ������������ �ƴ��� üũ
    {
        //Debug.Log("distancecheck");
        distance = Vector3.Distance(mouse_down.transform.position, mouse_up.transform.position);    // mouse_down,mouse_up �ΰ����� ���ӿ�����Ʈ��
                                                                                                    // �ҷ��ͼ� �Ÿ��� ���� �Ÿ��� 1�����ΰ�쿡�� 
                                                                                                    // �� ������Ʈ�� ��ų� ��ģ��� touch�� ����
                                                                                                    // �ƴѰ�쿡�� swipe�� ����
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

    void touch_direction()                      // touch ������ ���� ��,��
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
                    //Debug.Log("�������� �̵�");
                    //Debug.Log("���� rsight : " + rsight + "stop : " + stop);
                    if (rsight && !stop)      // ���� ������ �������̵��� Ȱ��ȭ�Ȼ����̸� ���ÿ� stop���°� �ƴ� true ���¶�� ����Ȱ��ȭ
                    {
                        //Debug.Log("�������� ����");
                        running();          // �޸��� �Լ� speed = running_speed; speed �� ��ü
                    }
                    else// ���� �̵� �Լ� �θ���
                    {
                        //Debug.Log("�������� �ȱ�");
                        walking();          // �ȴ� �Լ� speed���� �ȴ� ������ ����
                        rsight = true;      // ���������̵�
                        stop = false;       // stop�ΰ�� stop ����

                    }
                }
            }
            if (!width)
            {
                if ((ty / tx) < radian)
                {
                    //Debug.Log("�������� �̵�");
                    //Debug.Log("���� rsight : " + rsight + "stop : " + stop);
                    if (!rsight && !stop)
                    {
                        //Debug.Log("�������� ����");
                        running();          // �޸��� �Լ� speed = running_speed; speed �� ��ü
                    }
                    else    // ���� �̵� �Լ� �θ���
                    {
                        //Debug.Log("�������� �ȱ�");
                        walking();          // �ȴ� �Լ� speed���� �ȴ� ������ ����
                        rsight = false;     // ���� �����̵�
                        stop = false;       // stop�ΰ�� stop ����
                    }


                }
            }
        }

        else if (tx <= ty)       // swipe ���� �Է�
        {
            Debug.Log("�����Է�");
            if (height)          // ���� �Է��϶�
            {
                if ((tx / ty) < radian)
                {
                    if (tree_climbing)
                    {
                        climbing_up = true;
                    }
                    Debug.Log("�������� �����̵�");
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
                    Debug.Log("���������� �۶��̵�");
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
                    Debug.Log("�������� �۶��̵�");
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
        if (tx > ty)                    // ���ο����� �Է��� ���Դ�/
        {

            //Debug.Log("tx>ty"+tx/ty + "radian" + radian);
            if (width)      // ������ <
            {
                if (height)                 // ������ ���κκ� 1��и�
                {
                    Debug.Log("1");
                    t_cos = tx / math.sqrt(tx * tx + ty * ty);
                    t_sin = ty / math.sqrt(tx * tx + ty * ty);
                    ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }
                else if (!height)           // ������ ���κκ� 4��и� 
                {
                    Debug.Log("2");
                    //t_cos = tx / math.sqrt(tx * tx + ty * ty);
                    //t_sin = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos  * jump_power_rope * -1);
                }
            }
            if (!width)     // ���� >
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

        else if (tx <= ty)       // swipe ���� �Է�
        {
            //Debug.Log("�����Է�");
            if (height)          // ���� �Է��϶�
            {
                if (width)       // ������ 1��и�
                {
                    Debug.Log("8");
                    t_sin = tx / math.sqrt(tx * tx + ty * ty);
                    t_cos = ty / math.sqrt(tx * tx + ty * ty);
                    ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }
                else if (!width)     // ������ 2��и�
                {
                    Debug.Log("7");
                    //t_sin = tx / math.sqrt(tx * tx + ty * ty);
                    //t_cos = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }

            }
            else if (!height)
            {
                if (width)       // �Ʒ����� 4���и�
                {
                    Debug.Log("3");
                    //t_sin = tx / math.sqrt(tx * tx + ty * ty);
                    //t_cos = ty / math.sqrt(tx * tx + ty * ty);
                    //ridgid.velocity = new Vector2(t_sin * jump_power_rope, t_cos * jump_power_rope);
                }
                else if (!width)     // �Ʒ����� 3��и�
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

    void game_stop()                //������ �������ÿ� �����Է°��� swipe on �ϰ�� 
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
        tx = mouse_up.transform.position.x - mouse_down.transform.position.x;       // mouse_down ��ġ�� ����
        ty = mouse_up.transform.position.y - mouse_down.transform.position.y;       // mouse_up ��ġ�� ����
    }


}

