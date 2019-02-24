using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static GameObject GetTask()
    {
        return GameObject.FindGameObjectWithTag(GetTag.Tasks);
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

    public Vec2Class(float x,float y)
    {
        vec2 = new Vector2(x, y);
    }

    public Vec2Class(Vector2 vec2)
    {
        this.vec2 = vec2;
    }
}