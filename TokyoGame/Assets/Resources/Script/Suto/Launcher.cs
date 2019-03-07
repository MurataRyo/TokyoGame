using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [HideInInspector] public bool controlFlag = false;
    GameObject Player;
    PlayerMove playerMove;
    XBox xbox;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMove = Player.GetComponent<PlayerMove>();
        xbox = GetComponent<XBox>();
    }
    
    void Update()
    {
        if (controlFlag && playerMove.launchControl)
        {
            RotateLaunch();
        }
    }

    // 向きを変える
    void RotateLaunch()
    {
        transform.eulerAngles -= new Vector3(0f, 0f, Input.GetAxisRaw((XBox.AxisStr.RightJoyRight).ToString()) * 1f);
        //if (Input.GetKey(KeyCode.X))
        //{
        //    transform.Rotate(0f, 0f, 1f);
        //}
        //if (Input.GetKey(KeyCode.X))
        //{
        //    transform.Rotate(0f, 0f, -1f);
        //}
    }
}

