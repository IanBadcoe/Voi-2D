using System;
using System.Diagnostics;
using Godot;

public class Line
{
    public readonly Vector2 Start;
    public readonly Vector2 End;

    public Line(Vector2 start,
                Vector2 end)
    {
        Start = start;
        End = end;
    }

    public Vector2 Vector
    {
        get
        {
            return End - Start;
        }
    }

    public Vector2 Direction
    {
        get
        {
            return Vector.Normalized();
        }
    }

    public Vector2 Normal
    {
        get
        {
            Vector2 dir = Direction;
            // rotate 90 degrees from direction
            // (whether we call this clockwise depends on how we picture the coordinate system)
            return new Vector2(dir.Y, -dir.X);
        }
    }

    public void DrawOn(Node2D target, Color color, bool show_arrow = false)
    {
        DrawUtils.DrawLine(target, Start, End, color, show_arrow);
    }

    public Vector2 CommonPoint(Line other)
    {
        if (Start == other.Start || Start == other.End)
        {
            return Start;
        }

        Debug.Assert(End == other.End || End == other.Start);

        return End;
    }

    public Vector2 OtherPoint(Vector2 point)
    {
        if (point == Start)
        {
            return End;
        }

        Debug.Assert(point == End);

        return Start;
    }
}