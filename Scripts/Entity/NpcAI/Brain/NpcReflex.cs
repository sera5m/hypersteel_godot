using Godot;
using Hypersteel.Abilities.Kits;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>
/// Interrupt. Never planned in the tree.
/// Notes the fire *after* it happens, not before or during.
/// </summary>
public sealed class NpcReflex
{
	public NpcReflexKind LastFired { get; private set; }
	public ulong LastFiredMsec { get; private set; }

	public bool TryNadeLeap(ActorEntity body, Vector3 away)
	{
		if (body == null) return false;

		var flat = away;
		flat.Y = 0f;
		if (flat.LengthSquared() < 0.01f)
			flat = -body.GlobalTransform.Basis.Z;
		flat = flat.Normalized();

		bool pulsed = false;
		if (body.jumpKit != null)
			pulsed = body.jumpKit.TryPulse(JumpVerb.Dash, flat);

		if (!pulsed)
			body.Velocity += flat * 6f + Vector3.Up * 3f;

		Note(NpcReflexKind.NadeLeap);
		return true;
	}

	void Note(NpcReflexKind kind)
	{
		LastFired = kind;
		LastFiredMsec = Time.GetTicksMsec();
		GD.Print($"[npc-reflex] {kind} t={LastFiredMsec}");
	}
}
