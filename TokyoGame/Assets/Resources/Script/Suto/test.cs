using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class test : MonoBehaviour
{
    private float maxStep = 100; //伸びる距離
    [HideInInspector] public List<Vec2Class> originate; //座標を保存
    private GameObject colGo;
    private EdgeCollider2D edge2D;
    private GameObject taskObject;
    private GoalTask goalTask;
    private GameObject lightBase;
    private ParticleSystem lightLine;
    private float loads;

    void Start()
    {
        //　コライダーを入れる子オブジェクト
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
        loads = 0f;
        edge2D = colGo.GetComponent<EdgeCollider2D>();
        edge2D.edgeRadius = 0.45f;
        taskObject = Utility.GetTask();
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
            var main = lightLine.main;
            loads = (startPos - hit.point).magnitude;
            Debug.Log((startPos - hit.point).magnitude);
            main.startLifetimeMultiplier = 0.1f;
            main.startSpeed = loads / main.startLifetimeMultiplier;

            if (IsRefrect(hit)) //光が反射
            {
                RefrectRay(hit, ref position, ref direction);

                Debug.Log("鏡に当たった");
                DrawReflect(position, direction);
                break;
            }
            else if (NotRefrect(hit)) //光が終了
            {
                RefrectRay(hit, ref position, ref direction);
                Debug.Log("反射しません");
                break;
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

    void RefrectRay(RaycastHit2D hit, ref Vector2 position, ref Vector2 direction)
    {
        direction = Vector2.Reflect(direction, hit.normal);
        position = hit.point;
        originate.Add(new Vec2Class(position));
        edge2D.points = originate.Select(v => (Vector2)transform.InverseTransformPoint(v.vec2)).ToArray();

    }
}
