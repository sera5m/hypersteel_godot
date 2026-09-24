using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

public enum NpcWorldKind
{
	Ammo = 0,
	Health,
	Stim,
	Battery,
	Turret,
}

/// <summary>Ambient props. Groups: world_pickup (meta kind), world_turret.</summary>
public static class NpcWorldUse
{
	public static Node3D Nearest(Node from, NpcWorldKind kind, float radius = 18f)
	{
		if (from == null) return null;
		var group = kind == NpcWorldKind.Turret ? "world_turret" : "world_pickup";
		Node3D best = null;
		var bestD = radius;
		var origin = from is Node3D n3 ? n3.GlobalPosition : Vector3.Zero;
		foreach (var n in from.GetTree().GetNodesInGroup(group))
		{
			if (n is not Node3D p) continue;
			if (kind != NpcWorldKind.Turret)
			{
				var tag = p.HasMeta("kind") ? p.GetMeta("kind").ToString() : "";
				if (!tag.Equals(kind.ToString(), System.StringComparison.OrdinalIgnoreCase))
					continue;
			}
			var d = origin.DistanceTo(p.GlobalPosition);
			if (d >= bestD) continue;
			bestD = d;
			best = p;
		}
		return best;
	}
}
