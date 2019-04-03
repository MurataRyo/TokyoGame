using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightParent : MonoBehaviour
{
    GameObject Player;
    PlayerMove playerMove;
    new CompositeCollider2D collider2D;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMove = Player.GetComponent<PlayerMove>();
        collider2D = gameObject.GetComponent<CompositeCollider2D>();
    }
    
    void FixedUpdate()
    {
        if (playerMove.playerState == PlayerMove.PlayerState.Light && playerMove.lineMove)
        {
            collider2D.isTrigger = false;
        }
        else
        {
            collider2D.isTrigger = true;
        }
    }
}
