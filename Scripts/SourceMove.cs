using Godot;

/// <summary>
/// Quake / Source PM_Accelerate + PM_AirAccelerate + PM_Friction,
/// ported from EricXu1728/Godot4SourceEngineMovement (GDScript) into C#.
/// Hypersteel verbs (slide, wallrun, vault, jump) feed velocity into this.
/// </summary>
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

	public static void ClampVelocity(ref Vector3 vel, float maxVelocity)
	{
		if (vel.Length() > maxVelocity)
			vel = vel.Normalized() * maxVelocity;
	}

	public static bool MoveAndSlideOwn(CharacterBody3D body, ref Vector3 velocity, Vector3 up, float floorMaxAngle, float gravity, int maxSlides = 6)
	{
		bool onFloor = false;
		float dt = (float)body.GetPhysicsProcessDeltaTime();

		Vector3 checkMotion = velocity * (1f / 60f);
		checkMotion.Y -= gravity * (1f / 360f);
		KinematicCollision3D testCol = body.MoveAndCollide(checkMotion, testOnly: true);
		if (testCol != null)
		{
			Vector3 testNormal = testCol.GetNormal();
			if (testNormal.AngleTo(up) < floorMaxAngle)
				onFloor = true;
		}

		Vector3 motion = velocity * dt;
		for (int step = 0; step < maxSlides; step++)
		{
			KinematicCollision3D collision = body.MoveAndCollide(motion);
			if (collision == null)
				break;

			Vector3 normal = collision.GetNormal();
			if (normal.AngleTo(up) < floorMaxAngle)
				onFloor = true;

			motion = collision.GetRemainder().Slide(normal);
			velocity = velocity.Slide(normal);
		}

		return onFloor;
	}
}
