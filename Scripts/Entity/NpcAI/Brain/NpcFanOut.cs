using Godot;
using System.Collections.Generic;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>
/// Lateral slots so the squad does not stack on one point.
/// Slot is stable (instance id sort). Spacing along right of approach axis.
/// </summary>
public static class NpcFanOut
{
	public const float Spacing = 2.2f;
	public const int MaxSlots = 5;

	public static void Apply(ActorEntity self, ref NpcSenseSnapshot sense)
	{
		if (self == null || !sense.HasLastKnown) return;

		var axis = sense.LastKnownTarget - self.GlobalPosition;
		axis.Y = 0f;
		if (axis.LengthSquared() < 0.04f) return;
		axis = axis.Normalized();
		var right = axis.Cross(Vector3.Up);
		if (right.LengthSquared() < 0.01f) right = self.GlobalTransform.Basis.X;
		right = right.Normalized();

		var ids = new List<ulong>();
		ids.Add(self.GetInstanceId());
		var tree = self.GetTree();
		if (tree != null)
		{
			foreach (var n in tree.GetNodesInGroup("npc_squad"))
			{
				if (n is not ActorEntity other || other == self || other.IsDead) continue;
				ids.Add(other.GetInstanceId());
			}
		}
		ids.Sort();
		if (ids.Count > MaxSlots) ids.RemoveRange(MaxSlots, ids.Count - MaxSlots);

		var slot = ids.IndexOf(self.GetInstanceId());
		if (slot < 0) slot = 0;
		var centered = slot - (ids.Count - 1) * 0.5f;

		var home = sense.HasCover ? sense.CoverPoint : sense.LastKnownTarget;
		sense.FanDest = home + right * centered * Spacing;
		sense.HasFanDest = true;
	}
}
