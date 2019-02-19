using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class test : MonoBehaviour
{
    public int reflectCount = 5;
    public float maxStep = 200;

    //[HideInInspector] LineRenderer line; //線
    //[HideInInspector] float counter; // 伸びるカウント
    //[HideInInspector] float dist; //距離
    //[HideInInspector] GameObject startobj; //スタートオブジェクト
    //[HideInInspector] GameObject goalobj; //目標オブジェクト
    //[HideInInspector] float drawSpeed = 6f; //出る速さ

    void Start()
    {
        //startobj = GameObject.FindGameObjectWithTag("Launch");
        //goalobj = GameObject.FindGameObjectWithTag("Mirror");
        //line = GetComponent<LineRenderer>();
        //line.SetPosition(0, startobj.transform.position);

        //// 距離
        //dist = Vector3.Distance(startobj.transform.position, goalobj.transform.position);
    }

    void Update()
    {
        ////カウントが距離よりも小さいとき
        //if (counter < dist)
        //{
        //    //この速さで出る
        //    counter += 0.5f / drawSpeed;

        //    //どこまで伸びているのか補完
        //    float x = Mathf.Lerp(0, dist, counter);

        //    //目標とスタートの座標を入れる
        //    Vector3 pointA = startobj.transform.position;
        //    Vector3 pointB = goalobj.transform.position;

        //    //今どこまで伸びているのか計算
        //    Vector3 pointLine = x * Vector3.Normalize(pointB - pointA) + pointA;

        //    line.SetPosition(1, pointLine);
        //}
    }

    void OnDrawGizmos()
    {
        DrawReflect(transform.position + transform.right * 0.75f, transform.right, reflectCount);
    }

    void DrawReflect(Vector3 position, Vector3 direction, int reflections)
    {
        if (reflections == 0)
        {
            return;
        }
        Vector3 startPos = position;

        Ray2D ray = new Ray2D(position, direction);
        RaycastHit2D hit = Physics2D.Raycast(position,direction);
        if(hit.collider != null)
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
            if(hit.collider.tag == "Mirror")
            {
                Debug.Log("鏡に当たった");
            }
        }
        else
        {
            position += direction * maxStep;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos, position);

        DrawReflect(position, direction, reflections - 1);
    }
}
