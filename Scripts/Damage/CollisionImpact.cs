using Godot;

namespace Hypersteel.Damage;

public enum MotionStance
{
	Air = 0,
	Climb,
	Wallrun,
	Ground,
	Crouch,
	Prone,
}

public static class CollisionImpact
{
	public const float CrushMassRatio = 0.45f;
	public const float CrushMin = 8f;
	public const float FleshRestitution = 0.18f;
	public const float SteelRestitution = 0.58f;

	public static float StanceMul(MotionStance s) => s switch
	{
		MotionStance.Air => 1.00f,
		MotionStance.Climb => 0.70f,
		MotionStance.Wallrun => 0.60f,
		MotionStance.Ground => 0.50f,
		MotionStance.Crouch => 0.20f,
		MotionStance.Prone => 0.05f,
		_ => 1f,
	};

	/// <summary>
	/// Normal KE fraction. cos²θ of angle between velocity and contact normal.
	/// Head-on = 1, 90° scrape = 0. No acos.
	/// </summary>
	public static float NormalEnergy01(Vector3 relativeVel, Vector3 normal)
	{
		float v2 = relativeVel.LengthSquared();
		if (v2 < 1e-8f) return 0.15f;
		Vector3 n = normal.LengthSquared() > 1e-8f ? normal.Normalized() : Vector3.Up;
		float vn = relativeVel.Dot(n);
		float cos2 = (vn * vn) / v2;
		if (cos2 < 0f) cos2 = 0f;
		if (cos2 > 1f) cos2 = 1f;
		return cos2;
	}

	/// <summary>Inelastic leftover (1-e²) times reduced-mass * vn². Elastic steel soaks less HP.</summary>
	public static float Kinetic(float myMass, float otherMass, Vector3 relVel, Vector3 normal, float restitution)
	{
		myMass = Math.Max(0.01f, myMass);
		otherMass = Math.Max(0.01f, otherMass);
		float e = Math.Clamp(restitution, 0f, 0.95f);
		float vn = Math.Abs(relVel.Dot(normal.LengthSquared() > 1e-8f ? normal.Normalized() : Vector3.Up));
		float reduced = myMass * otherMass / (myMass + otherMass);
		float inelastic = (1f - e * e) * 0.5f * reduced * vn * vn;
		float crush = 0f;
		float ratio = otherMass / myMass;
		if (ratio >= CrushMassRatio)
			crush = CrushMin * ratio * (0.35f + vn);
		return Math.Max(inelastic * 0.02f, crush); // 0.02 scales engine units toward HP
	}

	public static float MassOf(Node n, float fallback)
	{
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

	public static float RestitutionOf(Node n)
	{
		Node p = n;
		while (p != null)
		{
			if (p is CollisionDamage c) return c.Restitution;
			p = p.GetParent();
		}
		return FleshRestitution;
	}
}
