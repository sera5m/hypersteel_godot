using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>
/// Trap / door portal stub. Door/exit = nav portal. PlaceAtPortal is interest only —
/// no munition spawn, no friendly-step logic yet. Own traps later: team id, step-over.
/// Int gate lives on caller (Stats / Role).
/// </summary>
public enum NpcTrapKind
{
	None = 0,
	StickMunition,
	CutReel,
	DropBlocker,
	RaiseCover,
	DoorPortal,
}

public struct NpcPortalMark
{
	public Vector3 Point;
	public NpcTrapKind Kind;
	public bool Valid;
}

/// <summary>Static helpers. Group: nav_portal (meta kind optional).</summary>
public static class NpcTrapPortal
{
	public const string PortalGroup = "nav_portal";

	/// <summary>Nearest portal node in radius. Null if none.</summary>
	public static Node3D NearestPortal(Node from, float radius = 12f)
	{
		if (from == null) return null;
		Node3D best = null;
		var bestD = radius;
		var origin = from is Node3D n3 ? n3.GlobalPosition : Vector3.Zero;
		foreach (var n in from.GetTree().GetNodesInGroup(PortalGroup))
		{
			if (n is not Node3D p) continue;
			var d = origin.DistanceTo(p.GlobalPosition);
			if (d >= bestD) continue;
			bestD = d;
			best = p;
		}
		return best;
	}

	/// <summary>
	/// Stub place: records a mark at the portal. Does not spawn, does not arm.
	/// Returns false if no portal or kind is None.
	/// </summary>
	public static bool PlaceAtPortal(ActorEntity body, NpcTrapKind kind, out NpcPortalMark mark)
	{
		mark = default;
		if (body == null || kind == NpcTrapKind.None) return false;
		var portal = NearestPortal(body);
		if (portal == null) return false;
		mark = new NpcPortalMark
		{
			Point = portal.GlobalPosition,
			Kind = kind,
			Valid = true,
		};
		GD.Print($"[npc-trap-stub] PlaceAtPortal {kind} @ {mark.Point}");
		return true;
	}

	/// <summary>Sense crumb: true if a portal exists in radius (for later wait / bounce-guess).</summary>
	public static bool HasPortalNear(ActorEntity body, float radius = 12f) =>
		body != null && NearestPortal(body, radius) != null;
}
