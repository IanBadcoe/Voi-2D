using System;
using Godot;

public static class CircumSolver
{
    public static Circle SolveCircle(Vector2 M, Vector2 N, Vector2 O)
    {
        // our circle is represented by three simultaneous equations
        // of the form:
        // (Cx - Px)^2 + (Cy - Py)^2 = R^2
        // (where "P" is any one of our points, m, n and o)
        // in a general form:
        // Cx^2 - 2PxCx + Px^2 + Cy^2 - 2PyCy + Py^2 - R^2 = 0
        // and if we rearrange such that:
        // H = -Cx
        // I = -Cy
        // J = R^2 + Cx^2 + Cy^2
        // we can get:
        // 2PxH + 2PyI + J = -(Px^2 + Py^2)
        // and we make a system of three simultaneous equations from that for the three points, M, N and O:
        // 2MxH + 2MyI + J = -(Mx^2 + My^2)
        // 2NxH + 2NyI + J = -(Nx^2 + Ny^2)
        // 2OxH + 2OyI + J = -(Ox^2 + Oy^2)
        // and solve that using matrices (if the points are col-linear there is no solution and the equations won't solve...)

        // as matrices we have
        // Coeffs * Unknowns = Constants:
        //
        // [2Mx, 2My, 1]   [H]   [-(Mx^2 + My^2)]
        // [2Nx, 2Ny, 1] * [I] = [-(Nx^2 + Ny^2)]
        // [2Ox, 2Oy, 1]   [J]   [-(Ox^2 + Oy^2)]
        // so let's build that

        double[,] coeffs = new double[,]
        {
            { 2 * M.X, 2 * M.Y, 1},
            { 2 * N.X, 2 * N.Y, 1},
            { 2 * O.X, 2 * O.Y, 1},
        };
        double[] consts = new double[]
        {
            -(M.X * M.X + M.Y * M.Y),
            -(N.X * N.X + N.Y * N.Y),
            -(O.X * O.X + O.Y * O.Y),
        };

        double[] result = SimultaneousEqns.Solve(coeffs, consts);

        if (result == null)
        {
            return null;
        }

        double Cx = -result[0];
        double Cy = -result[1];

        double Dx = M.X - Cx;
        double Dy = M.Y - Cy;

        return new Circle(new Vector2((float)Cx, (float)Cy), (float)Math.Sqrt(Dx * Dx + Dy * Dy));
    }
}