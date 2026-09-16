using Godot;

namespace Hypersteel.Abilities.Kits;

/// <summary>
/// Closed surface a jumpkit may call. Player and AI motors implement this.
/// Do not let a kit invoke arbitrary methods on PlayerMovement.
/// </summary>
public interface IMoveMotor
{
	Vector3 Velocity { get; set; }
	Vector3 Forward { get; }
	bool OnFloor { get; }
	float GravityScale { get; set; }

	void ApplyImpulse(Vector3 delta);
	void OverrideVelocity(Vector3 worldVelocity, float seconds);
	void RestoreVelocity(Vector3 worldVelocity);
	void SetAirControl(bool on);
}
