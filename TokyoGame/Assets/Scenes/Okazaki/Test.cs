using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    new Rigidbody2D rigidbody;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    Vector3 current;
    Vector3 target;
    Vector3 defaultPosition;
    [SerializeField] float moveTime;
    float moveCount;
    Vector3 speed;
    bool reverse = false;

    void Start()
    {
        defaultPosition = transform.position;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        current = defaultPosition + start;
        target = defaultPosition + end;
        speed = (target - current) / moveTime;
    }

    void FixedUpdate()
    {
        moveCount += Time.fixedDeltaTime;

        if(moveCount >= moveTime)
        {
            moveCount = 0f;
            current = new Vector2(transform.position.x, transform.position.y);

            if (!reverse)
            {
                target = defaultPosition + start;
                reverse = true;
            }
            else
            {
                target = defaultPosition + end;
                reverse = false;
            }
            speed = (target - current) / moveTime;
        }

        rigidbody.MovePosition(speed * Time.fixedDeltaTime + transform.position);
        //rigidbody.velocity = new Vector2((target.x - current.x) / moveTime, (target.y - current.y) / moveTime);
    }
}
