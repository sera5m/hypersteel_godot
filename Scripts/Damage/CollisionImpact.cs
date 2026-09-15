using Godot;

namespace Hypersteel.Damage;

public static class CollisionImpact
{
	public const float Efficiency = 0.65f;
	public const float CrushMassRatio = 0.45f;
	public const float CrushMin = 8f;
	public const float ImpulseScale = 0.15f;

	public static float Kinetic(float myMass, float otherMass, float speedAlongNormal, float glancing01)
	{
		myMass = Math.Max(0.01f, myMass);
		otherMass = Math.Max(0.01f, otherMass);
		float rel = Math.Abs(speedAlongNormal);
		float reduced = (myMass * otherMass) / (myMass + otherMass);
		float impulse = reduced * rel * Efficiency * (0.25f + 0.75f * glancing01);
		float fromSpeed = impulse * ImpulseScale;

		float ratio = otherMass / myMass;
		float crush = 0f;
		if (ratio >= CrushMassRatio)
			crush = CrushMin * ratio * (0.35f + rel);

		return Math.Max(fromSpeed, crush);
	}

	public static float Glancing01(Vector3 relativeVel, Vector3 normal)
	{
		Vector3 n = normal.LengthSquared() > 0.0001f ? normal.Normalized() : Vector3.Up;
		float along = Math.Abs(relativeVel.Normalized().Dot(n));
		if (relativeVel.LengthSquared() < 0.0001f) return 0.2f;
		return along; // 1 = head-on, 0 = scrape
	}

	public static float MassOf(Node n, float fallback)
	{
		if (n is RigidBody3D rb) return Math.Max(0.01f, rb.Mass);
		if (n is CollisionDamage cd && cd.Mass > 0f) return cd.Mass;
		if (n is Hypersteel.Entity.ActorEntity actor && actor.Mass > 0f) return actor.Mass;
		Node p = n;
		while (p != null)
		{
			if (p is CollisionDamage c && c.Mass > 0f) return c.Mass;
			if (p is Hypersteel.Entity.ActorEntity a && a.Mass > 0f) return a.Mass;
			if (p is RigidBody3D r) return Math.Max(0.01f, r.Mass);
			p = p.GetParent();
		}
		return fallback;
	}
}
