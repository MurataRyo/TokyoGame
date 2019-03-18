using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    XBox xbox;

    void Start()
    {
        xbox = Utility.GetTaskObject().GetComponent<XBox>();
    }

    // 向きを変える
    public void RotateLaunch()
    {
        transform.eulerAngles -= new Vector3(0f, 0f, Input.GetAxisRaw((XBox.AxisStr.RightJoyRight).ToString()) * 1f);
    }
}

