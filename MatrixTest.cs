using Godot;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MatrixTest : Node2D
{
	bool Dirty = false;
	bool Update = false;
	bool Released = true;

	Vector2 N;
	Vector2 M;
	Vector2 O;

	Circle Circle;

	Random Random = new Random();

	public override void _Ready()
	{
		double[,] coeffs = {
			{1, 2},
			{3, 4},
		};

		double[] consts = {
			3, 7
		};

		double[] ret = SimultaneousEqns.Solve(coeffs, consts);
	}

	public override void _Process(double _delta)
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
				Update = true;
				Released = false;
			}
		}
		else
		{
			Released = true;
		}

		if (Update)
		{
			Update = false;

			N = new Vector2((float)Random.NextDouble() * 500 + 10, (float)Random.NextDouble() * 500 + 10);
			M = new Vector2((float)Random.NextDouble() * 500 + 10, (float)Random.NextDouble() * 500 + 10);
			O = new Vector2((float)Random.NextDouble() * 500 + 10, (float)Random.NextDouble() * 500 + 10);

			Circle = CircumSolver.SolveCircle(N, M, O);

			Dirty = true;
		}
	}

	public override void _Draw()
	{
		Vector2 off1 = new Vector2(10, 10);
		Vector2 off2 = new Vector2(10, -10);

		DrawLine(N + off1, N - off1, Colors.Red);
		DrawLine(N + off2, N - off2, Colors.Red);

		DrawLine(M + off1, M - off1, Colors.Green);
		DrawLine(M + off2, M - off2, Colors.Green);

		DrawLine(O + off1, O - off1, Colors.Blue);
		DrawLine(O + off2, O - off2, Colors.Blue);

		if (Circle != null)
		{
			DrawCircle(Circle);
		}
	}

	private void DrawCircle(Circle circle)
	{
		List<Vector2> points = [];

		for (float a = 0; a < Mathf.Pi * 2; a += 0.1f)
		{
			Vector2 offset = new Vector2(Mathf.Sin(a), Mathf.Cos(a)) * circle.Radius;

			points.Add(circle.Position + offset);
		}

		points.Add(points.First());

		DrawPolyline(points.ToArray(), Colors.White);
	}

}
