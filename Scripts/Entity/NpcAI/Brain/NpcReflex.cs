using Godot;
using Hypersteel.Abilities.Kits;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>Interrupt. Notes the fire after it happens.</summary>
public sealed class NpcReflex
{
	public NpcReflexKind LastFired { get; private set; }
	public ulong LastFiredMsec { get; private set; }

	const ulong RollLockMs = 800;

	public bool TryNadeLeap(ActorEntity body, Vector3 away)
	{
		if (body == null) return false;
		Impulse(body, Flat(away, body), 6f, 3f, JumpVerb.Dash);
		Note(NpcReflexKind.NadeLeap);
		return true;
	}

	public bool TryHitRoll(ActorEntity body, Vector3 fromThreat)
	{
		if (body == null) return false;
		if (LastFired == NpcReflexKind.Roll && Time.GetTicksMsec() - LastFiredMsec < RollLockMs)
			return false;

		var away = Flat(body.GlobalPosition - fromThreat, body);
		var side = away.Cross(Vector3.Up);
		if (side.LengthSquared() < 0.01f) side = body.GlobalTransform.Basis.X;
		side = side.Normalized();
		if ((Time.GetTicksMsec() & 1) == 1) side = -side;

		Impulse(body, side, 8f, 1.6f, JumpVerb.Dash);
		Note(NpcReflexKind.Roll);
		return true;
	}

	static Vector3 Flat(Vector3 v, ActorEntity body)
	{
		v.Y = 0f;
		if (v.LengthSquared() < 0.01f) v = -body.GlobalTransform.Basis.Z;
		return v.Normalized();
	}

	static void Impulse(ActorEntity body, Vector3 dir, float along, float up, JumpVerb verb)
	{
		var pulsed = false;
		if (body.jumpKit != null)
			pulsed = body.jumpKit.TryPulse(verb, dir);
		if (!pulsed)
			body.Velocity += dir * along + Vector3.Up * up;
	}

	void Note(NpcReflexKind kind)
	{
		LastFired = kind;
		LastFiredMsec = Time.GetTicksMsec();
		GD.Print($"[npc-reflex] {kind} t={LastFiredMsec}");
	}
}
