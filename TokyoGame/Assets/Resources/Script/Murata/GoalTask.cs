using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTask : MonoBehaviour
{
    [HideInInspector]public List<BoxCollider2D> rayColliders;
    // Start is called before the first frame update
    void Start()
    {
        rayColliders = new List<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //レイの追加
    public void AddRayColider(BoxCollider2D col)
    {
        rayColliders.Add(col);
    }

    //レイの削除
    public void RemoveRayColider(BoxCollider2D col)
    {
        rayColliders.Remove(col);
    }
}
