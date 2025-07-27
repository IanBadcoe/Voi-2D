using System.Collections;
using System.Collections.Generic;
using Godot;

public interface PGBase
{
	CollisionShape2D Collision { get; set; }

	Polygon2D Visual { get; set; }

	IEnumerable<PGBase> GetBranches();

	void AddBranch(PGBase branch);

	// probably not required if the branch is being deleted (QueueFree'd)
	void RemoveBranch(PGBase branch);
}
