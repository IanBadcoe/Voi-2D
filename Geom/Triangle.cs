using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Triangle : IPolygon
{
    Vector2 V1;
    Vector2 V2;
    Vector2 V3;

    public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        (V1, V2, V3) = PointUtils.SortPointsForWinding(v1, v2, v3);
    }

    public IEnumerable<Vector2> GetVerts(bool closed = false)
    {
        yield return V1;
        yield return V2;
        yield return V3;

        if (closed)
        {
            yield return V1;
        }
    }

    public IEnumerable<Line> GetLines()
    {
        yield return new Line(V1, V2);
        yield return new Line(V2, V3);
        yield return new Line(V3, V1);
    }

    public bool Contains(Vector2 p)
    {
        foreach (var l in GetLines())
        {
            if (!PointUtils.IsInsideLine(l, p))
            {
                return false;
            }
        }

        return true;
    }
}
