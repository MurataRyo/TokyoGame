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

    [HideInInspector] public GameObject taskObject;
    [HideInInspector] public GoalTask goalTask;
    [HideInInspector] public ParticleSystem lightLine;
    [HideInInspector] public Vector2 lastPoint; //RayがObjectに当たらないときも光を出すための座標

    protected virtual void Start()
    {
        //　必要な子オブジェクト
        foreach (Transform child in transform)
        {
            if (child.transform.tag == GetTag.LightSource)
            {
                lightLine = child.gameObject.GetComponent<ParticleSystem>();
            }
        }
        taskObject = Utility.GetTaskObject();
        goalTask = taskObject.GetComponent<GoalTask>();
        keepPoints = new List<Vector2>();
        keepLines = new List<Line>();
        keepLinePrevious = new List<Vector2>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        keepPoints = new List<Vector2>();
        keepPoints.Add(new Vector2(transform.position.x, transform.position.y));
        DrawReflect(transform.position + transform.up * 0.75f, transform.up);

        if (ChangeLight())
        {
            keepLines = new List<Line>();

            for (int i = 0; i < keepPoints.Count - 1; i++)
            {
                keepLines.Add(new Line(keepPoints[i], keepPoints[i + 1]));
            }
        }

        AddLightDrow();
    }

    //光の情報が変更されたかどうか
    public bool ChangeLight()
    {
        if (keepLinePrevious.Count != keepPoints.Count)
            return true;

        for (int i = 0; i < keepPoints.Count; i++)
        {
            if (keepLinePrevious[i] != keepPoints[i])
            {
                return true;
            }
        }

        return false;
    }

    //線の始点と終点を調べその間にParticleを1個ずつ並べる
    void AddLightDrow()
    {
        for (int i = 0; i < keepPoints.Count - 1; i++)
        {
            float range = (keepPoints[i] - keepPoints[i + 1]).magnitude * 3;
            for (int j = 0; j < range; j++)
            {
                ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
                emit.position = lightLine.transform.InverseTransformPoint(Vector2.Lerp(keepPoints[i], keepPoints[i + 1], j / range));
                lightLine.Emit(emit, 1);
            }
        }
    }

    //レイを反射させる
    void DrawReflect(Vector2 position, Vector2 direction)
    {
        Vector2 startPos = new Vector2(position.x, position.y); //初期位置

        RaycastHit2D[] hits = Physics2D.RaycastAll(position + direction * 0.5f, direction);
        hits = hits.OrderBy(x => x.distance).ToArray();
        Debug.Log(hits.Length);
        if (hits.Length != 0)
        {
            // 反射地点を変えてその座標からレイを伸ばす
            foreach (RaycastHit2D hit in hits)
            {
                if (IsRefrect(hit)) //光が反射
                {
                    RefrectRay(hit, ref position, ref direction);

                    //Debug.Log("鏡に当たった");
                    DrawReflect(position, direction);

                    return;
                }
                else if (NotRefrect(hit)) //光の終了
                {
                    RefrectRay(hit, ref position, ref direction);
                    //Debug.Log("反射しません");
                    return;
                }
            }
        }

        lastPoint = position + direction * 1000;
        keepPoints.Add(lastPoint);

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
    protected virtual void RefrectRay(RaycastHit2D hit, ref Vector2 position, ref Vector2 direction)
    {
        direction = Vector2.Reflect(direction, hit.normal);
        position = hit.point;
        keepPoints.Add(new Vector2(position.x, position.y));
    }
}
