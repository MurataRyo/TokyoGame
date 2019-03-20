using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    const float DEATH_HEIGHT = -5f;
    GameObject Player;
    Vector3 position = Vector3.zero;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        if(Player.transform.position.y > DEATH_HEIGHT)
        {
            position.x = Player.transform.position.x;
        }

        if (Player.transform.position.y <= DEATH_HEIGHT + 5f && Player.transform.position.y > DEATH_HEIGHT)
        {
            position.y = DEATH_HEIGHT + 7f;
        }
        else if(Player.transform.position.y > DEATH_HEIGHT + 5f)
        {
            position.y = Player.transform.position.y + 2f;
        }

        position.z = -10f;

        transform.position = position;
    }
}
