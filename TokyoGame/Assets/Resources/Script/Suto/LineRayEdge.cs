using System.Linq;
using UnityEngine;

//直線光にEdge追加
public class LineRayEdge : LineRay
{
    private EdgeCollider2D edge2D;

    protected override void Start()
    {
        base.Start();
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == GetTag.Col2)
            {
                edge2D = child.gameObject.GetComponent<EdgeCollider2D>();
            }
        }
        edge2D.edgeRadius = 0.3f;
    }

    protected override void RefrectRay(RaycastHit2D hit, ref Vector2 position, ref Vector2 direction)
    {
        base.RefrectRay(hit, ref position, ref direction);
        ColInfo();
    }

    //レイの通りにコライダー描画
    void ColInfo()
    {
        edge2D.points = keepPoints.Select(v => (Vector2)transform.InverseTransformPoint(v)).ToArray();
    }
}
