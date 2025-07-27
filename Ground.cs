using Godot;
using System;
using System.Collections.Generic;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

[Meta(typeof(IAutoConnect))]
public partial class Ground : StaticBody2D, PGBase
{
	// --------------------------------------------------------------
	// IAutoNode boilerplate
	public override void _Notification(int what) => this.Notify(what);
	// --------------------------------------------------------------

	// ------
	// PGBase

	[Node]
	public CollisionShape2D Collision { get; set; }

	[Node]
	public Polygon2D Visual { get; set; }

	List<PGBase> Branches = [];

	public IEnumerable<PGBase> GetBranches()
	{
		foreach (var b in Branches)
		{
			yield return b;
		}
	}

	public void AddBranch(PGBase branch)
	{
		foreach (var b in Branches)
		{
			(b as PhysicsBody2D).AddCollisionExceptionWith(branch as PhysicsBody2D);
		}

		Branches.Add(branch);
	}

	public void RemoveBranch(PGBase branch)
	{
		if (Branches.Remove(branch))
		{
			foreach (var b in Branches)
			{
				(b as PhysicsBody2D).RemoveCollisionExceptionWith(branch as PhysicsBody2D);
			}
		}
	}

	// ------
}
