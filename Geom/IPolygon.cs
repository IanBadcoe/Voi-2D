using Godot;
using System;
using System.Collections.Generic;

public interface IPolygon
{
	IEnumerable<Vector2> GetVerts(bool closed = false);
	IEnumerable<Line> GetLines();
}
