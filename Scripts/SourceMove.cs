using Godot;

public struct SourceSlideHit
{
	public bool OnFloor;
	public bool HitWall;
	public Vector3 FloorNormal;
	public Vector3 WallNormal;
	public float WallImpactSpeed;
}

public static class SourceMove
{
	public static Vector3 WishDirection(Basis bodyBasis, Vector2 input)
	{
		Vector3 wish = bodyBasis * new Vector3(input.X, 0f, input.Y);
		wish.Y = 0f;
		return wish.LengthSquared() > 0.0001f ? wish.Normalized() : Vector3.Zero;
	}

	public static void Friction(ref Vector3 vel, float dt, float friction, float stopSpeed)
	{
		Vector3 horizontal = new Vector3(vel.X, 0f, vel.Z);
		float speed = horizontal.Length();
		if (speed < 0.0001f)
			return;

		float control = speed < stopSpeed ? stopSpeed : speed;
		float drop = control * friction * dt;
		float newSpeed = Mathf.Max(speed - drop, 0f);
		float scale = newSpeed / speed;
		vel.X *= scale;
		vel.Z *= scale;
	}

	public static void Accelerate(ref Vector3 vel, Vector3 wishDir, float wishSpeed, float accel, float dt)
	{
		if (wishDir.LengthSquared() < 0.0001f || wishSpeed <= 0f)
			return;

		float currentSpeed = vel.Dot(wishDir);
		float addSpeed = wishSpeed - currentSpeed;
		if (addSpeed <= 0f)
			return;

		float accelSpeed = accel * wishSpeed * dt;
		if (accelSpeed > addSpeed)
			accelSpeed = addSpeed;

		vel += wishDir * accelSpeed;
	}

	public static void AirAccelerate(ref Vector3 vel, Vector3 wishDir, float wishSpeed, float accel, float airSpeedCap, float dt)
	{
		if (wishDir.LengthSquared() < 0.0001f)
			return;

		wishSpeed = Mathf.Min(wishSpeed, airSpeedCap);
		float currentSpeed = vel.Dot(wishDir);
		float addSpeed = wishSpeed - currentSpeed;
		if (addSpeed <= 0f)
			return;

		float accelSpeed = accel * wishSpeed * dt;
		if (accelSpeed > addSpeed)
			accelSpeed = addSpeed;

		vel += wishDir * accelSpeed;
	}

	public static void AirDrag(ref Vector3 vel, float keepPerSecond, float dt)
	{
		keepPerSecond = Mathf.Clamp(keepPerSecond, 0.01f, 1f);
		float scale = Mathf.Pow(keepPerSecond, dt);
		vel.X *= scale;
		vel.Z *= scale;
	}

	public static void PivotTowardWish(ref Vector3 vel, Vector3 wishDir, float yawRateAbs, float scalar, float maxConvert, float dt)
	{
		if (wishDir.LengthSquared() < 0.0001f)
			return;

		Vector3 h = new Vector3(vel.X, 0f, vel.Z);
		float speed = h.Length();
		if (speed < 0.0001f)
			return;

		float convert = Mathf.Clamp(yawRateAbs * scalar * dt, 0f, maxConvert);
		Vector3 blended = h.Lerp(wishDir * speed, convert);
		vel.X = blended.X;
		vel.Z = blended.Z;
	}

	public static void LurchIntoWish(ref Vector3 vel, Vector3 wishDir, float fraction)
	{
		if (wishDir.LengthSquared() < 0.0001f || fraction <= 0f)
			return;
		float mag = new Vector3(vel.X, 0f, vel.Z).Length();
		vel += wishDir * mag * fraction;
	}

