using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    [HideInInspector] LineRenderer line; //線
    [HideInInspector] float counter; // 伸びるカウント
    [HideInInspector] float dist; //距離
    [HideInInspector] GameObject origin; //スタートオブジェクト
    [HideInInspector] GameObject destination; //目標オブジェクト
    [HideInInspector] float drawSpeed = 6f; //出る速さ
    [HideInInspector] EdgeCollider2D edgeCollider2D;
    List<Vector2> lineEdge = new List<Vector2>();

    void Start()
    {
        origin = GameObject.FindGameObjectWithTag("Launch");
        destination = GameObject.FindGameObjectWithTag("Mirror");
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, origin.transform.position);

        // 距離
        dist = Vector3.Distance(origin.transform.position, destination.transform.position);
    }
    
    void Update()
    {
        //カウントが距離よりも小さいとき
        if (counter < dist)
        {
            lineEdge.Add(origin.transform.position);
            //この速さで出る
            counter += 0.5f / drawSpeed;

            //どこまで伸びているのか補完
            float x = Mathf.Lerp(0, dist, counter);

            //目標とスタートの座標を入れる
            Vector3 pointA = origin.transform.position;
            Vector3 pointB = destination.transform.position;

            //今どこまで伸びているのか計算
            Vector3 pointLine = x * Vector3.Normalize(pointB - pointA) + pointA;

            line.SetPosition(1, pointLine);
            edgeCollider2D.points = lineEdge.ToArray();
        }
    }
}
