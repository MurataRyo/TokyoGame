﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    // 自機の状態
    public enum PlayerState
    {
        Default,
        Light,
    }

    [HideInInspector] public PlayerState playerState = PlayerState.Default;
    GameObject launchHit;

    [SerializeField] GameObject Model;          // 自機のモデル
    [SerializeField] GameObject LightModel;     // 自機のモデル（光状態）
    float speed;                                // 移動速度
    float jumpPower;                            // ジャンプ力
    float angle;                                // 向き
    private const float WALK_SPEED = 5f;        // 歩行速度
    private const float RUN_SPEED = 10f;        // 走行速度
    private const float LIGHT_SPEED = 15f;      // 光状態の時の速度
    private const float JUMP_HEIGHT = 4f;       // ジャンプの頂点
    private const float GRAVITY_SIZE = 20f;     // 重力の強さ
    bool runFlag = false;                       // 走るかどうか
    bool isGround = false;                      // 接地しているかどうか
    bool lightFlag = false;                     // 光と重なっているかどうか
    bool jumpFlag = false;                      // ジャンプするかどうか
    bool lightJump = false;
    bool search = false;
    bool lineMove = false;
    [HideInInspector]
    public bool launchControl = false;          // 光源を操作するかどうか
    [HideInInspector]
    public bool stopPlayer = false;
    [HideInInspector]
    public float deathHeight;                   // 落下死になる高さ

    Vector2 position = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    new SkinnedMeshRenderer renderer;
    new Rigidbody2D rigidbody;
    BoxCollider2D boxCollider2D;
    BoxCollider2D launchHitCollider;
    CircleCollider2D circleCollider2D;
    XBox xbox;

    void Start()
    {
        launchHit = GameObject.FindGameObjectWithTag("LaunchHit");
        jumpPower = Mathf.Pow(JUMP_HEIGHT * 2 * GRAVITY_SIZE, 0.5f); // ジャンプ力の計算
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        launchHitCollider = launchHit.GetComponent<BoxCollider2D>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        xbox = Utility.GetTaskObject().GetComponent<XBox>();
        renderer = GetComponent<SkinnedMeshRenderer>();
        deathHeight = -7f;
    }

    void Update()
    {
        /*Collider2D[] playerHit = Physics2D.OverlapBoxAll(transform.position, boxCollider2D.size, 0f);*/
        Collider2D[] playerHit = Physics2D.OverlapCircleAll
            (new Vector2(transform.position.x, transform.position.y + (circleCollider2D.offset.y * transform.localScale.y)), 0f, LayerMask.GetMask("Col")); // 自機の当たり判定の取得
        //Collider2D[] lineHit = Physics2D.OverlapCircleAll
        //    (new Vector2(transform.position.x, transform.position.y + (circleCollider2D.offset.y * transform.localScale.y)), 0f, LayerMask.GetMask("Col"));
        Vector2 velocity = rigidbody.velocity;

        lightFlag = false;

        // 侵入判定
        for (int i = 0; i < playerHit.Length; i++)
        {
            if (playerHit[i].tag == "Col" || playerHit[i].tag == "Col2")
            {
                lightFlag = true;
            }
        }
        //for (int i = 0; i < lineHit.Length; i++)
        //{
        //    if (lineHit[i].tag == "Col")
        //    {
        //        lineMove = false;
        //    }
        //}

        // 速度の変更
        if (runFlag)
        {
            speed = RUN_SPEED;
        }
        else if(playerState == PlayerState.Light)
        {
            speed = LIGHT_SPEED;
        }
        else
        {
            speed = WALK_SPEED;
        }

        // 走るかどうか
        if (xbox.Button(XBox.Str.X) && playerState == PlayerState.Default && isGround)
        {
            runFlag = true;
        }
        else
        {
            runFlag = false;
        }

        if(!xbox.Button(XBox.Str.RB) && lightJump)
        {
            lightJump = false;
        }

        // ジャンプする
        if (xbox.ButtonDown(XBox.Str.A))
        {
            if (playerState == PlayerState.Light)
            {
                lightJump = true;
                playerState = PlayerState.Default;
            }
            else
            {
                if (!isGround || launchControl)
                {
                    return;
                }
            }
            jumpFlag = true;
        }
        RayGround();
        //Debug.Log(lineMove);
    }

    void FixedUpdate()
    {
        Vector2 velocity = rigidbody.velocity;
        //Vector2 position = transform.position;
        //float x = circleCollider2D.radius + speed * Time.fixedDeltaTime;
        //Vector2 y = new Vector2(velocity.x, velocity.y).normalized;
        //Vector2 z = x * y + position;
        //Collider2D[] lightSearch = Physics2D.OverlapCircleAll(z, 0f, LayerMask.GetMask("Col"));

        // 光の中を出入りする
        if (lightFlag && xbox.Button(XBox.Str.RB) && playerState != PlayerState.Light && !lightJump)
        {
            playerState = PlayerState.Light;
            velocity = new Vector2(0f, 0f);     // 速度をリセット
        }
        else if ((!lightFlag || !xbox.Button(XBox.Str.RB)) && playerState != PlayerState.Default)
        {
            playerState = PlayerState.Default;
            velocity = new Vector2(0f, 0f);     // 速度をリセット
        }

        // 落下死判定
        if(transform.position.y < deathHeight)
        {
            Utility.GetTaskObject().GetComponent<GameTask>().mode = GameTask.Mode.gameOver;
        }

        // 自機の移動
        if (!stopPlayer)
        {
            if(launchHit.transform.position != new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z) && velocity.x > 0f)
            {
                launchHit.transform.position = new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
                angle = 90f;
            }
            if (launchHit.transform.position != new Vector3(transform.position.x - launchHitCollider.size.x / 2, transform.position.y, transform.position.z) && velocity.x < 0f)
            {
                launchHit.transform.position = new Vector3(transform.position.x - launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
                angle = -90f;
            }

            Model.transform.eulerAngles = new Vector3(0f, angle, 0f);

            if (playerState == PlayerState.Light) // 光の中に入っている時の移動
            {
                Vector2 vect2 = new Vector2(Input.GetAxisRaw((XBox.AxisStr.LeftJoyRight).ToString()), -Input.GetAxisRaw((XBox.AxisStr.LeftJoyUp).ToString())).normalized * speed;
                velocity = transform.right * vect2.x + transform.up * vect2.y;

                rigidbody.velocity = new Vector2(velocity.x, velocity.y);
                Model.SetActive(false);
                LightModel.SetActive(true);
                boxCollider2D.enabled = false;
                circleCollider2D.isTrigger = false;
                search = true;
            }
            else // それ以外の時の移動
            {
                if (launchControl)
                {
                    velocity.x = 0f;
                }
                else
                {
                    velocity.x = Input.GetAxisRaw((XBox.AxisStr.LeftJoyRight).ToString()) * speed;
                }

                if (jumpFlag && velocity.y == jumpPower)
                {
                    jumpFlag = false;
                }

                /*重力関係---------------------------------------------------*/
                if (((isGround && !jumpFlag) || playerState == PlayerState.Light) && velocity.y <= 0f)
                {
                    velocity.y = 0f;
                }
                else if (jumpFlag)
                {
                    velocity.y = jumpPower;
                }
                else if(velocity.y < -15f)
                {
                    velocity.y = -15f;
                }
                else
                {
                    velocity.y -= GRAVITY_SIZE * Time.deltaTime;
                }
                /*----------------------------------------------------------*/

                rigidbody.velocity = new Vector2(velocity.x, velocity.y);
                boxCollider2D.enabled = true;
                Model.SetActive(true);
                LightModel.SetActive(false);
                circleCollider2D.isTrigger = true;
                search = false;
            }
            //for (int i = 0; i < lightSearch.Length; i++)
            //{
            //    if (lightSearch[i].tag == "Col")
            //    {
            //        search = false;
            //    }
            //}
        }
        else
        {
            rigidbody.velocity = new Vector2(0f, 0f);
        }
    }

    void LightSearch()
    {
        Vector2 position = transform.position;
        Vector2 velocity = rigidbody.velocity;
        float x = circleCollider2D.radius + LIGHT_SPEED * Time.fixedDeltaTime;
        Vector2 y = new Vector2(velocity.x, velocity.y).normalized;
        Vector2 z = x * y + position;
        Vector2 direction = position - z;

        RaycastHit2D hit = Physics2D.Raycast(z, direction, x, LayerMask.GetMask("Col"));

        if(Physics2D.Raycast(z, direction, x, LayerMask.GetMask("Col")))
        {
            Debug.Log(hit.point);
            Debug.Log(Quaternion.FromToRotation(direction, hit.normal).eulerAngles);
            transform.position = ClosestPoint(hit.collider, z) - (ClosestPoint(hit.collider, z) - position);
        }
        Debug.DrawRay(z, direction, Color.red);
        Debug.Log(ClosestPoint(hit.collider, z) - position);
    }

    Vector2 ClosestPoint(Collider2D collider, Vector2 point)
    {
        GameObject go = new GameObject();
        go.transform.position = point;
        CircleCollider2D circle = go.AddComponent<CircleCollider2D>();
        circle.radius = 0.01f;
        ColliderDistance2D dis = collider.Distance(circle);
        Destroy(go);
        return dis.pointA;
    }

    // 接地判定
    void RayGround()
    {
        isGround = false;
        Ray2D ray = new Ray2D(new Vector2(transform.position.x, transform.position.y + (boxCollider2D.offset.y * transform.localScale.y)), Vector2.down);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(ray.origin, new Vector2(boxCollider2D.size.x * transform.localScale.x, boxCollider2D.size.y * transform.localScale.y), 0f, ray.direction, 0.1f);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.normal.y > 0.5f && (hit.collider.isTrigger == false))
            {
                isGround = true;
            }
        }
    }
}
