using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapBackCreate : MonoBehaviour
{
    [SerializeField] Vector2Int num;
    [SerializeField] float size;
    private bool[,] flag;
    GameObject parent;
    enum Add
    {
        t1_1,
        t2_1,
        t3_1,
    }

    // Start is called before the first frame update
    void Start()
    {
        parent = new GameObject();
        parent.transform.position = transform.position;
        flag = new bool[num.x, num.y];
        for (int i = 0; i < num.x; i++)
        {
            for (int j = 0; j < num.y; j++)
            {
                flag[i, j] = false;
            }
        }

        for (int i = 0; i < num.x; i++)
        {
            for (int j = 0; j < num.y; j++)
            {
                if (flag[i, j])
                    continue;

                while (true)
                {
                    int range = Enum.GetValues(typeof(Add)).Length;
                    int k = UnityEngine.Random.Range(0, 101);
                    if (k < 75)
                        k = 0;
                    else if(k < 90)
                    {
                        k = 1;
                    }
                    else
                    {
                        k = 2;
                    }

                    if (CreateNum((Add)k, i, j))
                    {
                        CreateNum(i, j, (Add)k);
                        break;
                    }
                }
            }
        }
    }

    void CreateNum(int i, int j, Add add)
    {
        GameObject go = new GameObject();
        SpriteRenderer rend = go.AddComponent<SpriteRenderer>();
        
        string path = "/" + add.ToString();
        int blockNum = Resources.LoadAll<Sprite>(GetPath.StageBack + path).Length;
        Sprite sprite = Resources.Load<Sprite>(GetPath.StageBack + path + "/tile" + UnityEngine.Random.Range(1, blockNum + 1).ToString());

        rend.sortingOrder = -2;
        rend.sprite = sprite;
        go.transform.localScale = Vector3.one * size;
        go.transform.parent = parent.transform;
        go.name = "StageBlock";
        go.transform.localPosition = new Vector3(i * size, j * size, 1.5f);
    }

    bool CreateNum(Add add, int i, int j)
    {
        switch (add)
        {
            case Add.t1_1:
                flag[i, j] = true;
                return true;

            case Add.t2_1:

                if (flag[i, j])
                    return false;

                if (flag.GetLongLength(0) <= i + 1)
                    return false;

                if (flag[i + 1, j])
                    return false;

                flag[i, j] = true;
                flag[i + 1, j] = true;
                return true; ;

            case Add.t3_1:

                if (flag[i, j])
                    return false;

                if (flag.GetLongLength(0) <= i + 1)
                    return false;

                if (flag[i + 1, j])
                    return false;

                if (flag.GetLongLength(1) <= j + 1)
                    return false;

                if (flag[i, j + 1])
                    return false;
                
                if (flag[i + 1, j + 1])
                    return false;

                flag[i, j] = true;
                flag[i + 1, j] = true;
                flag[i, j + 1] = true;
                flag[i + 1, j + 1] = true;
                return true; ;
        }
        return false;
    }
}
