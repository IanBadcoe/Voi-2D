using Godot;

public class Ray
{
    // normal points out...
    public readonly Vector2 Normal;

    public readonly float Distance;

    public Ray(Vector2 normal, float dist)
    {
        Normal = normal;
        Distance = dist;
    }

    public enum ClipResult
    {
        Unchanged,
        ClippedStart,       // we kept the original end, but moved the start
        ClippedEnd,         // we kept the original start, but moved the end
        Discarded
    }

    public (ClipResult, Line) ClipLine(Line line)
    {
        float start_proj = line.Start.Dot(Normal) - Distance;
        float end_proj = line.End.Dot(Normal) - Distance;

        if (start_proj > 0)
        {
            if (end_proj > 0)
            {
                // start and end both out
                return (ClipResult.Discarded, null);
            }

            // not right
            return (ClipResult.ClippedStart, new Line(line.Start + line.Vector * start_proj / (start_proj - end_proj), line.End));
        }
        else
        {
            if (end_proj > 0)
            {
                return (ClipResult.ClippedEnd, new Line(line.Start, line.End + line.Vector * end_proj / (start_proj - end_proj)));
            }

            // start and end both in
            return (ClipResult.Unchanged, line);
        }
    }

    public bool IsInside(Vector2 p)
    {
        return p.Dot(Normal) <= Distance;
    }

    public Ray Reversed()
    {
        return new(-Normal, -Distance);
    }
}