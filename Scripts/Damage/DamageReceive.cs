using Godot;

namespace Hypersteel.Damage;

/// <summary>Shared soak + bounce. Used by ArmorPiece and ActorEntity.</summary>
public static class DamageReceive
{
	public struct Soak
	{
		public ApOutcome Outcome;
		public float TakenByCover;
		public float Through;
		public Vector3 BounceDir;
		public bool Stopped;
	}

	public static Soak Armor(in DamagePacket packet, int armorLevel, float plateHp, Vector3 comAxis, Vector3 hitPoint, Vector3 incomingDir)
	{
		var soak = new Soak();
		soak.Outcome = ArmorLadder.Outcome(packet.Ap, armorLevel, packet.KineticFlags);

		if ((packet.KineticFlags & KineticFlags.Incendiary) != 0 && armorLevel > packet.Ap)
		{
			soak.TakenByCover = Math.Min(plateHp, packet.Kinetic * 0.15f);
			soak.Through = packet.Kinetic * 0.15f;
			soak.Stopped = false;
			return soak;
		}

		float mul = ArmorLadder.KineticMul(soak.Outcome, packet.Ap, armorLevel);
		float incoming = packet.Kinetic * mul;

		switch (soak.Outcome)
		{
			case ApOutcome.Null:
				soak.TakenByCover = 0f;
				soak.Through = 0f;
				soak.Stopped = true;
				soak.BounceDir = BounceDirection(comAxis, hitPoint, incomingDir);
				break;
			case ApOutcome.Bounce:
				soak.TakenByCover = Math.Min(plateHp, incoming);
				soak.Through = incoming;
				soak.Stopped = false;
				soak.BounceDir = BounceDirection(comAxis, hitPoint, incomingDir);
				break;
			case ApOutcome.Cavitation:
				soak.TakenByCover = plateHp;
				soak.Through = incoming;
				soak.Stopped = false;
				break;
			default:
				float eat = Math.Min(plateHp, incoming);
				soak.TakenByCover = eat;
				soak.Through = Math.Max(0f, incoming - eat * 0.5f);
				if (soak.Outcome == ApOutcome.Overpen)
					soak.Through = incoming;
				soak.Stopped = soak.Through <= 0.001f;
				break;
		}

		return soak;
	}

	public static Vector3 BounceDirection(Vector3 comAxis, Vector3 hitPoint, Vector3 incomingDir)
	{
		Vector3 axis = comAxis.LengthSquared() > 0.0001f ? comAxis.Normalized() : Vector3.Up;
		Vector3 incoming = incomingDir.LengthSquared() > 0.0001f ? incomingDir.Normalized() : -axis;
		Vector3 n = (hitPoint - Vector3.Zero);
		if (n.LengthSquared() < 0.0001f)
			n = axis;
		else
			n = n.Normalized();
		// Flatten against the plate's long axis so bounce is across the panel, not into the wearer.
		n = (n - axis * n.Dot(axis)).Normalized();
		if (n.LengthSquared() < 0.0001f)
			n = incoming.Cross(axis).Normalized();
		if (n.LengthSquared() < 0.0001f)
			n = Vector3.Forward;
		return incoming.Bounce(n);
	}
}
