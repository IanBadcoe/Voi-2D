using Godot;
using System;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

[Meta(typeof(IAutoConnect))]
public partial class PhysApproachTest : Node2D
{
	// --------------------------------------------------------------
	// IAutoNode boilerplate
	public override void _Notification(int what) => this.Notify(what);
	// --------------------------------------------------------------

	[Node]
	PGBase Ground { get; set; }

	public override void _Ready()
	{
		var trunk = PGBranch.CreateOn(Ground, new Vector2(0, -40), 0f, 120, 40);
		var b1 = PGBranch.CreateOn(trunk, Mathf.Pi / 4, 90, 30);
		var b2 = PGBranch.CreateOn(trunk, -Mathf.Pi / 4, 90, 30);
		var b11 = PGBranch.CreateOn(b1, Mathf.Pi / 4, 70, 20);
		var b12 = PGBranch.CreateOn(b1, -Mathf.Pi / 4, 70, 20);
		var b21 = PGBranch.CreateOn(b2, Mathf.Pi / 4, 70, 20);
		var b22 = PGBranch.CreateOn(b2, -Mathf.Pi / 4, 70, 20);
		PGBranch.CreateOn(b11, Mathf.Pi / 4, 50, 10);
		PGBranch.CreateOn(b11, -Mathf.Pi / 4, 50, 10);
		PGBranch.CreateOn(b12, Mathf.Pi / 4, 50, 10);
		PGBranch.CreateOn(b12, -Mathf.Pi / 4, 50, 10);
		PGBranch.CreateOn(b21, Mathf.Pi / 4, 50, 10);
		PGBranch.CreateOn(b21, -Mathf.Pi / 4, 50, 10);
		PGBranch.CreateOn(b22, Mathf.Pi / 4, 50, 10);
		PGBranch.CreateOn(b22, -Mathf.Pi / 4, 50, 10);
	}
}
