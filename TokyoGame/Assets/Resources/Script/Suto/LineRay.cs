using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LineRay : MonoBehaviour
{
    [HideInInspector] public List<Vector2> keepPoints; //座標を保存
    [HideInInspector] public List<Line> keepLines; //線を保存
    [HideInInspector] public List<Vector2> keepLinePrevious; //保存

    private GameObject colGo;
    private EdgeCollider2D edge2D;
    private GameObject taskObject;
    private GoalTask goalTask;
    private GameObject lightBase;
    private ParticleSystem lightLine;

    void Start()
    {
        //　必要な子オブジェクト
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == GetTag.Col)
            {
                edge2D = child.gameObject.GetComponent<EdgeCollider2D>();
            }
            if (child.transform.tag == GetTag.LightSource)
            {
                lightLine = child.gameObject.GetComponent<ParticleSystem>();
            }
        }
        edge2D.edgeRadius = 0.45f;
        taskObject = Utility.GetTaskObject();
        goalTask = taskObject.GetComponent<GoalTask>();
        keepPoints = new List<Vector2>();
        keepLines = new List<Line>();
        keepLinePrevious = new List<Vector2>();
    }

    //Particleの反射
    void Update()
    {
        keepPoints = new List<Vector2>();
        keepPoints.Add(new Vector2(transform.position.x, transform.position.y));



        DrawReflect(transform.position + transform.right * 0.75f, transform.right);

        if (ChangeLight())
        {
            keepLines = new List<Line>();
            //線の始点と終点を調べその間にParticleを1個ずつ並べる
            for (int i = 0; i < keepPoints.Count - 1; i++)
            {
                float range = (keepPoints[i] - keepPoints[i + 1]).magnitude * 10;
                for (int j = 0; j < range; j++)
                {
                    ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
                    emit.position = lightLine.transform.InverseTransformPoint(Vector2.Lerp(keepPoints[i], keepPoints[i + 1], j / range));
                    lightLine.Emit(emit, 1);
                }
                keepLines.Add(new Line(keepPoints[i], keepPoints[i + 1]));
            }
        }
        keepLinePrevious = keepPoints;
    }

    //光の情報が変更されたかどうか
    private bool ChangeLight()
    {
        if (keepLinePrevious.Count != keepPoints.Count)
            return true;

        for (int i = 0; i < keepPoints.Count; i++)
        {
            if (keepLinePrevious[i] != keepPoints[i])
                return true;
        }

        return false;
    }

    //レイを反射させる
    void DrawReflect(Vector2 position, Vector2 direction)
    {
        Vector2 startPos = new Vector2(position.x, position.y); //初期位置

        // 反射地点を変えてその座標からレイを伸ばす
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(position + direction * 0.5f, direction))
        {
            if (IsRefrect(hit)) //光が反射
            {
                RefrectRay(hit, ref position, ref direction);

                //Debug.Log("鏡に当たった");
                DrawReflect(position, direction);

                break;
            }
            else if (NotRefrect(hit)) //光の終了
            {
                RefrectRay(hit, ref position, ref direction);
                //Debug.Log("反射しません");
                break;
            }
        }
    }

    //反射可能
    bool IsRefrect(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return false;

        if (hit.collider.tag == GetTag.Refrect)
            return true;

        return false;
    }

    //光が貫通するかどうか
    bool NotRefrect(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return false;

        if (hit.collider.tag == GetTag.Block)
            return true;

        if (hit.collider.tag == GetTag.Mirror_Back)
            return true;

        return false;
    }

    //レイの始点と終点を調べる
    void RefrectRay(RaycastHit2D hit, ref Vector2 position, ref Vector2 direction)
    {
        direction = Vector2.Reflect(direction, hit.normal);
        position = hit.point;
        keepPoints.Add(new Vector2(position.x, position.y));
        ColInfo();
    }

    //レイの通りにコライダー描画
    void ColInfo()
    {
        edge2D.points = keepPoints.Select(v => (Vector2)transform.InverseTransformPoint(v)).ToArray();
    }
}
