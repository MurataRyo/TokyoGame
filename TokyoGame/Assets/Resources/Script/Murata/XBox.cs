using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class XBox : MonoBehaviour
{
    //通常ボタン
    public enum Str
    {
        LB,
        RB,
        Select,
        Start,
        A,
        B,
        X,
        Y,
    }

    //Axisボタン
    public enum AxisStr
    {
        LeftJoyUp,
        LeftJoyRight,
        LeftButtonUp,
        LeftButtonRight,
        RightJoyUp,
        RightJoyRight,
        BackTrigger,
    }

    public class Axis
    {
        public bool flag = false;
        public bool flagLog = false;
        public bool axisFlag = false;
        public bool axisFlagLog = false;
        public string name;
    }
    public Axis[] axis = new Axis[Enum.GetValues(typeof(AxisStr)).Length];

    void Awake()
    {
        for (int i = 0; i < axis.Length; i++)
        {
            axis[i] = new Axis();
            axis[i].name = ((AxisStr)Enum.ToObject(typeof(AxisStr), i)).ToString();
        }
    }


    void Update()
    {
        for (int i = 0; i < axis.Length; i++)
        {
            axis[i].axisFlagLog = axis[i].axisFlag;
            axis[i].axisFlag = Input.GetAxisRaw(axis[i].name) > 0f;

            axis[i].flagLog = axis[i].flag;
            axis[i].flag = Input.GetAxisRaw(axis[i].name) != 0f;
        }
    }

    //通常ボタンの当たり判定
    public bool ButtonDown(Str button)
    {
        return Input.GetButtonDown(button.ToString());
    }
    public bool ButtonUp(Str button)
    {
        return Input.GetButtonUp(button.ToString());
    }
    public bool Button(Str button)
    {
        return Input.GetButton(button.ToString());
    }
    public bool ButtonDown(Str[] button)
    {
        for(int i = 0;i < button.Length;i++)
        {
            if (ButtonDown(button[i]))
                return true;
        }
        return false;
    }
    public bool ButtonUp(Str[] button)
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (ButtonUp(button[i]))
                return true;
        }
        return false;
    }
    public bool Button(Str[] button)
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (Button(button[i]))
                return true;
        }
        return false;
    }

    //Axisボタンの判定
    public bool ButtonDown(AxisStr button, bool Plus)
    {
        return axis[(int)button].flag && 
               !axis[(int)button].flagLog &&
               Plus == axis[(int)button].axisFlag;
    }
    public bool ButtonUp(AxisStr button, bool Plus)
    {
        return !axis[(int)button].flag && axis[(int)button].axisFlagLog ||
            axis[(int)button].axisFlagLog && Plus != axis[(int)button].axisFlag && axis[(int)button].axisFlagLog == Plus;
    }
    public bool Button(AxisStr button, bool Plus)
    {
        return axis[(int)button].flag && axis[(int)button].axisFlag == Plus;
    }

    public bool ButtonDown(AxisStr[] button, bool[] Plus)
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (ButtonDown(button[i],Plus[i]))
                return true;
        }
        return false;
    }
    public bool ButtonUp(AxisStr[] button, bool[] Plus)
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (ButtonUp(button[i], Plus[i]))
                return true;
        }
        return false;
    }
    public bool Button(AxisStr[] button, bool[] Plus)
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (Button(button[i], Plus[i]))
                return true;
        }
        return false;
    }

    public float AxisGet(AxisStr button)
    {
        return Input.GetAxisRaw(axis[(int)button].name);
    }
}
