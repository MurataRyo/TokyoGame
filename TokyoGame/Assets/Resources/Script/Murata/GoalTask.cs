using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTask : MonoBehaviour
{
    [HideInInspector]public List<Vec2Class[]> rayVartex;  //レイの頂点
    // Start is called before the first frame update
    void Start()
    {
        rayVartex = new List<Vec2Class[]>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //レイの追加
    public void AddRayVartex(Vec2Class[] vertex)
    {
        rayVartex.Add(vertex);
    }

    //レイの削除
    public void RemoveRayVartex(Vec2Class[] vertex)
    {
        rayVartex.Remove(vertex);
    }
}
