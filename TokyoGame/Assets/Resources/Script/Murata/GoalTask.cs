using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoalTask : MonoBehaviour
{
    public List<LineRay> lineRays;
    Star star;
    Star starLog;
    GameObject colObject;
    StarTask starTask;
    // Start is called before the first frame update
    private void Awake()
    {
        colObject = new GameObject();
        colObject.tag = GetTag.Star;
        starTask = colObject.AddComponent<StarTask>();
        lineRays = new List<LineRay>();

        
    }

    private void Start()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LaunchBase");
        foreach (GameObject go in gameObjects)
        {
            lineRays.Add(go.GetComponent<LineRay>());
        }
        StartCoroutine(Colliderupdate());
    }

    //Updateの役割
    private IEnumerator Colliderupdate()
    {
        //エラー回避のため
        yield return null;

        starLog = null;
        while (true)
        {
            Line[] lines = LineListToLines(lineRays);
            star = LineToStar(lines);

            Debug.Log(star);
            //星があるかどうか
            if (star != null)
            {
                //新規生成の場合
                if (starLog == null)
                {
                    starTask.thisStarCol = CreateCol(star);
                }
                //座標が変更された場合
                else if (!IsNewStar(starLog, star))
                {
                    starTask.thisStarCol.points = ColliderVec2s(star);
                }
            }
            else
            {
                if (starTask.thisStarCol != null)
                    Destroy(starTask.thisStarCol);

                starTask.thisStarCol = null;
            }
            yield return null;
            starLog = star;
        }
    }

    private bool IsNewStar(Star oldStar, Star newStar)
    {
        Overlap[] oldOverlaps = oldStar.overlaps;

        List<Overlap> newOverlaps = new List<Overlap>();
        newOverlaps.AddRange(newStar.overlaps);

        foreach (Overlap overlap in oldOverlaps)
        {
            Overlap outOverlap;
            if (IsInOverlapPos(overlap, newOverlaps.ToArray(), out outOverlap))
            {
                newOverlaps.Remove(outOverlap);

                if (newOverlaps.Count == 0)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        return true;
    }

    private bool IsInOverlapPos(Overlap inOverlap, Overlap[] overlaps, out Overlap outOverlap)
    {
        foreach (Overlap overlap in overlaps)
        {
            if (inOverlap.pos == overlap.pos)
            {
                outOverlap = overlap;
                return true;
            }
        }
        outOverlap = null;
        return false;
    }

    private EdgeCollider2D CreateCol(Star star)
    {
        EdgeCollider2D edge2D = colObject.AddComponent<EdgeCollider2D>();

        edge2D.points = ColliderVec2s(star);
        edge2D.isTrigger = true;
        edge2D.edgeRadius = 0.45f;
        return edge2D;
    }

    private Vector2[] ColliderVec2s(Star star)
    {
        List<Vector2> vec2s = new List<Vector2>();
        Overlap baseOverlap = star.outSideOverlaps[0];
        Line baseLine = baseOverlap.lines[0];
        vec2s.Add(baseOverlap.pos);
        while (true)
        {
            foreach (Line line in star.lines)
            {
                if (line == baseLine ||
                    !InIfOverlap(line.overlaps.ToArray(), baseOverlap))
                    continue;
                foreach (Overlap overlap in line.overlaps)
                {
                    if (ContinueFlag(overlap,baseOverlap,star.outSideOverlaps,vec2s))
                        continue;

                    baseLine = line;
                    baseOverlap = overlap;
                    vec2s.Add(overlap.pos);
                    if (vec2s.Count == 5)
                    {
                        vec2s.Add(vec2s[0]);
                        return vec2s.ToArray();
                    }
                }
            }
        }
    }

    private bool ContinueFlag(Overlap overlap,Overlap baseOverlap,Overlap[] outSideOverlaps,List<Vector2> listVec2)
    {
        if (!InIfOverlap(star.outSideOverlaps, overlap))
            return true;

        if (baseOverlap == overlap)
            return true;

        if (listVec2.Contains(overlap.pos))
            return true;

        return false;
    }

    private bool InIfOverlap(Overlap[] overlaps, Overlap baseOverlap)
    {
        foreach (Overlap overlap in overlaps)
        {
            if (overlap == baseOverlap)
                return true;
        }
        return false;
    }

    public static GameObject CreateBlock(Vector2 pos)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefab/Stage/Block/Block"));
        go.transform.position = new Vector3(pos.x - 0.05f, pos.y - 0.05f, 0f);
        go.transform.localScale = Vector3.one * 0.5f;
        Destroy(go.GetComponent<BoxCollider2D>());

        return go;
    }

    private Line[] LineListToLines(List<LineRay> lineRays)
    {
        List<Line> lines = new List<Line>();
        foreach (LineRay lineRay in lineRays)
        {
            if (lineRay == null)
                continue;

            foreach (Line line in lineRay.keepLines)
            {
                lines.Add(line);
            }
        }

        return lines.ToArray();
    }

    private Star LineToStar(Line[] lines)
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
                            {
                                //Debug.Log("試行回数は" + q + "回");
                                return star;
                            }
                        }
                    }
                }
            }
        }
        //Debug.Log("試行回数は" + q + "回");
        return null;
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
            if (line.overlaps.Count < 2)
                return false;
            bool flag = line.overlaps[0].pos.x == line.overlaps[1].pos.x;
            //端っこの頂点を探す
            foreach (Overlap overlap in line.overlaps)
            {
                if (!flag)
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
                else
                {
                    if (min > overlap.pos.y)
                    {
                        minOverlap = overlap;
                        min = overlap.pos.y;
                    }
                    if (max < overlap.pos.y)
                    {
                        maxOverlap = overlap;
                        max = overlap.pos.y;
                    }
                }
            }
            //端っこと内側かどうかを調べ1つの頂点に対して両方（端と内)になれば星ではないのでfalse
            foreach (Overlap overlap in line.overlaps)
            {
                if (overlap == maxOverlap || overlap == minOverlap)
                {
                    if (inSideOverlap.Contains(overlap))
                        return false;

                    if (!outSideOverlap.Contains(overlap))
                        outSideOverlap.Add(overlap);
                }
                else
                {
                    if (outSideOverlap.Contains(overlap))
                        return false;

                    if (!inSideOverlap.Contains(overlap))
                        inSideOverlap.Add(overlap);
                }
            }
        }
        star = new Star(outSideOverlap.ToArray(), inSideOverlap.ToArray());
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

    //おなじ頂点を持っているか
    private bool InIfOverlap(Overlap overlapBase, Overlap[] overlaps)
    {
        foreach (Overlap overlap in overlaps)
        {
            if (overlap.pos == overlapBase.pos)
                return true;
        }
        return false;
    }

    //線から重なってる点へ変換
    private Overlap[] LineToOverlap(Line[] lines)
    {
        List<Overlap> overlaps = new List<Overlap>();
        //Lineの中の頂点情報を初期化する
        foreach (Line line in lines)
        {
            line.OverlapClear();
        }
        for (int i = 0; i < lines.Length - 1; i++)
        {
            for (int j = i + 1; j < lines.Length; j++)
            {
                Vector2 overlap;
                if (!LineIfOverlap(lines[i], lines[j], out overlap))
                    continue;

                if (OverlapIf(overlaps, overlap))
                    continue;

                Overlap overlapClass = new Overlap(lines[i], lines[j], overlap);
                overlapClass.LineAddOverlap();
                overlaps.Add(overlapClass);
            }
        }

        return overlaps.ToArray();
    }

    //頂点が同じ座標かどうか
    private bool OverlapIf(List<Overlap> overlaps, Vector2 vec2)
    {
        foreach (Overlap overlap in overlaps)
        {
            if (vec2 == overlap.pos)
            {
                return true;
            }
        }
        return false;
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
    private Line[] rayVartexToLines(List<List<Vec2Class>> vec2)
    {
        List<Line> lines = new List<Line>();
        for (int i = 0; i < vec2.Count; i++)
        {
            for (int j = 0; i < vec2[i].Count; j++)
            {
                //直線情報を追加していく
                lines.Add(new Line(vec2[i][j].vec2, vec2[i][j + 1].vec2));
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

    public void OverlapClear()
    {
        overlaps = new List<Overlap>();
    }

    public void AddOverlaps(Overlap overlap)
    {
        if (overlaps != null && overlaps.Count != 0)
        {
            foreach (Overlap overlapA in overlaps)
            {
                if (overlapA.pos == overlap.pos)
                    return;
            }
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
        
        return 0;
    }

    public void LineAddOverlap()
    {
        foreach (Line line in lines)
        {
            line.AddOverlaps(this);
        }
    }
}

//星の情報
public class Star
{
    public Line[] lines;

    public Overlap[] outSideOverlaps;
    public Overlap[] inSideOverlaps;
    public Overlap[] overlaps;

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
