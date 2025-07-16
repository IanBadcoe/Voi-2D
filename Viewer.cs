using Godot;
using System;
using System.Linq;

public partial class Viewer : Node2D
{
	bool Dirty = true;

	IPolySource Source;

	public Viewer()
	{
		Source = new Delaunay([new Vector2(10, 10), new Vector2(510, 510)]);
	}

	public override void _Process(double delta)
	{
		if (Dirty)
		{
			Dirty = false;
			QueueRedraw();
		}
	}

	public override void _Draw()
	{
		foreach (var t in Source.GetPolys())
		{
			DrawPolyline(t.GetVerts(true).ToArray(), Colors.Red);
		}
	}
}
