using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    private float speed;//移動スピード
    private int pointIndex = 0;//  現データインデックス
    [SerializeField] Vector3[] pointDate;//移動先の座標を保存

    void Start()
    {
        speed = 0f;
        transform.position = pointDate[pointIndex];//最初の位置を保存した位置の最初に設定
    }

    void Update()
    {
        speed += 0.25f * Time.deltaTime;
        if (speed >= 1.0f)
        {
            speed = 0f;
            //最初の地点に戻す
            if (++pointIndex >= pointDate.Length - 1)
                pointIndex = 0;
        }
        //保存した前後の位置を補完してその間を一直線で移動
        transform.position = Vector3.Lerp(pointDate[pointIndex], pointDate[pointIndex + 1], speed);
    }
}
