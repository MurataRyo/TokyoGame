using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static GameObject GetTaskObject()
    {
        return GameObject.FindGameObjectWithTag(GetTag.Tasks);
    }

    public static int BoolToInt(bool flag)
    {
        return flag ? 1 : -1;
    }
}

public struct GetTag
{
    public const string Block = "Block";
    public const string LightSource = "LightSource";
    public const string Mirror = "Mirror";
    public const string Mirror_Back = "MirrorBack";
    public const string Glass = "Glass";
    public const string Player = "Player";
    public const string Tasks = "Tasks";
    public const string Col = "Col";
}

public class Vec2Class
{
    public Vector2 vec2 { get; set; }

    public Vec2Class()
    {

    }

    public Vec2Class(float x, float y)
    {
        vec2 = new Vector2(x, y);
    }

    public Vec2Class(Vector2 vec2)
    {
        this.vec2 = vec2;
    }
}

public class ChoiceClass
{
    public int choiceNum;   //選択できる数
    public int nowChoice;   //現在選択している数

    public ChoiceClass(int choiceNum)
    {
        this.choiceNum = choiceNum;
        this.nowChoice = 0;
    }

    public ChoiceClass(int choiceNum, int nowChoice)
    {
        this.choiceNum = choiceNum;
        this.nowChoice = nowChoice;
    }

    //選択変更  trueがplusでfalseがマイナス
    public void ChoiceChange(bool flag)
    {
        nowChoice += Utility.BoolToInt(flag);
        if(nowChoice == -1 || nowChoice == choiceNum)
        {
            if(flag)
            {
                nowChoice = 0;
            }
            else
            {
                nowChoice = choiceNum - 1;
            }
        }
    }
}