	public static void Rebound(ref Vector3 vel, Vector3 normal, HypersteelPhysSauce sauce)
	{
		if (sauce == null || !sauce.reboundEnabled)
			return;

		float incoming = vel.Length();
		if (incoming < 0.0001f)
			return;

		float into = vel.Dot(normal);
		if (into >= 0f)
			return;

		float impact = -into;
		if (impact < sauce.reboundMinImpact)
		{
			vel = vel.Slide(normal);
			return;
		}

		float steepness = impact / incoming;
		float restit = Mathf.Min(sauce.reboundRestitution * steepness, sauce.reboundMax);

		if (steepness >= sauce.reboundSteepBounce)
		{
			Vector3 bounced = vel.Bounce(normal);
			float cap = incoming * sauce.reboundMax;
			if (bounced.Length() > cap)
				bounced = bounced.Normalized() * cap;
			vel = vel.Slide(normal) * (1f - restit) + bounced * restit;
		}
		else
		{
			vel = vel.Slide(normal);
		}
	}

	public static void ClampVelocity(ref Vector3 vel, float maxVelocity)
	{
		if (vel.Length() > maxVelocity)
			vel = vel.Normalized() * maxVelocity;
	}

	public static float StepHeightOf(CharacterBody3D body)
	{
		float snap = body.FloorSnapLength;
		if (snap > 0.05f) return snap;
		return CapsuleHeight(body) / 3f;
	}

	public static float CapsuleHeight(CharacterBody3D body)
	{
		foreach (Node child in body.GetChildren())
		{
			if (child is CollisionShape3D cs && cs.Shape is CapsuleShape3D cap)
				return cap.Height + cap.Radius * 2f;
		}
		return 1.8f;
	}

	public static SourceSlideHit MoveAndSlideOwn(CharacterBody3D body, ref Vector3 velocity, Vector3 up, float floorMaxAngle, float gravity, HypersteelPhysSauce sauce, int maxSlides = 6)
	{
		SourceSlideHit hit = default;
		float dt = (float)body.GetPhysicsProcessDeltaTime();
		float stepH = StepHeightOf(body);

		Vector3 checkMotion = velocity * (1f / 60f);
		checkMotion.Y -= gravity * (1f / 360f);
		KinematicCollision3D testCol = body.MoveAndCollide(checkMotion, testOnly: true);
		if (testCol != null)
		{
			Vector3 testNormal = testCol.GetNormal();
			if (testNormal.AngleTo(up) < floorMaxAngle)
			{
				hit.OnFloor = true;
				hit.FloorNormal = testNormal;
			}
		}

		Vector3 motion = velocity * dt;
		for (int step = 0; step < maxSlides; step++)
		{
			KinematicCollision3D collision = body.MoveAndCollide(motion);
			if (collision == null)
				break;

			Vector3 normal = collision.GetNormal();
			if (normal.AngleTo(up) < floorMaxAngle)
			{
				hit.OnFloor = true;
				hit.FloorNormal = normal;
				motion = collision.GetRemainder().Slide(normal);
				velocity = velocity.Slide(normal);
				continue;
			}

			hit.HitWall = true;
			hit.WallNormal = normal;
			hit.WallImpactSpeed = Mathf.Max(hit.WallImpactSpeed, -velocity.Dot(normal));

			if (TryStepUp(body, collision.GetRemainder(), up, stepH, floorMaxAngle))
			{
				hit.OnFloor = true;
				break;
			}

			Rebound(ref velocity, normal, sauce);
			motion = collision.GetRemainder().Slide(normal);
		}

		return hit;
	}

	static bool TryStepUp(CharacterBody3D body, Vector3 remainder, Vector3 up, float stepH, float floorMaxAngle)
	{
		if (stepH < 0.05f) return false;
		Vector3 horiz = remainder;
		horiz.Y = 0f;
		if (horiz.LengthSquared() < 1e-6f) return false;

		KinematicCollision3D ceiling = body.MoveAndCollide(up * stepH, testOnly: true);
		if (ceiling != null && ceiling.GetTravel().Length() < stepH * 0.25f)
			return false;

		body.GlobalPosition += up * stepH;
		KinematicCollision3D fwd = body.MoveAndCollide(horiz);
		body.MoveAndCollide(-up * stepH);

		if (fwd == null) return true;
		return fwd.GetNormal().AngleTo(up) < floorMaxAngle;
	}
}
