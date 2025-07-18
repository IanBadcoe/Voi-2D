using Godot;

public class Circle
{
    public readonly Vector2 Position;
    public readonly float Radius;

    public Circle(Vector2 pos, float rad)
    {
        Position = pos;
        Radius = rad;
    }

    public bool Contains(Vector2 point)
    {
        float dist2 = Position.DistanceSquaredTo(point);

        return dist2 <= Radius * Radius;
    }
}