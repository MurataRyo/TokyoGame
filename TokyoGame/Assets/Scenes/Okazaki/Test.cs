using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

public class Test : MonoBehaviour
{
    [SerializeField] SpriteShapeController shapeController;
    [SerializeField] float range;
    [SerializeField] float angle;
    [SerializeField] int vertexCount;

    void Start()
    {
        if(angle == 360)
        {
            CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
            circle.radius = range;
            circle.isTrigger = true;
        }
    }

    void FixedUpdate()
    {
        shapeController.spline.Clear();
        Collider2D[] col2D = Physics2D.OverlapCircleAll(transform.position, 0.01f);
        if (col2D.Where(col => !NotRefrect(col.tag)).ToArray().Length == 0)
        {
            shapeController.spline.InsertPointAt(0, new Vector3(0f, 0f, 0f));
            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 endPoint = new Vector2(Mathf.Cos(angle / (vertexCount - 1) * i * Mathf.Deg2Rad) * range, Mathf.Sin(angle / (vertexCount - 1) * i * Mathf.Deg2Rad) * range);
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.TransformDirection(endPoint.normalized), range);
                foreach (RaycastHit2D hit2D in hit)
                {
                    if (NotRefrect(hit2D.collider.tag))
                    {
                        continue;
                    }
                    endPoint = transform.InverseTransformPoint(hit2D.point);
                    break;
                }
                shapeController.spline.InsertPointAt(0, endPoint);
            }
            shapeController.BakeCollider();
        }
        else
        {
            shapeController.polygonCollider.pathCount = 0;
        }
    }

    //光が貫通するかどうか
    bool NotRefrect(string tag)
    {
        if (tag == GetTag.Refrect)
            return false;

        if (tag == GetTag.Mirror_Back)
            return false;

        if (tag == GetTag.Block)
            return false;

        if (tag == GetTag.NotRefrect)
            return false;

        return true;
    }
}
