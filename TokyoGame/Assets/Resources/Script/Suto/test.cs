using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class test : MonoBehaviour
{
    [HideInInspector] private float maxStep = 100; //伸びる距離
    [HideInInspector] public List<Vec2Class> originate; //反射した座標を保存
    [HideInInspector] public float boxRange; //コライダーの長さ
    GameObject colBox; //コライダーを入れる箱
    GameObject taskObject; 
    GoalTask goalTask;

    #region
    //[HideInInspector] LineRenderer line; //線
    //[HideInInspector] float counter; // 伸びるカウント
    //[HideInInspector] float dist; //距離
    //[HideInInspector] GameObject startobj; //スタートオブジェクト
    //[HideInInspector] GameObject goalobj; //目標オブジェクト
    //[HideInInspector] float drawSpeed = 6f; //出る速さ
    #endregion

    void Start()
    {
        //　コライダーを入れる子オブジェクト
        foreach (Transform chiild in transform)
        {
            if (chiild.gameObject.tag == GetTag.Col)
            {
                colBox = chiild.gameObject;
                break;
            }
        }
        taskObject =
            Utility.GetTask();
        goalTask = taskObject.GetComponent<GoalTask>();
        #region
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
        #endregion
        originate = new List<Vec2Class>();
    }

    void Update()
    {

        //foreach(RaycastHit2D hits in Physics2D.RaycastAll(transform.position, transform.right))
        //{
        //    Debug.Log(hits.collider.gameObject.name);
        //}

        #region
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
        #endregion
    }

    //レイをシーンで見えるようにする
    void OnDrawGizmos()
    {
        goalTask.RemoveRayVartex(originate);
        originate = new List<Vec2Class>();
        originate.Add(new Vec2Class(transform.position));
        DrawReflect(transform.position + transform.right * 0.75f, transform.right);
        goalTask.AddRayVartex(originate);
    }

    //レイを反射させる
    void DrawReflect(Vector2 position, Vector2 direction)
    {
        Vector2 startPos = new Vector2(position.x, position.y); //初期位置

        // 反射地点を変えてその座標からレイを伸ばす
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(position + direction * 0.5f, direction))
        {
            //BoxCol2D(hit.point);
            if (IsRefrect(hit)) //鏡に当たった時
            {
                direction = Vector2.Reflect(direction, hit.normal);
                position = hit.point;

                originate.Add(new Vec2Class(position));
                Debug.Log("鏡に当たった");
                DrawReflect(position, direction);
            }
            else if (NotRefrect(hit)) //光を終了させるもの
            {
                direction = Vector2.Reflect(direction, hit.normal);
                position = hit.point;
                originate.Add(new Vec2Class(position));
                Debug.Log("反射しません");
                break;
            }
            else //光を貫通するもの
            {
                continue;
                //position += direction * maxStep;
                //originate.Add(new Vec2Class(position));
            }


        }


        Gizmos.DrawLine(startPos, position);
    }

    //反射可能かどうかの検索
    bool IsRefrect(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return false;

        if (hit.collider.tag != GetTag.Mirror)
            return false;

        return true;
    }

    bool NotRefrect(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return false;

        if (hit.collider.tag == GetTag.Mirror)
            return false;

        if (hit.collider.tag == GetTag.Player)
            return false;

        if (hit.collider.tag == GetTag.Glass)
            return false;

        if (hit.collider.tag == GetTag.Col)
            return false;

        return true;
    }

    // コライダー
    void BoxCol2D(Vector2 direction)
    {
        Vector2 colBoxPos = new Vector2(colBox.transform.position.x, colBox.transform.position.y);

        boxRange = (colBoxPos - direction).magnitude;
        BoxCollider2D bX2D = colBox.GetComponent<BoxCollider2D>();
        bX2D.offset = new Vector2(BoxRangeMath(colBoxPos, direction), 0);
        bX2D.size = new Vector2(boxRange, 0.25f);
    }

    float BoxRangeMath(Vector2 colBoxPos, Vector2 direction)
    {
        return boxRange - boxRange / 2;
    }
}
