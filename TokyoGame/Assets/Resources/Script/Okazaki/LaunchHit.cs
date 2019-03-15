using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchHit : MonoBehaviour
{
    GameObject Player;      // 自機
    PlayerMove playerMove;  // 自機のスクリプト
    private List<GameObject> m_hitObjects = new List<GameObject>();
    GameObject target;      // 選択されているオブジェクト
    int select = 0;         // 選択している番号
    XBox xbox;

    // 取得
    void Start()
    {
        Player = transform.root.gameObject;
        playerMove = Player.GetComponent<PlayerMove>();
        xbox = GetComponent<XBox>();
    }
    
    void Update()
    {
        // 自機の近くに光源があるとき
        if (m_hitObjects.Count > 0)
        {
            // 光源を操作する状態に移行
            if (playerMove.playerState == PlayerMove.PlayerState.Normal && Input.GetButtonDown(XBox.Str.B.ToString()))
            {
                if (playerMove.launchControl)
                {
                    playerMove.launchControl = false;
                }
                else
                {
                    playerMove.launchControl = true;
                }
            }

            // 光源を操作する状態のとき
            if (playerMove.launchControl)
            {
                /*動かす光源の選択-------------------*/
                if (Input.GetKeyDown(KeyCode.L))
                {
                    select++;
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    select--;
                }
                /*----------------------------------*/
            }
            if (select > m_hitObjects.Count - 1)
            {
                select = 0;
            }
            if (select < 0)
            {
                select = m_hitObjects.Count - 1;
            }
            target = m_hitObjects[select];                  // 選択されているオブジェクトの取り出し
            if (playerMove.launchControl)
            {
                target.GetComponent<LaunchControl>().Select();  // 選択した光源の向きを変えられるようにする
            }
        }
        else
        {
            select = 0;
        }
        //Debug.Log(m_hitObjects.Count);
        //Debug.Log(target);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Launch")
        {
            return;
        }
        m_hitObjects.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Launch")
        {
            return;
        }
        m_hitObjects.Remove(collision.gameObject);
    }
}
