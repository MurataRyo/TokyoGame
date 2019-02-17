using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float speed;                                // 移動速度
    float jumpPower;                            // ジャンプ力
    private const float WALK_SPEED = 3f;        // 歩行速度
    private const float RUN_SPEED = 4f;         // 走行速度
    private const float JUMP_HEIGHT = 2.5f;     // ジャンプの頂点
    private const float GRAVITY_SIZE = 9.81f;   // 重力の強さ
    bool runFlag = false;                       // 走るかどうか
    bool isGround = false;                      // 接地しているかどうか
    bool isRightWall = false;                   // 右の壁に触れているかどうか
    bool isLeftWall = false;                    // 左の壁に触れているかどうか
    bool lightFlag = false;                     // 光に当たっているかどうか（現在未使用）
    bool jumpFlag = false;                      // ジャンプするかどうか
    const float DISTANCE = 0.1f;

    new Rigidbody2D rigidbody;
    new BoxCollider2D collider2D;

    void Start()
    {
        jumpPower = Mathf.Pow(JUMP_HEIGHT * 2 * GRAVITY_SIZE, 0.5f); // ジャンプ力の計算
        rigidbody = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<BoxCollider2D>();
    }
    
    void Update()
    {
        Vector2 velocity = rigidbody.velocity;

        // 右に移動
        if(Input.GetKey(KeyCode.D) && !isRightWall)
        {
            if(runFlag)
            {
                speed = RUN_SPEED;
            }
            else
            {
                speed = WALK_SPEED;
            }
        }
        // 左に移動
        else if (Input.GetKey(KeyCode.A) && !isLeftWall)
        {
            if (runFlag)
            {
                speed = -RUN_SPEED;
            }
            else
            {
                speed = -WALK_SPEED;
            }
        }
        else
        {
            speed = 0f;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            runFlag = true;
        }
        else
        {
            runFlag = false;
        }

        // ジャンプする
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            jumpFlag = true;
        }

        if(jumpFlag && !isGround)
        {
            jumpFlag = false;
        }

        /*重力関係---------------------------------------------------*/
        if (isGround && !jumpFlag)
        {
            velocity.y = 0f;
        }
        else if(jumpFlag)
        {
            velocity.y = jumpPower;
        }
        else
        {
            velocity.y -= GRAVITY_SIZE * Time.deltaTime;
        }

        if(Input.GetKeyUp(KeyCode.Space) && velocity.y > 0f)
        {
            velocity /= 3f;
        }
        /*----------------------------------------------------------*/

        rigidbody.velocity = new Vector2(speed, velocity.y);
    }

    // 接地・接触判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGround = false;
        isRightWall = false;
        isLeftWall = false;

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if(collision.contacts[i].normal.y > 0.5f)
            {
                isGround = true;
            }
            if(collision.contacts[i].normal.x > 0.5f)
            {
                isLeftWall = true;
            }
            if(collision.contacts[i].normal.x < -0.5f)
            {
                isRightWall = true;
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
