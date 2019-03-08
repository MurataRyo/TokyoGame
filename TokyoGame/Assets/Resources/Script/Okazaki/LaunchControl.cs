using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchControl : MonoBehaviour
{
    GameObject Player;
    GameObject launchBase;
    PlayerMove playerMove;
    Launcher launcher;
    new CircleCollider2D collider2D;
    XBox xbox;
    public bool selectFlag = false;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        launchBase = transform.root.gameObject;
        playerMove = Player.GetComponent<PlayerMove>();
        launcher = launchBase.GetComponent<Launcher>();
        collider2D = gameObject.GetComponent<CircleCollider2D>();
        xbox = GetComponent<XBox>();
    }
    
    void Update()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, collider2D.radius);  // 当たり判定の取得
        launcher.controlFlag = false;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].tag == "LaunchHit")
            {
                if (selectFlag && Input.GetButtonDown(XBox.Str.B.ToString()))
                {
                    playerMove.launchControl = true;
                }
                launcher.controlFlag = true;
            }
        }
        //Debug.Log(hitFlag);
    }
}
