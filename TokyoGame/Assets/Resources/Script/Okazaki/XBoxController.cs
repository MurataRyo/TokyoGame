using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XBoxController : MonoBehaviour
{
    XBox xbox;

    void Start()
    {
        xbox = Utility.GetTaskObject().GetComponent<XBox>();
    }

    // 自機の移動に使うボタン
    public float MoveButton()
    {
        return Input.GetAxisRaw((XBox.AxisStr.LeftJoyRight).ToString());
    }

    // 自機が光状態の時の移動に使うボタン
    public Vector2 LightMoveButton()
    {
        return new Vector2(Input.GetAxisRaw((XBox.AxisStr.LeftJoyRight).ToString()), -Input.GetAxisRaw((XBox.AxisStr.LeftJoyUp).ToString())).normalized;
    }

    // 光源操作に使うボタン
    public float LaunchMoveButton()
    {
        return Input.GetAxisRaw((XBox.AxisStr.RightJoyRight).ToString());
    }

    // 操作する光源の選択に使うボタン（右）
    public bool LaunchSelectRight()
    {
        return xbox.ButtonDown(XBox.AxisStr.LeftButtonRight, true);
    }

    // 操作する光源の選択に使うボタン（左）
    public bool LaunchSelectLeft()
    {
        return xbox.ButtonDown(XBox.AxisStr.LeftButtonRight, false);
    }

    // 自機を走らせる時に使うボタン
    public bool RunButton()
    {
        return xbox.Button(XBox.Str.X);
    }

    // 自機のジャンプに使うボタン
    public bool JumpButton()
    {
        return xbox.ButtonDown(XBox.Str.A);
    }

    // 自機の状態の切り替えに使うボタン
    public bool ChangeButton()
    {
        return xbox.Button(XBox.Str.RB);
    }

    // 自機を光源操作モードに移行させる時に使うボタン
    public bool ControlButton()
    {
        return xbox.ButtonDown(XBox.Str.B);
    }
}
