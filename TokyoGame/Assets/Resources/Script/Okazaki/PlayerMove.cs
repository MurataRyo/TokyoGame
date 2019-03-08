﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 自機の状態
    public enum PlayerState
    {
        Normal,
        Light,
    }

    [HideInInspector] public PlayerState playerState = PlayerState.Normal;
    GameObject launchHit;
    
    float speed;                                // 移動速度
    float jumpPower;                            // ジャンプ力
    private const float WALK_SPEED = 5f;        // 歩行速度
    private const float RUN_SPEED = 7f;         // 走行速度
    private const float JUMP_HEIGHT = 6f;       // ジャンプの頂点
    private const float GRAVITY_SIZE = 9.81f;   // 重力の強さ
    bool runFlag = false;                       // 走るかどうか
    bool isGround = false;                      // 接地しているかどうか
    bool isRightWall = false;                   // 右の壁に触れているかどうか
    bool isLeftWall = false;                    // 左の壁に触れているかどうか
    bool lightFlag = false;                     // 光と重なっているかどうか
    bool jumpFlag = false;                      // ジャンプするかどうか
    [HideInInspector]
    public bool launchControl = false;          // 光源を操作するかどうか

    Vector2 position = Vector2.zero;

    new Rigidbody2D rigidbody;
    new BoxCollider2D collider2D;
    XBox xbox;

    void Start()
    {
        launchHit = GameObject.FindGameObjectWithTag("LaunchHit");
        jumpPower = Mathf.Pow(JUMP_HEIGHT * 2 * GRAVITY_SIZE, 0.5f); // ジャンプ力の計算
        rigidbody = GetComponent<Rigidbody2D>();
        collider2D = gameObject.GetComponent<BoxCollider2D>();
        xbox = GetComponent<XBox>();
    }

    void Update()
    {
        Collider2D[] playerHit = Physics2D.OverlapBoxAll(transform.position, collider2D.size, 0f);  // 自機の当たり判定の取得
        Vector2 velocity = rigidbody.velocity;

        lightFlag = false;

        // 侵入判定
        for (int i = 0; i < playerHit.Length; i++)
        {
            if (playerHit[i].tag == "Col")
            {
                lightFlag = true;
            }
        }

        // 光の中を出入りする
        if (lightFlag && Input.GetButton(XBox.Str.RB.ToString()) && playerState != PlayerState.Light)
        {
            playerState = PlayerState.Light;
            velocity = new Vector2(0f, 0f);     // 速度をリセット
        }
        else if ((!lightFlag || !Input.GetButton(XBox.Str.RB.ToString())) && playerState != PlayerState.Normal) 
        {
            playerState = PlayerState.Normal;
            velocity = new Vector2(0f, 0f);     // 速度をリセット
        }

        // 速度の変更
        if (runFlag)
        {
            speed = RUN_SPEED;
        }
        else
        {
            speed = WALK_SPEED;
        }

        // 走るかどうか
        if (Input.GetButton(XBox.Str.X.ToString()))
        {
            runFlag = true;
        }
        else
        {
            runFlag = false;
        }
        
        // 自機の移動
        if (playerState == PlayerState.Light) // 光の中に入っている時の移動
        {
            Vector2 vect2 = new Vector2(Input.GetAxisRaw((XBox.AxisStr.LeftJoyRight).ToString()), -Input.GetAxisRaw((XBox.AxisStr.LeftJoyUp).ToString())).normalized;
            velocity = transform.right * vect2.x + transform.up * vect2.y;
            velocity *= speed;
            //transform.position = new Vector3(
            //        Mathf.Clamp(transform.position.x, light.transform.position.x - (light.transform.localScale.x / 2), light.transform.position.x + (light.transform.localScale.x / 2)),
            //        Mathf.Clamp(transform.position.y, light.transform.position.y - (light.transform.localScale.y / 2), light.transform.position.y + (light.transform.localScale.y / 2)),
            //        0f);

            // 光の中から出ないようにする
            //if (!lightFlag)
            //{
            //    transform.position = position;
            //    velocity = new Vector2(0f, 0f);
            //}
            //else
            //{
            //    velocity = transform.right * vect2.x + transform.up * vect2.y;
            //    velocity *= speed;
            //}
            position = transform.position;
            rigidbody.velocity = new Vector2(velocity.x, velocity.y);

        }
        else // それ以外の時の移動
        {
            if(launchControl)
            {
                velocity.x = 0f;
            }
            else
            {
                velocity.x = Input.GetAxisRaw((XBox.AxisStr.LeftJoyRight).ToString()) * speed;

                if ((isRightWall && velocity.x > 0f) || (isLeftWall && velocity.x < 0f))
                {
                    velocity.x = 0f;
                }
            }

            // ジャンプする
            if (Input.GetButtonDown(XBox.Str.A.ToString()))
            {
                if (isGround && !launchControl)
                {
                    jumpFlag = true;
                }
                else if (launchControl)
                {
                    launchControl = false;
                }
            }

            if (jumpFlag && !isGround)
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
            else
            {
                velocity.y -= GRAVITY_SIZE * Time.deltaTime;
            }
            /*----------------------------------------------------------*/

            rigidbody.velocity = new Vector2(velocity.x, velocity.y);
        }
        RayGround();
        //ModeChange();
    }

    // 自機の状態変化
    private void ModeChange()
    {
        if(playerState == PlayerState.Normal)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            collider2D.size = new Vector2(1f, 2f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 0.5f, 1f);
            collider2D.size = new Vector2(0.5f, 1f);
        }
    }

    // 接地判定
    void RayGround()
    {
        isGround = false;
        Ray2D ray = new Ray2D(transform.position, Vector2.down);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(ray.origin, new Vector2(collider2D.size.x, collider2D.size.y), 0f, ray.direction, 0.1f);

        foreach(RaycastHit2D hit in hits)
        {
            if(hit.normal.y > 0.5f && (hit.collider.isTrigger == false))
            {
                isGround = true;
            }
        }
    }

    // 衝突判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isRightWall = false;
        isLeftWall = false;

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.x > 0.5f)
            {
                isLeftWall = true;
                return;
            }
            if (collision.contacts[i].normal.x < -0.5f)
            {
                isRightWall = true;
                return;
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }
}
