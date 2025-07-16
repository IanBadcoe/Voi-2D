using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Delaunay : IPolySource
{
    List<Triangle> Triangles = [];

    public Delaunay(IEnumerable<Vector2> points)
    {
        Build(points);
    }

    public void Build(IEnumerable<Vector2> points)
    {
        List<Vector2> point_list = points.ToList();

        Vector2 max = point_list.Aggregate(
            point_list.First(),
            (x, y) => x.Max(y)
        );
        Vector2 min = point_list.Aggregate(
            point_list.First(),
            (x, y) => x.Min(y)
        );

        Rect2 bounds = new Rect2((min + max) / 2, max - min);

        Triangles.Add(new Triangle(
            min,
            max,
            new Vector2(max.X, min.Y)
        ));

        Triangles.Add(new Triangle(
            max,
            min,
            new Vector2(min.X, max.Y)
        ));
    }

    public IEnumerable<IPolygon> GetPolys()
    {
        foreach (var t in Triangles)
        {
            yield return t;
        }
    }

    public int PolyCount()
    {
        return Triangles.Count;
    }
}