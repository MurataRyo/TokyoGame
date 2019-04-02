using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    XBox xbox;
    [SerializeField] bool FanLight;

    void Start()
    {
        xbox = Utility.GetTaskObject().GetComponent<XBox>();
    }

    // 向きを変える
    public void RotateLaunch()
    {
        if(FanLight)
            transform.Rotate(0f, Input.GetAxisRaw((XBox.AxisStr.RightJoyRight).ToString()) * -1f, 0f);
        else
            transform.Rotate(0f, 0f, Input.GetAxisRaw((XBox.AxisStr.RightJoyRight).ToString()) * 1f);
    }
}

