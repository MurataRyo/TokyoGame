using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    [SerializeField] float speed;//スピード
    private int pointIndex = 0;//  現データインデックス
    [SerializeField] Vector2[] pointDate;//移動先の座標を保存
    private Vector2 defaultPos;
    private Rigidbody2D rb2D;
    private TargetJoint2D tj2D;



    void Start()
    {
        defaultPos = transform.position;//初期座標にする
        for (int i = 0; i < pointDate.Length; i++)
        {
            pointDate[i] += defaultPos;
        }
        transform.position = pointDate[pointIndex];//最初の位置を保存した位置の最初に設定
        rb2D = GetComponent<Rigidbody2D>();
        tj2D = GetComponent<TargetJoint2D>();

    }

    void FixedUpdate()
    {
        movePos();
    }

    void movePos()
    {
        Vector2 pos = tj2D.target;
        float range = (pos - pointDate[pointIndex]).magnitude;//距離
        float move = speed * Time.fixedDeltaTime;//速度

        //通り越しそうなら指定された場所までしか動かない
        while (range < move)
        {
            pos = pointDate[pointIndex];
            pointIndex = pointIndex == pointDate.Length - 1 ? 0 : pointIndex + 1;
            move -= range;
            range = (pos - pointDate[pointIndex]).magnitude;
        }

        pos = Vector2.MoveTowards(pos, pointDate[pointIndex], move);
        tj2D.target = pos;
    }
}
