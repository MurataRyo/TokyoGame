using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchControl : MonoBehaviour
{
    GameObject launchBase;  // 光源
    Launcher launcher;      // 光源のスクリプト

    // 取得
    void Start()
    {
        launchBase = transform.root.gameObject;
        launcher = launchBase.GetComponent<Launcher>();
    }

    // 自分が選択されたら向きを変えられるようにする
    public void Select()
    {
        launcher.RotateLaunch();
    }
}
