using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float speed;
    float jumpPower;
    private const float WALK_SPEED = 3f;
    private const float RUN_SPEED = 4f;
    private const float JUMP_HEIGHT = 2.5f;
    private const float GRAVITY_SIZE = 9.81f;
    bool runFlag = false;
    bool isGround = false;
    bool lightFlag = false;
    bool jumpFlag = false;
    const float DISTANCE = 0.1f;

    new Rigidbody2D rigidbody;
    new BoxCollider2D collider2D;

    void Start()
    {
        jumpPower = Mathf.Pow(JUMP_HEIGHT * 2 * GRAVITY_SIZE, 0.5f);
        rigidbody = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<BoxCollider2D>();
    }
    
    void Update()
    {
        Vector2 velocity = rigidbody.velocity;

        if(Input.GetKey(KeyCode.D))
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
        else if (Input.GetKey(KeyCode.A))
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

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            jumpFlag = true;
        }

        if(jumpFlag && !isGround)
        {
            jumpFlag = false;
        }

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

        rigidbody.velocity = new Vector2(speed, velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGround = false;

        for(int i = 0; i < collision.contacts.Length; i++)
        {
            if(collision.contacts[i].normal.y > 0.5f)
            {
                isGround = true;
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
