using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>Turns a role verb into a generic move. Pathing is not this layer.</summary>
public static class NpcMacroMicro
{
	public static NpcVerb Rewrite(NpcVerb asked, in NpcSenseSnapshot sense, ActorEntity body)
	{
		if (asked == NpcVerb.BackUp && sense.WorldHazardAhead)
			return NpcVerb.GetOutOfView;

		if (asked == NpcVerb.MoveAndAttack && sense.WorldHazardAhead)
			return NpcVerb.ShootWorld;

		if (asked == NpcVerb.None)
			return NpcVerb.Hold;

		_ = body;
		return asked;
	}

	public static Vector3 BackUpPoint(ActorEntity body, float meters = 4f)
	{
		if (body == null) return Vector3.Zero;
		return body.GlobalPosition - body.GlobalTransform.Basis.Z * meters;
	}
}
