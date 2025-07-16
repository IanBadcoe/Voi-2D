using Godot;
using System;
using System.Collections.Generic;

public interface IPolySource
{
    public int PolyCount();
    public IEnumerable<IPolygon> GetPolys();
}
