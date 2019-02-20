using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static GameObject GetTask()
    {
        return GameObject.FindGameObjectWithTag("Tasks");
    }
}
