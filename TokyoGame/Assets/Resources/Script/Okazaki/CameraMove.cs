using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    GameObject Player;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        transform.position = new Vector3(0f, 2f, -10f) + Player.transform.position;
    }
}
