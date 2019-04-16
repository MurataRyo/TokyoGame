using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpot : MonoBehaviour
{
    [SerializeField] float speed;//移動スピード
    private int pointIndex = 0;//  現データインデックス
    [SerializeField] Vector2[] pointDate;//移動先の座標を保存

    void Start()
    {
        transform.position = pointDate[pointIndex];//最初の位置を保存した位置の最初に設定
    }

    void Update()
    {
        movePos();
    }

    void movePos()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        float range = (pos - pointDate[pointIndex]).magnitude;
        float move = speed * Time.deltaTime;

        //通り越しそうなら指定された場所までしか動かない
        while (range < move)
        {
            pos = pointDate[pointIndex];
            pointIndex = pointIndex == pointDate.Length - 1 ? 0 : pointIndex + 1;
            move -= range;
            range = (pos - pointDate[pointIndex]).magnitude;
        }
        pos = Vector2.MoveTowards(pos, pointDate[pointIndex], move);
        transform.position = pos;
    }
}