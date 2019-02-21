using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTask : MonoBehaviour
{
    [HideInInspector]public List<Vector2[]> rayVartex;  //レイの頂点
    // Start is called before the first frame update
    void Start()
    {
        rayVartex = new List<Vector2[]>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //レイの追加
    public void AddRayVartex(ref Vector2[] vertex)
    {
        rayVartex.Add(vertex);
    }

    //レイの削除
    public void RemoveRayVartex(ref Vector2[] vertex)
    {
        rayVartex.Remove(vertex);
    }
}
