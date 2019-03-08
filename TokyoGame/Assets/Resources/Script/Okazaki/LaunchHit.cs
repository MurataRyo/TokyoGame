using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchHit : MonoBehaviour
{
    new BoxCollider2D collider2D;

    void Start()
    {
        collider2D = gameObject.GetComponent<BoxCollider2D>();
    }
    
    void Update()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(transform.position, collider2D.size, 0f);  // 当たり判定の取得

        for(int i = 0; i < hit.Length; i++)
        {

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Launch")
        {
            return;
        }
        LaunchControl launchControl = GetComponent<LaunchControl>();
        Debug.Log(collision.gameObject);
    }
}
