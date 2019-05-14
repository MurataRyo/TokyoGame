using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    GameObject Player;
    PlayerMove playerMove;
    Vector3 position = Vector3.zero;
    Vector2 startPosition = Vector2.zero;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMove = Player.GetComponent<PlayerMove>();

        if (playerMove.startPosition.x <= 9f)
            startPosition.x = 9f;

        else
            startPosition.x = playerMove.startPosition.x;

        startPosition.y = playerMove.startPosition.y + 2f;
        position = new Vector3(startPosition.x, startPosition.y, -10f);
    }

    void Update()
    {
        if(playerMove.playerState != PlayerMove.PlayerState.Start)
        {
            if (Player.transform.position.y > playerMove.deathHeight + 2f)
            {
                if (Player.transform.position.x <= 9f)
                    position.x = 9f;

                else
                    position.x = Player.transform.position.x;
            }

            if (Player.transform.position.y <= playerMove.deathHeight + 7f)
                position.y = playerMove.deathHeight + 9f;

            else
            {
                if (position.y >= Player.transform.position.y + 2f)
                    position.y = Player.transform.position.y + 2f;

                else if (position.y <= Player.transform.position.y)
                    position.y = Player.transform.position.y;
            }
        }

        transform.position = position;
    }
}
