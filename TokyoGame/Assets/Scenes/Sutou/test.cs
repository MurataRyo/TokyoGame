using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class test : MonoBehaviour
{
    [HideInInspector] private float maxStep = 200;
    [HideInInspector] public List<Vec2Class> originate;
    GameObject taskObject;
    GoalTask goalTask;
    #region
    //[HideInInspector] LineRenderer line; //線
    //[HideInInspector] float counter; // 伸びるカウント
    //[HideInInspector] float dist; //距離
    //[HideInInspector] GameObject startobj; //スタートオブジェクト
    //[HideInInspector] GameObject goalobj; //目標オブジェクト
    //[HideInInspector] float drawSpeed = 6f; //出る速さ

    void Start()
    {
        taskObject =
            Utility.GetTask();
        goalTask = taskObject.GetComponent<GoalTask>();
        //foreach (Transform chiild in transform)
        //{
        //    if (chiild.gameObject.tag == "Launch")
        //    {
        //        startobj = chiild.gameObject;
        //        break;
        //    }
        //}
        //startobj = GameObject.FindGameObjectWithTag("Launch");
        //goalobj = GameObject.FindGameObjectWithTag("Mirror");
        //line = GetComponent<LineRenderer>();
        //line.SetPosition(0, startobj.transform.position);

        //// 距離
        //dist = Vector3.Distance(startobj.transform.position, goalobj.transform.position);

        originate = new List<Vec2Class>();
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
    #endregion

    void OnDrawGizmos()
    {
        goalTask.RemoveRayVartex(originate);
        originate = new List<Vec2Class>();
        originate.Add(new Vec2Class(transform.position));
        DrawReflect(transform.position + transform.right * 0.75f, transform.right);
        goalTask.AddRayVartex(originate);
    }

    void DrawReflect(Vector2 position, Vector2 direction)
    {
        Vector2 startPos = position;

        RaycastHit2D hit = Physics2D.Raycast(position + direction * 0.5f, direction);
        if (hit.collider != null && hit.collider.tag == "Mirror")
        {
            direction = Vector2.Reflect(direction, hit.normal);
            position = hit.point;
            originate.Add(new Vec2Class(position));
            Debug.Log("鏡に当たった");
            DrawReflect(position, direction);
        }
        else
        {
            position += direction * maxStep;
            originate.Add(new Vec2Class(position));
        }

        Gizmos.DrawLine(startPos, position);
    }
}
