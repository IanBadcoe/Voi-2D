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

    public Vector2 Direction
    {
        get
        {
            return (End - Start).Normalized();
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
        target.DrawLine(Start, End, color);

        if (show_arrow)
        {
            target.DrawColoredPolygon(new Vector2[] {
                End,
                End - Direction * 20 + Normal * 10,
                End - Direction * 20 - Normal * 10
            }, color);
        }
    }
}