using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Viewer : Node2D
{
	bool Dirty = true;
	bool Pressed;
	bool Released = true;

	VoironoiPlaned2D Source;

	Random Random = new Random();

	public Viewer()
	{
		Source = new VoironoiPlaned2D(new Rect2(new(10, 10), new(510, 510)));
		Source.AddPoint(new(100, 100));
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (Dirty)
		{
			Dirty = false;
			QueueRedraw();
		}

		if (Input.IsPhysicalKeyPressed(Key.Space))
		{
			if (Released)
			{
				Pressed = true;
				Released = false;
			}
		}
		else
		{
			Released = true;
		}

		if (Pressed)
		{
			Pressed = false;

			Source.AddPoint(new Vector2((float)(Random.NextDouble() * 500 + 10), (float)(Random.NextDouble() * 500 + 10)));

			QueueRedraw();
		}
	}

	public override void _Draw()
	{
        foreach (var c in Source.Cells)
        {
            DrawUtils.DrawPoint(this, c.Position, Colors.Green);
        };

		foreach (var t in Source.GetPolys())
            {
                List<Vector2> points = t.GetLines().Select(x => x.End).ToList();

                // var centre = points.Aggregate(Vector2.Zero, (x, y) => x + y);
                // centre /= points.Count;

                // Vector2 step_in(Vector2 p) { return p + (centre - p).Normalized() * 10; }

                // points = points.Select(x => step_in(x)).ToList();
                var prev_point = points.Last();

                foreach (var point in points)
                {
                    DrawUtils.DrawLine(this, prev_point, point, Colors.Red, false);
                    prev_point = point;
                }
            }
	}
}
