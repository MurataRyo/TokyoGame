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
        if(Player.transform.position.y > playerMove.deathHeight + 5f)
        {
            position.x = Player.transform.position.x;
        }

        if (Player.transform.position.y <= playerMove.deathHeight + 10f && Player.transform.position.y > playerMove.deathHeight + 5f)
        {
            position.y = playerMove.deathHeight + 12f;
        }
        else if(Player.transform.position.y > playerMove.deathHeight + 5f)
        {
            position.y = Player.transform.position.y + 2f;
        }

        position.z = -10f;

        transform.position = position;
    }
}
