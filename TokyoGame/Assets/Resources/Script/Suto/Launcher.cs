using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    void Update()
    {
        RotateLaunch();
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

