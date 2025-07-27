using Godot;
using System;

using System.Collections.Generic;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;

[Meta(typeof(IAutoConnect))]
public partial class PGBranch : RigidBody2D, PGBase
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

	CapsuleShape2D Shape
	{
		get
		{
			return Collision.Shape as CapsuleShape2D;
		}
	}

	bool Dirty = true;
	float LengthInner = 100;
	float RadiusInner = 20;

	public float Length
	{
		get
		{
			return LengthInner;
		}
		set
		{
			LengthInner = value;
			Dirty = true;
		}
	}

	public float Radius
	{
		get
		{
			return RadiusInner;
		}
		set
		{
			RadiusInner = value;
			Dirty = true;
		}
	}

	// world coordinates
	//
	// untransformed, our endpoint is Lenth up the Y axis
	public Vector2 EndPoint
	{
		get
		{
			return GetGlobalTransform() * new Vector2(0, -Length);
		}
	}

	static PackedScene Scene = ResourceLoader.Load("res://PGrow/PGBranch.tscn") as PackedScene;

	public override void _Ready()
	{
	}

	public override void _Process(double _delta)
	{
		if (Dirty)
		{
			Dirty = false;

			Shape.Height = Length + Radius * 2;
			Shape.Radius = Radius;

            Collision.Position = new Vector2(0, -Length / 2);
            Visual.Position = new Vector2(0, -Length / 2);

			GeneratePoly();
		}
	}


	private void GeneratePoly()
	{
		CapsuleShape2D capsule = Collision.Shape as CapsuleShape2D;

		List<Vector2> points = [];

		Vector2 offset = new(0, Length / 2);

		for (int i = 0; i < 40; i++)
		{
			if (i == 20)
			{
				offset = -offset;
			}

			float angle = (float)i / 40 * 2 * Mathf.Pi;
			points.Add(new Vector2(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius) + offset);
		}

		Visual.Polygon = [.. points];
	}

	public static PGBranch CreateOn(PGBranch parent, float rotation, float length, float radius)
	{
		return CreateOn(parent, parent.EndPoint, rotation, length, radius);
	}

	public static PGBranch CreateOn(PGBase parent, Vector2 location, float rotation, float length, float radius)
	{
		PGBranch ret = Scene.Instantiate<PGBranch>();
		ret.Length = length;
		ret.Radius = radius;

		Node parent_node = parent as Node;

		// location is world location, so set parent first, so we don't get the parent's transform wrongly
		// applied twice
		parent_node.AddChild(ret);
		ret.Rotation = rotation;
		ret.GlobalPosition = location;

		PinJoint2D joint = new PinJoint2D();
		parent_node.AddChild(joint);
		joint.GlobalPosition = location;
		joint.Rotation = rotation;
		joint.NodeA = parent_node.GetPath();
		joint.NodeB = ret.GetPath();
		joint.AngularLimitEnabled = true;
		joint.AngularLimitLower = rotation - 0.1f;
		joint.AngularLimitUpper = rotation + 0.1f;
        joint.

		parent.AddBranch(ret);

		return ret;
	}
}
