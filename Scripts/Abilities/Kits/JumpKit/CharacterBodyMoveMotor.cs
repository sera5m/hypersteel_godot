using Godot;

namespace Hypersteel.Abilities.Kits;

/// <summary>
/// Default motor mapping: any CharacterBody3D. If the body is PlayerMovement,
/// also writes playerVelocity so Source step does not eat the dash on the next tick.
/// Replace with a first-party motor slot when the player scene is an ActorEntity.
/// </summary>
public sealed class CharacterBodyMoveMotor : IMoveMotor
{
	public readonly CharacterBody3D Body;
	float _gravityScale = 1f;
	float _overrideLeft;
	Vector3 _overrideVel;

	public CharacterBodyMoveMotor(CharacterBody3D body)
	{
		Body = body;
	}

	public Vector3 Velocity
	{
		get => Body.Velocity;
		set => WriteVel(value);
	}

	public Vector3 Forward => -Body.GlobalBasis.Z;

	public bool OnFloor => Body.IsOnFloor();

	public float GravityScale
	{
		get => _gravityScale;
		set => _gravityScale = value;
	}

	public bool OverrideActive => _overrideLeft > 0f;
	public float OverrideLeft => _overrideLeft;

	public void ApplyImpulse(Vector3 delta) => WriteVel(ReadVel() + delta);

	public void OverrideVelocity(Vector3 worldVelocity, float seconds)
	{
		_overrideVel = worldVelocity;
		_overrideLeft = Mathf.Max(0f, seconds);
		WriteVel(worldVelocity);
	}

	public void RestoreVelocity(Vector3 worldVelocity)
	{
		_overrideLeft = 0f;
		WriteVel(worldVelocity);
	}

	public void SetAirControl(bool on) { }

	public void TickOverride(float dt)
	{
		if (_overrideLeft <= 0f) return;
		WriteVel(_overrideVel);
		_overrideLeft -= dt;
	}

	Vector3 ReadVel()
	{
		if (Body is PlayerMovement pm) return pm.playerVelocity;
		return Body.Velocity;
	}

	void WriteVel(Vector3 v)
	{
		Body.Velocity = v;
		if (Body is PlayerMovement pm)
			pm.playerVelocity = v;
	}
}
