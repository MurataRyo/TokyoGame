using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCollider : MonoBehaviour
{
    EdgeCollider2D edge2D;

    // Start is called before the first frame update
    public void Create(Vector2[] vec2s)
    {
        edge2D = gameObject.AddComponent<EdgeCollider2D>();
        //この数字はRayに合わせる
        edge2D.edgeRadius = 0.45f;
        ColUpdate(vec2s);
    }

    public void ColUpdate(Vector2[] vec2s)
    {
        edge2D.points = vec2s;
    }

    public void DeleteCol()
    {
        Destroy(edge2D);
        Destroy(this);
    }
}
