using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [HideInInspector] public bool controlFlag = false;
    GameObject Player;
    PlayerMove playerMove;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMove = Player.GetComponent<PlayerMove>();
    }
    
    void Update()
    {
        if (controlFlag && playerMove.launchControl)
        {
            RotateLaunch();
        }
    }

    // キーで向きを変える
    void RotateLaunch()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(0f, 0f, 1f);
        }
        if (Input.GetKey(KeyCode.X))
        {
            transform.Rotate(0f, 0f, -1f);
        }
    }
}

