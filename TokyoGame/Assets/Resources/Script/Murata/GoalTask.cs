using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTask : MonoBehaviour
{
    [HideInInspector] public List<List<Vec2Class>> rayVartex;  //レイの頂点
    // Start is called before the first frame update
    void Awake()
    {
        rayVartex = new List<List<Vec2Class>>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            LineToTriangle(rayVartexToLines(rayVartex));
        }
    }

    //レイの追加
    public void AddRayVartex(List<Vec2Class> vertex)
    {
        rayVartex.Add(vertex);
    }

    //レイの削除
    public void RemoveRayVartex(List<Vec2Class> vertex)
    {
        rayVartex.Remove(vertex);
    }

    //線情報から三角形情報を取得
    private Triangle[] LineToTriangle(Line[] lines)
    {
        List<Triangle> triangles = new List<Triangle>();
        foreach(Line line in lines)
        {
            for(int i = 0; i < lines.Length;i++)
            {
                if (line == lines[i])
                    continue;

                Vector2 overlap;
                if(LineIfOverlap(line,lines[i],out overlap))
                {
                    Debug.Log(overlap);
                }
            }
        }
        return triangles.ToArray();
    }

    //線が重なっているかを調べ点を返す
    private bool LineIfOverlap(Line linea,Line lineb,out Vector2 overlap)
    {
        overlap = Vector2.zero;
        Vector2 p1 = linea.vartex[0];
        Vector2 p2 = linea.vartex[1];

        Vector2 p3 = lineb.vartex[0];
        Vector2 p4 = lineb.vartex[1];

        var d = (p2.x - linea.vartex[0].x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        overlap.x = p1.x + u * (p2.x - p1.x);
        overlap.y = p1.y + u * (p2.y - p1.y);

        return true;
    }

    //頂点情報から線情報へ変換
    private Line[] rayVartexToLines(List<List<Vec2Class>> vec2a)
    {
        List<Line> lines = new List<Line>();
        foreach (List<Vec2Class> vec2b in vec2a)
        {
            for (int i = 0; i < vec2b.Count - 1; i++)
            {
                //直線情報を追加していく
                lines.Add(new Line(vec2b[i].vec2, vec2b[i + 1].vec2));
            }
        }
        return lines.ToArray();
    }
}

//線の情報を保持するクラス※値型だと同じものか判断できないためクラス
public class Line
{
    public Vector2[] vartex;
    public Line(Vector2 vec2a, Vector2 vec2b)
    {
        vartex = new Vector2[2];
        vartex[0] = vec2a;
        vartex[1] = vec2b;
    }
}

//頂点と線の情報を保持するクラス
public class Triangle
{
    public Vector2[] vartex;
    public Line[] lines;
    public Triangle(Vector2 vec2a, Vector2 vec2b, Vector2 vec2c,Line linea, Line lineb, Line linec)
    {
        vartex = new Vector2[3];
        vartex[0] = vec2a;
        vartex[1] = vec2b;
        vartex[2] = vec2c;

        lines = new Line[3];
        lines[0] = linea;
        lines[1] = lineb;
        lines[2] = linec;
    }
}

