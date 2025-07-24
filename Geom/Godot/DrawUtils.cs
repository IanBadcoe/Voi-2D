using Godot;

public static class DrawUtils
{
    public static void DrawLine(Node2D target, Vector2 start, Vector2 end, Color color, bool show_arrow)
    {
        target.DrawLine(start, end, color);

        if (show_arrow)
        {
            Vector2 direction = (end - start).Normalized();
            Vector2 normal = new Vector2(direction.Y, -direction.X);

            target.DrawColoredPolygon(new Vector2[] {
                end,
                end - direction * 20 + normal * 10,
                end - direction * 20 - normal * 10
            }, color);
        }
    }

    public static void DrawPoint(Node2D target, Vector2 p, Color color)
    {
        Vector2 offset1 = new Vector2(10, 10);
        Vector2 offset2 = new Vector2(10, -10);

        target.DrawLine(p + offset1, p - offset1, color);
        target.DrawLine(p + offset2, p - offset2, color);
    }
}