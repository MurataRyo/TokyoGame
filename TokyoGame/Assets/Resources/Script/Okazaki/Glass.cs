using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    GameObject Player;
    new BoxCollider2D collider2D;
    PlayerMove playerMove;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMove = Player.GetComponent<PlayerMove>();
        collider2D = GetComponent<BoxCollider2D>();
    }
    
    void Update()
    {
        if(playerMove.playerState == PlayerMove.PlayerState.Light)
        {
            collider2D.isTrigger = true;
        }
        else
        {
            collider2D.isTrigger = false;
        }
    }
}
