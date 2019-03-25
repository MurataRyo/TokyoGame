using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    GameObject Player;
    PlayerMove playerMove;
    Vector3 position = Vector3.zero;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMove = Player.GetComponent<PlayerMove>();
    }

    void Update()
    {
        if(Player.transform.position.y > playerMove.deathHeight + 2f)
        {
            position.x = Player.transform.position.x;
        }

        if (Player.transform.position.y <= playerMove.deathHeight + 7f && Player.transform.position.y > playerMove.deathHeight + 2f)
        {
            position.y = playerMove.deathHeight + 9f;
        }
        else if(Player.transform.position.y > playerMove.deathHeight + 2f)
        {
            position.y = Player.transform.position.y + 2f;
        }

        position.z = -10f;

        transform.position = position;
    }
}
