using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static GameObject GetTask()
    {
        return GameObject.FindGameObjectWithTag("Tasks");
    }

    Vec2Class vec2 = new Vec2Class();

    private void OnAnimatorIK(int layerIndex)
    {
        vec2 = new Vec2Class(1,3);
    }
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