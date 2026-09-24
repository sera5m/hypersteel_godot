using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>Standard intercept. No aimbot — caller still has to turn at RoleDef rate.</summary>
public static class AimLead
{
	public static bool Flat(Vector3 shooter, Vector3 target, Vector3 targetVel, float muzzle, out Vector3 aimPoint, out float t)
	{
		aimPoint = target;
		t = 0f;
		var r = target - shooter;
		var a = targetVel.Dot(targetVel) - muzzle * muzzle;
		var b = 2f * r.Dot(targetVel);
		var c = r.Dot(r);

		if (Mathf.Abs(a) < 1e-4f)
		{
			if (Mathf.Abs(b) < 1e-4f) return false;
			t = -c / b;
			if (t <= 0f) return false;
			aimPoint = target + targetVel * t;
			return true;
		}

		var disc = b * b - 4f * a * c;
		if (disc < 0f) return false;
		var sqrt = Mathf.Sqrt(disc);
		var t1 = (-b - sqrt) / (2f * a);
		var t2 = (-b + sqrt) / (2f * a);
		t = SmallestPositive(t1, t2);
		if (t <= 0f) return false;
		aimPoint = target + targetVel * t;
		return true;
	}

	public static bool Ballistic(Vector3 shooter, Vector3 target, Vector3 targetVel, float muzzle, float g, out Vector3 aimPoint, out float t)
	{
		if (!Flat(shooter, target, targetVel, muzzle, out aimPoint, out t))
			return false;
		aimPoint += Vector3.Up * (0.5f * g * t * t);
		return true;
	}

	static float SmallestPositive(float a, float b)
	{
		var okA = a > 0f;
		var okB = b > 0f;
		if (okA && okB) return Mathf.Min(a, b);
		if (okA) return a;
		if (okB) return b;
		return -1f;
	}
}
