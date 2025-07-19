using Godot;

public static class PointUtils
{
    public static (Vector2, Vector2, Vector2) SortPointsForWinding(Vector2 a, Vector2 b, Vector2 c)
    {
        if (IsInsideLine(a, b, c))
        {
            return (a, b, c);
        }

        return (b, a, c);
    }

    public static bool IsInsideLine(Vector2 l1, Vector2 l2, Vector2 p)
    {
        return IsInsideLine(new Line(l1, l2), p);
    }

    public static bool IsInsideLine(Line line, Vector2 p)
    {
        return (p - line.Start).Dot(line.Normal) > 0;
    }
}