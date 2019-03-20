using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    GameObject Player;
    PlayerMove playerMove;
    CompositeCollider2D collider2D;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMove = Player.GetComponent<PlayerMove>();
        collider2D = gameObject.GetComponent<CompositeCollider2D>();
    }
    
    void Update()
    {
        if (playerMove.playerState == PlayerMove.PlayerState.Light)
        {
            collider2D.isTrigger = false;
        }
        else
        {
            collider2D.isTrigger = true;
        }
    }
}
