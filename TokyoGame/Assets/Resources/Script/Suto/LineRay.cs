using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LineRay : MonoBehaviour
{
    [HideInInspector] public List<Vector2> keepPoints; //座標を保存
    [HideInInspector] public List<Line> keepLines; //線を保存
    [HideInInspector] public List<Vector2> keepLinePrevious; //1つ前の線の始点を保存
    [HideInInspector] public Vector2[] keepLinePreviousPlus; //1つ前の線の終点を保存
    private GameObject colGo;
    private EdgeCollider2D edge2D;
    private GameObject taskObject;
    private GoalTask goalTask;
    private GameObject lightBase;
    private ParticleSystem lightLine;

    void Start()
    {
        //　必要な子オブジェクト
        foreach (Transform chiild in transform)
        {
            if (chiild.gameObject.tag == GetTag.Col)
            {
                colGo = chiild.gameObject;
                break;
            }
        }
        foreach (Transform child2 in transform)
        {

            if (child2.transform.tag == GetTag.LightSource)
            {
                lightBase = child2.gameObject;
                break;
            }
        }

        lightLine = lightBase.GetComponent<ParticleSystem>();
        edge2D = colGo.GetComponent<EdgeCollider2D>();
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

        keepLines = new List<Line>();

        DrawReflect(transform.position + transform.right * 0.75f, transform.right);

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
            keepLinePrevious = keepPoints;



            if (keepLinePrevious[i].x != keepPoints[i].x && keepLinePrevious[i].y != keepPoints[i].y)
            {

                keepLinePrevious = new List<Vector2>();
                Debug.Log("aaa");
            }
        }
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
            else if (NotRefrect(hit)) //光が終了
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

        if (hit.collider.tag != GetTag.Mirror)
            return false;

        return true;
    }

    //反射不可能
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

        if (hit.collider.tag == "LaunchHit")
            return false;

        return true;
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
