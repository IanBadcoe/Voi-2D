using Godot;
using System;
using System.Collections.Generic;

public interface IPolygon
{
	public abstract IEnumerable<Vector2> GetVerts(bool closed = false);
}
