using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Delaunay : IPolySource
{
    List<Triangle> Triangles = [];
    Rect2 Bounds;

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
        ) + new Vector2(1, 1);
        Vector2 min = point_list.Aggregate(
            point_list.First(),
            (x, y) => x.Min(y)
        ) - new Vector2(1, 1);

        // set the bounds a smidge larger than the extents of the point set...
        Bounds = new Rect2(min, max - min);

        // add two triangles to cover the whole area
        Triangles.Add(new Triangle(
            Bounds.Position,
            Bounds.End,
            new Vector2(Bounds.End.X, Bounds.Position.Y)
        ));

        Triangles.Add(new Triangle(
            Bounds.Position,
            Bounds.End,
            new Vector2(Bounds.Position.X, Bounds.End.Y)
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

    public void InsertPoint(Vector2 p)
    {
        Triangle found = null;

        foreach (var t in Triangles)
        {
            if (t.Contains(p))
            {
                found = t;
                break;
            }
        }

        if (found != null)
        {
            List<Triangle> ToCheck = [];

            Triangles.Remove(found);

            foreach (var l in found.GetLines())
            {
                Triangles.Add(new Triangle(l.Start, l.End, p));
            }

            // not finished, need to check and apply flips, but gone off on another tangent...
        }
    }
}