using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    GameObject Player;
    PlayerMove playerMove;
    Animator animator;

    void Start()
    {
        Player = transform.parent.gameObject;
        playerMove = Player.GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        animator.SetBool("Move", playerMove.move);
        animator.SetBool("IsGround", playerMove.isGround);
        animator.SetBool("Jump", playerMove.jumpFlag);
        animator.SetBool("Control", playerMove.launchControl);
        animator.SetBool("Run", playerMove.runFlag);
        animator.SetFloat("AirTime", playerMove.airTime);
        animator.SetFloat("WaitTime", playerMove.waitTime);
    }
}
