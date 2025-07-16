using Godot;
using System;
using System.Collections.Generic;

public class Triangle : IPolygon
{
    Vector2 V1;
    Vector2 V2;
    Vector2 V3;

    public Triangle()
    {
    }

    public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        V1 = v1;
        V2 = v2;
        V3 = v3;
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
}
