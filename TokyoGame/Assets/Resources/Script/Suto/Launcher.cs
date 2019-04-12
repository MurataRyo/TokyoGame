using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    XBoxController controller;
    [SerializeField] bool FanLight;

    void Start()
    {
        controller = Utility.GetTaskObject().GetComponent<XBoxController>();
    }

    // 向きを変える
    public void RotateLaunch()
    {
        if(FanLight)
            transform.Rotate(0f, controller.LaunchMoveButton() * -1f, 0f);
        else
            transform.Rotate(0f, 0f, controller.LaunchMoveButton() * -1f);
    }
}

