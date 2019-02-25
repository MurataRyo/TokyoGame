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
        originate = new List<Vec2Class>();
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
            BoxCol2D(hit.point);
            if (IsRefrect(hit)) //光が反射
            {
                direction = Vector2.Reflect(direction, hit.normal);
                position = hit.point;

                originate.Add(new Vec2Class(position));
                Debug.Log("鏡に当たった");
                DrawReflect(position, direction);
                break;
            }
            else if (NotRefrect(hit)) //光が終了
            {
                direction = Vector2.Reflect(direction, hit.normal);
                position = hit.point;
                originate.Add(new Vec2Class(position));
                Debug.Log("反射しません");
                break;
            }
            else //光が貫通
            {
                continue;
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
