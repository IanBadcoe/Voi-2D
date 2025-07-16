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
}
