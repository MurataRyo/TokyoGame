using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Box2D追加
public class LineRayBox : LineRay
{
    private List<BoxCollider2D> box2d;
    const float box2DSize = 1.0078125f;

    protected override void Start()
    {
        base.Start();

        box2d = new List<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
        
        if (ChangeLight())
        {
            //光の線の数だけコライダーを使用させる
            foreach (BoxCollider2D b in box2d.Skip(keepPoints.Count - 1))
            {
                b.gameObject.SetActive(false);
            }
            //光の線の数だけコライダーを生成
            for (int i = 0; i < keepPoints.Count - 1; i++)
            {
                if (box2d.Count <= i)
                {
                    GameObject box = new GameObject("col");
                    box.tag = GetTag.Col2;
                    box.layer = LayerMask.NameToLayer("Col");
                    box2d.Add(box.AddComponent<BoxCollider2D>());
                    box2d[i].usedByComposite = true;
                    box.transform.parent = transform;
                }
                if (!box2d[i].gameObject.activeSelf)
                {
                    box2d[i].gameObject.SetActive(true);
                }
                box2d[i].transform.rotation = Quaternion.FromToRotation(Vector3.left, keepPoints[i] - keepPoints[i + 1]);
                box2d[i].transform.position = Vector2.Lerp(keepPoints[i], keepPoints[i + 1], 0.5f);
                box2d[i].size = new Vector2(box2DSize * (keepPoints[i] - keepPoints[i + 1]).magnitude, 1f);
            }
        }
        keepLinePrevious = keepPoints;
    }
}
