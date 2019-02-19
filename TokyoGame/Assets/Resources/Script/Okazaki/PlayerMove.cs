using System.Collections;
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
    bool lightFlag = false;                     // 光に当たっているかどうか
    bool jumpFlag = false;                      // ジャンプするかどうか

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
        // 光の中を出入りする
        if(lightFlag)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && playerState != PlayerState.Light)
            {
                playerState = PlayerState.Light;
            }
            else if(Input.GetKeyUp(KeyCode.LeftControl) && playerState != PlayerState.Normal)
            {
                playerState = PlayerState.Normal;
            }
        }
        else
        {
            playerState = PlayerState.Normal;
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
        if (Input.GetKey(KeyCode.LeftShift))
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
            Vector2 vect2 = new Vector2(Input.GetAxisRaw(("Horizontal").ToString()), Input.GetAxisRaw(("Vertical").ToString())).normalized;
            Vector2 velocity = transform.right * vect2.x + transform.up * vect2.y;
            velocity *= speed;
            rigidbody.velocity = new Vector2(velocity.x, velocity.y);
        }
        else // それ以外の時の移動
        {
            Vector2 velocity = rigidbody.velocity;
            // 右に移動
            if (Input.GetKey(KeyCode.D) && !isRightWall)
            {
                velocity.x = speed;
            }
            // 左に移動
            else if (Input.GetKey(KeyCode.A) && !isLeftWall)
            {
                velocity.x = -speed;
            }
            else
            {
                velocity.x = 0f;
            }

            // ジャンプする
            if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                jumpFlag = true;
            }

            if (jumpFlag && !isGround)
            {
                jumpFlag = false;
            }

            /*重力関係---------------------------------------------------*/
            if ((isGround && !jumpFlag) || playerState == PlayerState.Light)
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

            if (Input.GetKeyUp(KeyCode.Space) && velocity.y > 0f)
            {
                velocity /= 3f;
            }
            /*----------------------------------------------------------*/

            rigidbody.velocity = new Vector2(velocity.x, velocity.y);
        }

        Debug.Log(lightFlag);
    }

    // 接地・衝突判定
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

    // 侵入判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Launch")
        {
            lightFlag = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Launch")
        {
            lightFlag = false;
        }
    }
}
