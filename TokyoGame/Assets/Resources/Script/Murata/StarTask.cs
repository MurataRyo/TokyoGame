using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTask : MonoBehaviour
{
    GameObject player;
    PlayerMove playerMove;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMove>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == GetTag.Player)
        {
            if (playerMove.playerState == PlayerMove.PlayerState.Light)
                Debug.Log("クリア");
        }
    }
}
