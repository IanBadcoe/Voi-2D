using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Polygon : IPolygon
{
    List<Vector2> Verts = [];

    public IEnumerable<Vector2> GetVerts(bool closed = false)
    {
        foreach (var v in Verts)
        {
            yield return v;
        }

        if (closed)
        {
            yield return Verts.First();
        }
    }

    public IEnumerable<Line> GetLines()
    {
        Vector2 prev = Verts.Last();

        foreach (var curr in Verts)
        {
            yield return new Line(prev, curr);

            prev = curr;
        }
    }
}
