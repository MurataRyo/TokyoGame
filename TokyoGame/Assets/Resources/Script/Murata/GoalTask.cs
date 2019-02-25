using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTask : MonoBehaviour
{
    [HideInInspector]public List<List<Vec2Class>> rayVartex;  //レイの頂点
    // Start is called before the first frame update
    void Start()
    {
        rayVartex = new List<List<Vec2Class>>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(rayVartex[0].Count);

            Debug.Log(rayVartex.Count);
        }
    }

    //レイの追加
    public void AddRayVartex(List<Vec2Class> vertex)
    {
        rayVartex.Add(vertex);
    }

    //レイの削除
    public void RemoveRayVartex(List<Vec2Class> vertex)
    {
        rayVartex.Remove(vertex);
    }
}
