using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchHit : MonoBehaviour
{
    GameObject Player;      // 自機
    PlayerMove playerMove;  // 自機のスクリプト
    private List<GameObject> m_hitObjects = new List<GameObject>();
    [HideInInspector] public GameObject target;      // 選択されているオブジェクト
    int select = 0;         // 選択している番号
    XBoxController controller;

    // 取得
    void Start()
    {
        Player = transform.parent.gameObject;
        playerMove = Player.GetComponent<PlayerMove>();
        controller = Utility.GetTaskObject().GetComponent<XBoxController>();
    }
    
    void Update()
    {
        // 自機の近くに光源があり、かつ接地しているとき
        if (playerMove.isGround && m_hitObjects.Count > 0)
        {
            // 光源を操作する状態に移行
            if (playerMove.playerState == PlayerMove.PlayerState.Default && controller.ControlButton())
            {
                if (playerMove.launchControl)
                    playerMove.launchControl = false;

                else
                    playerMove.launchControl = true;
            }

            // 光源を操作する状態のとき
            if (playerMove.launchControl)
            {
                /*動かす光源の選択-------------------*/
                if (controller.LaunchSelectRight())
                    select++;

                if (controller.LaunchSelectLeft())
                    select--;

                /*----------------------------------*/
            }
            if (select > m_hitObjects.Count - 1)
                select = 0;

            if (select < 0)
                select = m_hitObjects.Count - 1;

            target = m_hitObjects[select];                  // 選択されているオブジェクトの取り出し

            if (playerMove.launchControl)
                target.GetComponent<LaunchControl>().Select();  // 選択した光源の向きを変えられるようにする
        }
        else    // それ以外のとき
        {
            if (playerMove.launchControl)
                playerMove.launchControl = false;

            select = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Launch")
            return;

        m_hitObjects.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Launch")
            return;

        m_hitObjects.Remove(collision.gameObject);
    }
}
