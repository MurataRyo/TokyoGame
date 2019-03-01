using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoalTask : MonoBehaviour
{
    [HideInInspector] public List<List<Vec2Class>> rayVartex;  //レイの頂点
    [HideInInspector] public List<Line> GetLines;
    // Start is called before the first frame update
    private void Awake()
    {
        GetLines = new List<Line>();
        rayVartex = new List<List<Vec2Class>>();
    }

    //確認用
    private void CreateBlock(Star star)
    {
        GameObject go = new GameObject();
        CreateBlock(star.triangles[0].overlaps[0].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[0].overlaps[1].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[0].overlaps[2].pos).transform.parent = go.transform;

        CreateBlock(star.triangles[1].overlaps[0].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[1].overlaps[1].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[1].overlaps[2].pos).transform.parent = go.transform;

        CreateBlock(star.triangles[2].overlaps[0].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[2].overlaps[1].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[2].overlaps[2].pos).transform.parent = go.transform;

        CreateBlock(star.triangles[3].overlaps[0].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[3].overlaps[1].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[3].overlaps[2].pos).transform.parent = go.transform;

        CreateBlock(star.triangles[4].overlaps[0].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[4].overlaps[1].pos).transform.parent = go.transform;
        CreateBlock(star.triangles[4].overlaps[2].pos).transform.parent = go.transform;
    }

    private GameObject CreateBlock(Vector2 pos)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefab/Stage/Block/Block"));
        go.transform.position = new Vector3(pos.x - 0.05f, pos.y - 0.05f, 0f);
        go.transform.localScale = Vector3.one * 0.1f;

        return go;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("線の数は" + rayVartex.Count + "本");
            Line[] lines = rayVartexToLines(rayVartex);
            Star[] stars = LineToStar(lines);
            if (stars != null)
            {
                Debug.Log("星は" + stars.Length + "個あります");
            }
            else
            {
                Debug.Log("星はありません");
            }
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

    private Star[] LineToStar(Line[] lines)
    {
        int q = 0;
        //線が５本未満なら星が作れないのでNullを返す
        if (lines.Length < 5)
            return null;

        List<Star> starList = new List<Star>();
        for (int i = 0; i < lines.Length - 4; i++)
        {
            for (int j = i + 1; j < lines.Length - 3; j++)
            {
                if (!LineIfOverlaps(lines[j], lines, new int[] { i }))
                {
                    q++;
                    continue;
                }

                for (int k = j + 1; k < lines.Length - 2; k++)
                {
                    if (!LineIfOverlaps(lines[k], lines, new int[] { i, j }))
                    {
                        q++;
                        continue;
                    }
                    for (int x = k + 1; x < lines.Length - 1; x++)
                    {
                        if (!LineIfOverlaps(lines[x], lines, new int[] { i, j, k }))
                        {
                            q++;
                            continue;
                        }
                        for (int y = x + 1; y < lines.Length; y++)
                        {
                            if (!LineIfOverlaps(lines[y], lines, new int[] { i, j, k, x }))
                            {
                                q++;
                                continue;
                            }
                            q++;
                            //5本の線をリストアップする
                            Line[] lineFive = new Line[] { lines[i], lines[j], lines[k], lines[x], lines[y] };
                            Overlap[] overlaps = LineToOverlap(lineFive);
                            Star star = null;
                            if (OverlapsAndLinesToStar(overlaps, lineFive, out star))
                                starList.Add(star);
                        }
                    }
                }
            }
        }
        Debug.Log("試行回数は" + q + "回");
        return starList.ToArray();
    }

    private bool OverlapsAndLinesToStar(Overlap[] overlaps, Line[] lines, out Star star)
    {
        List<Overlap> inSideOverlap = new List<Overlap>();
        List<Overlap> outSideOverlap = new List<Overlap>();
        star = null;
        foreach (Line line in lines)
        {
            float min = Mathf.Infinity;
            Overlap minOverlap = null;
            float max = -Mathf.Infinity;
            Overlap maxOverlap = null;
            foreach (Overlap overlap in line.overlaps)
            {
                if (min > overlap.pos.x)
                {
                    minOverlap = overlap;
                    min = overlap.pos.x;
                }

                if (max < overlap.pos.x)
                {
                    maxOverlap = overlap;
                    max = overlap.pos.x;
                }
            }
            foreach (Overlap overlap in line.overlaps)
            {
                if (overlap == maxOverlap || overlap == minOverlap)
                {
                    if (outSideOverlap.Contains(overlap))
                        return false;

                    if (!inSideOverlap.Contains(overlap))
                        inSideOverlap.Add(overlap);
                }
                else
                {
                    if (inSideOverlap.Contains(overlap))
                        return false;

                    if (!outSideOverlap.Contains(overlap))
                        outSideOverlap.Add(overlap);
                }
            }
        }
        star = new Star(outSideOverlap.ToArray(), inSideOverlap.ToArray());
        return true;
    }

    //三角形から星形を取得
    //ここのforが多いのが重たい原因
    public Star[] TrianglesToStars(Triangle[] trianglesIn)
    {
        //三角形が5つ以上ないと星はできないので5未満なら計算しない
        if (trianglesIn.Length < 5)
            return null;
        //Debug.Log(triCandidate.Length);
        int q = 0;
        List<Star> stars = new List<Star>();
        //全ての組み合わせを試す
        for (int i = 0; i < trianglesIn.Length - 4; i++)
        {
            for (int j = i + 1; j < trianglesIn.Length - 3; j++)
            {
                for (int k = j + 1; k < trianglesIn.Length - 2; k++)
                {
                    for (int x = k + 1; x < trianglesIn.Length - 1; x++)
                    {
                        for (int y = x + 1; y < trianglesIn.Length; y++)
                        {
                            Triangle[] triangles = new Triangle[5];
                            triangles[0] = trianglesIn[i];
                            triangles[1] = trianglesIn[j];
                            triangles[2] = trianglesIn[k];
                            triangles[3] = trianglesIn[x];
                            triangles[4] = trianglesIn[y];
                            q++;
                            if (IfStar(triangles))
                            {
                                Star newStar = new Star(triangles);

                                stars.Add(newStar);
                            }
                        }
                    }
                }
            }
        }
        return stars.ToArray();
    }
    //星が作れるかどうか
    private bool IfStar(Triangle[] triangles)
    {
        Triangle[] trianglesA = TriangleToTriangleCandidate(triangles);

        if (trianglesA == null || trianglesA.Length != 5)
            return false;

        if (!LineCrossIf(triangles))
            return false;

        if (!LineFiveIf(triangles))
            return false;

        Debug.Log("星の生成可能");
        return true;
    }

    //線が重なっているかどうか
    private bool LineCrossIf(Triangle[] triangles)
    {
        List<Line> lines = new List<Line>();
        foreach (Triangle triangle in triangles)
        {
            lines.Add(new Line(triangle.overlaps[0].pos, triangle.overlaps[1].pos));
            lines.Add(new Line(triangle.overlaps[1].pos, triangle.overlaps[2].pos));
            lines.Add(new Line(triangle.overlaps[2].pos, triangle.overlaps[0].pos));
        }

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; i + j < lines.Count; j++)
            {
                Vector2 overlap;
                if (LineIfOverlap(lines[i], lines[i + j], out overlap))
                {
                    //線が交わっていたら星はできないのでfalseを返す
                    if (overlap != lines[i].vartex[0] &&
                        overlap != lines[i].vartex[1])
                        return false;
                }
            }
        }
        return true;
    }

    //三角形の頂点と同じであるか※Listの三角形は無視をする
    private bool OverlapIf(ref Triangle triangleBase, List<Triangle> endTriangle, Triangle[] triangles)
    {
        foreach (Triangle triangle in triangles)
        {
            if (InIfTriangle(triangle, endTriangle.ToArray()))
                continue;

            if (triangleBase == triangle)
                continue;

            if (InIfOverlap(triangleBase, triangle))
            {
                endTriangle.Add(triangleBase);
                triangleBase = triangle;

                return true;
            }
        }
        return false;
    }

    //同じ頂点を持っているか
    private bool InIfOverlap(Triangle triangleBase, Triangle triangle)
    {
        foreach (Overlap overlap in triangleBase.overlaps)
        {
            if (InIfOverlap(overlap, triangle.overlaps))
                return true;
        }
        return false;
    }

    //同じ三角形を使用しているかどうか
    private bool InIfTriangle(Triangle triangleBase, Triangle[] triangles)
    {
        foreach (Triangle triangle in triangles)
        {
            if (triangle == triangleBase)
                return true;
        }
        return false;
    }

    //5本の線で出来ているかどうか
    private bool LineFiveIf(Triangle[] triangles)
    {
        List<Line> lines = new List<Line>();
        foreach (Triangle triangle in triangles)
        {
            foreach (Line line in triangle.lines)
            {
                if (!InIfLine(line, lines.ToArray()))
                    lines.Add(line);
            }
        }

        return lines.Count == 5;
    }

    //三角形から星になる候補を返す
    private Triangle[] TriangleToTriangleCandidate(Triangle[] triangles)
    {
        //三角形の候補
        Triangle[] triCandidate = LineInTriangle(triangles);
        triCandidate = OverlapInTriangle(triCandidate);

        return triCandidate;
    }

    //全ての直線が他の三角形の直線2本以上使われているのを返す
    private Triangle[] LineInTriangle(Triangle[] triangles)
    {
        List<Triangle> outTriangle = new List<Triangle>();

        foreach (Triangle triangleA in triangles)
        {
            if (LineInTriangle(triangleA, triangles))
                outTriangle.Add(triangleA);
        }

        return outTriangle.ToArray();
    }

    //全ての直線が他の三角形の直線2本以上使われているどうか
    private bool LineInTriangle(Triangle triangleBase, Triangle[] triangles)
    {
        foreach (Line lineA in triangleBase.lines)
        {
            int count = 2;
            foreach (Triangle triangleA in triangles)
            {
                if (triangleA == triangleBase)
                    continue;

                if (InIfLine(lineA, triangleA.lines))
                    count--;
            }

            if (count > 0)
                return false;

        }

        return true;
    }

    //同じ線を持っているか
    private bool InIfLine(Line lineBase, Line[] lines)
    {
        foreach (Line line in lines)
        {
            if (line == lineBase)
                return true;
        }
        return false;
    }

    //他の三角形と同じ頂点を3つ以上持っている三角形を返す
    private Triangle[] OverlapInTriangle(Triangle[] triangles)
    {
        List<Triangle> triCandidate = new List<Triangle>();
        foreach (Triangle triangleA in triangles)
        {
            //他の三角形と同じ頂点を3つ以上持っているかを調べている
            if (OverlapInTriangle(triangleA, triangles))
            {
                triCandidate.Add(triangleA);
            }
        }

        return triCandidate.ToArray();
    }

    //他の三角形と同じ頂点を3つ以上持っているか※同じ三角形からは1つの頂点まで
    private bool OverlapInTriangle(Triangle triangleBase, Triangle[] triangles)
    {
        int count = 2;
        foreach (Overlap overlapBase in triangleBase.overlaps)
        {
            foreach (Triangle triangleA in triangles)
            {
                if (triangleA == triangleBase)
                    continue;

                if (InIfOverlap(overlapBase, triangleA.overlaps))
                {
                    count--;

                    //3つ以上他の三角形と同じ頂点があるので追加
                    if (count == 0)
                        return true;

                    break;
                }
            }
        }

        return false;
    }

    //おなじ頂点を持っているか
    private bool InIfOverlap(Overlap overlapBase, Overlap[] overlaps)
    {
        foreach (Overlap overlap in overlaps)
        {
            if (overlap == overlapBase)
                return true;
        }
        return false;
    }

    //線から重なってる点へ変換
    private Overlap[] LineToOverlap(Line[] lines)
    {
        List<Overlap> overlaps = new List<Overlap>();
        for (int i = 0; i < lines.Length - 1; i++)
        {
            for (int j = i + 1; j < lines.Length; j++)
            {
                Vector2 overlap;
                if (!LineIfOverlap(lines[i], lines[j], out overlap))
                    continue;

                Overlap overlapClass = new Overlap(lines[i], lines[j], overlap);
                overlapClass.LineAddOverlap();
                overlaps.Add(overlapClass);
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

    private bool LineIfOverlaps(Line linea, Line[] lineb, int[] i)
    {
        for (int j = 0; j < i.Length; j++)
        {
            Vector2 vec2 = Vector2.zero;
            if (!LineIfOverlap(linea, lineb[i[j]], out vec2))
            {
                return false;
            }
        }
        return true;
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
    public Vec2Class[] vec2Classes;
    public Line(Vector2 vec2a, Vector2 vec2b)
    {
        overlaps = new List<Overlap>();
        vartex = new Vector2[2];
        vartex[0] = vec2a;
        vartex[1] = vec2b;
    }

    public Line(Vec2Class vec2A,Vec2Class vec2B)
    {
        vec2Classes = new Vec2Class[2];
        vec2Classes[0] = vec2A;
        vec2Classes[1] = vec2B;
        
        vartex = new Vector2[2];
        vartex[0] = vec2Classes[0].vec2;
        vartex[1] = vec2Classes[1].vec2;
    }

    public void AddOverlaps(Overlap overlap)
    {
        if (overlaps != null && overlaps.Count != 0)
        {
            if (overlaps.Contains(overlap))
                return;
        }

        overlaps.Add(overlap);
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

        Debug.Log("a");
        return 0;
    }

    public void LineAddOverlap()
    {
        lines[0].AddOverlaps(this);
        lines[1].AddOverlaps(this);
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

//星の情報
public class Star
{
    public Triangle[] triangles;
    public Line[] lines;

    public Overlap[] outSideOverlaps;
    public Overlap[] inSideOverlaps;
    public Overlap[] overlaps;
    public Star(Triangle[] triangles)
    {
        this.triangles = triangles;

        List<Line> lines = new List<Line>();
        foreach (Triangle triangle in triangles)
        {
            foreach (Line line in triangle.lines)
            {
                if (!InIfLine(line, lines.ToArray()))
                    lines.Add(line);
            }
        }
        this.lines = lines.ToArray();
    }

    public Star(Overlap[] outSideOverlaps, Overlap[] inSideOverlaps)
    {
        this.outSideOverlaps = outSideOverlaps;
        this.inSideOverlaps = inSideOverlaps;

        List<Overlap> overlaps = new List<Overlap>();
        overlaps.AddRange(outSideOverlaps);
        overlaps.AddRange(inSideOverlaps);
        this.overlaps = overlaps.ToArray();

        List<Line> lines = new List<Line>();
        foreach (Overlap overlap in this.overlaps)
        {
            foreach (Line line in overlap.lines)
            {
                if (!InIfLine(line, lines.ToArray()))
                    lines.Add(line);
            }
        }
        this.lines = lines.ToArray();
    }

    //同じ線を持っているか
    private bool InIfLine(Line lineBase, Line[] lines)
    {
        foreach (Line line in lines)
        {
            if (line == lineBase)
                return true;
        }
        return false;
    }
}
