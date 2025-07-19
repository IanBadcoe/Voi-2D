using Godot;

public class DelaunayTriangle : Triangle
{
    public readonly Circle CircumCircle;

    public DelaunayTriangle(Vector2 a, Vector2 b, Vector2 c) : base(a, b, c)
    {
        CircumCircle = CircumSolver.SolveCircle(a, b, c);
    }
}