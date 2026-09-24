using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

public static class NpcExecution
{
	public static void Step(ActorEntity body, NpcRoleDef role, NpcVerb verb, in NpcSenseSnapshot sense, float dt)
	{
		if (body == null) return;

		Vector3 wish = Vector3.Zero;
		switch (verb)
		{
			case NpcVerb.BackUp:
				wish = NpcMacroMicro.BackUpPoint(body) - body.GlobalPosition;
				break;
			case NpcVerb.GetOutOfView:
				wish = body.GlobalTransform.Basis.X;
				break;
			case NpcVerb.GoLook:
			case NpcVerb.MoveAndAttack:
			case NpcVerb.Enclose:
				if (sense.HasFanDest)
				{
					wish = sense.FanDest - body.GlobalPosition;
					break;
				}
				if (sense.HasCover)
				{
					var dCover = body.GlobalPosition.DistanceTo(sense.CoverPoint);
					if (dCover > 1.5f)
					{
						wish = sense.CoverPoint - body.GlobalPosition;
						break;
					}
				}
				if (sense.HasLastKnown)
					wish = sense.LastKnownTarget - body.GlobalPosition;
				break;
			default:
				break;
		}

		wish.Y = 0f;
		if (wish.LengthSquared() < 0.04f) return;

		var speed = role != null && role.SoftStep ? 3.2f : 4.5f;
		var step = wish.Normalized() * speed;
		body.Velocity = new Vector3(step.X, body.Velocity.Y, step.Z);
		body.MoveAndSlide();
		_ = dt;
	}
}
