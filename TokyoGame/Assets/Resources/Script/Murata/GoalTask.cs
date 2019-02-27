using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoalTask : MonoBehaviour
{
    [HideInInspector] public List<List<Vec2Class>> rayVartex;  //レイの頂点
    // Start is called before the first frame update
    private void Awake()
    {
        rayVartex = new List<List<Vec2Class>>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Line[] lines = rayVartexToLines(rayVartex);
            Overlap[] overlaps = LineToOverlap(lines);
            Triangle[] triangles = OverlapToTriangle(overlaps, lines);
            Star[] stars = TrianglesToStars(triangles);
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

    //三角形から星形を取得
    private Star[] TrianglesToStars(Triangle[] triangles)
    {
        List<Star> stars = new List<Star>();

        //三角形の候補            ※複数回やることにより早くなるかも?
        Triangle[] triCandidate = TriangleToTriangleCandidate(triangles);

        return stars.ToArray();
    }

    //三角形から星になる候補を返す
    private Triangle[] TriangleToTriangleCandidate(Triangle[] triangles)
    {
        //三角形の候補
        List<Triangle> triCandidate = new List<Triangle>();

        foreach (Triangle triangleA in triangles)
        {
            //他の三角形と同じ頂点を2つ以上持っているかを調べている
            if (OverlapInTriangle(triangleA, triangles))
            {
                triCandidate.Add(triangleA);
            }
        }

        return triCandidate.ToArray();
    }

    //他の三角形と同じ頂点を2つ以上持っているか※同じ三角形からは1つの頂点まで
    private bool OverlapInTriangle(Triangle triangleBase, Triangle[] triangles)
    {
        int count = 0;
        foreach (Overlap overlapBase in triangleBase.overlaps)
        {
            foreach (Triangle triangleA in triangles)
            {
                if (triangleA == triangleBase)
                    continue;

                if (InIfOverlap(overlapBase, triangleA))
                {
                    count++;
                    //2つ以上他の三角形と同じ頂点があるので追加
                    if (count >= 2)
                        return true;

                    break;
                }
            }
        }

        return false;
    }

    //おなじ頂点を持っているか
    private bool InIfOverlap(Overlap overlapBase, Triangle triangle)
    {
        foreach (Overlap overlap in triangle.overlaps)
        {
            if (overlap == overlapBase)
                return true;
        }
        return false;
    }

    //線情報と頂点情報から三角形情報を取得
    private Triangle[] OverlapToTriangle(Overlap[] overlaps, Line[] lines)
    {
        List<Triangle> triangles = new List<Triangle>();

        //三角形を探す

        //線の中身を検索
        foreach (Line line in lines)
        {
            //線の頂点が2未満なら絶対に三角形は作れないのから
            if (line.overlaps.Count < 2)
                continue;

            //線の2つの頂点を比べる
            foreach (Overlap overlapA in line.overlaps)
            {
                foreach (Overlap overlapB in line.overlaps)
                {
                    if (overlapA == overlapB)
                        continue;

                    //頂点の線を調べる
                    foreach (Line linea in overlapA.lines)
                    {
                        foreach (Line lineb in overlapB.lines)
                        {
                            //線が同じなら3角形ではない
                            if (linea == lineb)
                                continue;

                            //線の頂点を調べる
                            foreach (Overlap overlapC in linea.overlaps)
                            {
                                foreach (Overlap overlapD in lineb.overlaps)
                                {
                                    //２つの線が同じ頂点を持っていなければ出来ない
                                    if (overlapC != overlapD ||
                                    //同じ頂点で3角形は作れない
                                        overlapA == overlapC ||
                                        overlapB == overlapC)
                                        continue;

                                    Triangle newTriangle = new Triangle(overlapA, overlapB, overlapC);

                                    if (!CreateIfTriangle(triangles, newTriangle))
                                        continue;

                                    newTriangle.LineCreate();
                                    triangles.Add(newTriangle);
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach(Triangle triangleZ in triangles)
        {
            Debug.Log(triangleZ.overlaps[0].pos + " " + triangleZ.overlaps[1].pos + " " + triangleZ.overlaps[2].pos);
        }
        return triangles.ToArray();
    }

    //三角形が生成できるかどうか
    private bool CreateIfTriangle(List<Triangle> triangles, Triangle newTriangle)
    {

        foreach (Triangle triangle in triangles)
        {
            if (triangle.overlaps[0] == newTriangle.overlaps[0] &&
                triangle.overlaps[1] == newTriangle.overlaps[1] &&
                triangle.overlaps[2] == newTriangle.overlaps[2])
            {
                return false;
            }
        }
        return true;
    }

    //線から重なってる点へ変換
    private Overlap[] LineToOverlap(Line[] lines)
    {
        List<Overlap> overlaps = new List<Overlap>();
        foreach (Line line in lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (line == lines[i])
                    continue;

                Vector2 overlap;
                if (!LineIfOverlap(line, lines[i], out overlap))
                    continue;

                Overlap overlapClass = new Overlap(line, lines[i], overlap);
                if (IsCreateOverlap(overlapClass, overlaps.ToArray()))
                {
                    overlaps.Add(overlapClass);
                }
            }
        }

        return overlaps.ToArray();
    }

    //頂点を生成できるかどうか
    private bool IsCreateOverlap(Overlap newOverlap, Overlap[] nowOverlap)
    {
        foreach (Overlap overlap in nowOverlap)
        {
            //そもそも点の座補が違ったら生成可能なので入れる
            if (newOverlap.pos != overlap.pos)
                continue;

            //同じ線を利用していたら生成できない
            if (newOverlap.lines[0] == overlap.lines[0] && newOverlap.lines[1] == overlap.lines[1] ||
                newOverlap.lines[1] == overlap.lines[0] && newOverlap.lines[0] == overlap.lines[1])
            {
                return false;
            }
        }

        newOverlap.LineAddOverlap();
        return true;
    }

    //線が重なっているかを調べ点を返す
    //今後数学的に調べる場所
    private bool LineIfOverlap(Line linea, Line lineb, out Vector2 overlap)
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
    public List<Overlap> overlaps;
    public Vector2[] vartex;
    public Line(Vector2 vec2a, Vector2 vec2b)
    {
        overlaps = new List<Overlap>();
        vartex = new Vector2[2];
        vartex[0] = vec2a;
        vartex[1] = vec2b;
    }
}

//交わっている点の情報を保持するクラス
public class Overlap : IComparable
{
    public Line[] lines;
    public Vector2 pos;
    public Overlap(Line linea, Line lineb, Vector2 pos)
    {
        lines = new Line[2];
        lines[0] = linea;
        lines[1] = lineb;
        this.pos = pos;
    }

    //バグの原因の可能性あり並び替え
    public int CompareTo(object obj)
    {
        Overlap lap = (Overlap)obj;

        if (pos.x > lap.pos.x)
        {
            return -1;
        }
        else if (pos.x < lap.pos.x)
        {
            return 1;
        }
        else if (pos.y > lap.pos.y)
        {
            return -1;
        }
        else if (pos.y < lap.pos.y)
        {
            return 1;
        }

        return 0;
    }

    public void LineAddOverlap()
    {
        lines[0].overlaps.Add(this);
        lines[1].overlaps.Add(this);
    }
}

//頂点と線の情報を保持するクラス
public class Triangle
{
    public Overlap[] overlaps;
    public Line[] lines;
    public Triangle(Overlap overlapsA, Overlap overlapsB, Overlap overlapsC)
    {
        overlaps = new Overlap[3];
        overlaps[0] = overlapsA;
        overlaps[1] = overlapsB;
        overlaps[2] = overlapsC;

        Array.Sort(overlaps);
    }

    public void LineCreate()
    {
        List<Line> lineList = new List<Line>();
        foreach (Overlap overlapA in overlaps)
        {
            foreach (Overlap overlapB in overlaps)
            {
                if (overlapA == overlapB)
                    continue;

                foreach (Line lineA in overlapA.lines)
                {
                    foreach (Line lineB in overlapB.lines)
                    {
                        //同じ線じゃないならやり直し
                        if (lineA != lineB)
                            continue;

                        //元々リストに入っていたらやり直し
                        if (IfInLine(lineList.ToArray(), lineA))
                            continue;

                        //頂点のもととなっている線をリストに入れる
                        lineList.Add(lineA);
                    }
                }
            }
        }

        lines = lineList.ToArray();
    }

    public bool IfInLine(Line[] lines, Line newLine)
    {
        foreach (Line line in lines)
        {
            if (newLine == line)
                return true;
        }
        return false;
    }
}

public class Star
{
    public Triangle[] triangles;
    public Star(Triangle triangleA, Triangle triangleB, Triangle triangleC, Triangle triangleD, Triangle triangleE)
    {
        triangles = new Triangle[5];
        triangles[0] = triangleA;
        triangles[1] = triangleB;
        triangles[2] = triangleC;
        triangles[3] = triangleD;
        triangles[4] = triangleE;
    }
}


