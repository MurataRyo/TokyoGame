using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FunLight : MonoBehaviour
{
    [SerializeField] SpriteShapeController shapeController;
    [SerializeField] float range;
    [SerializeField] float angle;
    [SerializeField] int vertexCount;

    void FixedUpdate()
    {
        shapeController.spline.Clear();
        shapeController.spline.InsertPointAt(0, new Vector3(0f, 0f, 0f));
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 endPoint = new Vector3(Mathf.Cos(angle / (vertexCount - 1) * i * Mathf.Deg2Rad) * range, Mathf.Sin(angle / (vertexCount - 1) * i * Mathf.Deg2Rad) * range, 0f);
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, endPoint.normalized, range);
            foreach (RaycastHit2D hit2D in hit)
            {
               if(NotRefrect(hit2D))
                {
                    continue;
                }
                Debug.Log(hit2D.collider.name);
                endPoint = transform.InverseTransformPoint(hit2D.point);
                break;
            }
            shapeController.spline.InsertPointAt(0, endPoint);
        }
        shapeController.BakeCollider();
    }

    //光が貫通するかどうか
    bool NotRefrect(RaycastHit2D hit)
    {
        if (hit.collider.tag == GetTag.Refrect)
            return false;

        if (hit.collider.tag == GetTag.Mirror_Back)
            return false;

        if (hit.collider.tag == GetTag.Block)
            return false;

        if (hit.collider.tag == GetTag.NotRefrect)
            return false;

        return true;
    }
}
