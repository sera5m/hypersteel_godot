using Godot;
using System.Collections.Generic;

namespace Hypersteel.Entity.NpcAI.Brain;

public struct NpcSquadmate
{
	public ActorEntity Body;
	public int Rank;
	public float OccupantHp01;
	public float Plate01;
}

/// <summary>Off unless Role.HasTeamComms. No mesh. Rank: higher obeys.</summary>
public sealed class NpcTeamComms
{
	public readonly List<NpcSquadmate> Squad = new();

	public void Refresh(ActorEntity self, int myRank)
	{
		Squad.Clear();
		if (self == null) return;
		foreach (var n in self.GetTree().GetNodesInGroup("npc_squad"))
		{
			if (n is not ActorEntity other || other == self || other.IsDead) continue;
			var brain = other.GetNodeOrNull<NpcBrain>("NpcBrain");
			var rank = brain?.Stats != null ? brain.Stats.Rank : 0;
			var hp = 1f;
			if (other.health?.State != null)
				hp = other.health.State.HpOf(Hypersteel.Health.BodySegment.Torso) / 100f;
			Squad.Add(new NpcSquadmate { Body = other, Rank = rank, OccupantHp01 = hp });
		}
		_ = myRank;
	}

	public ActorEntity DyingAlly()
	{
		foreach (var m in Squad)
			if (m.OccupantHp01 > 0f && m.OccupantHp01 < 0.25f) return m.Body;
		return null;
	}

	public bool ShouldObey(int myRank, int orderRank) => orderRank > myRank;
}
