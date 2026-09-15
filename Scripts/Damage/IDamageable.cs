namespace Hypersteel.Damage;

/// <summary>
/// Unreal IDamageable stand-in. Implemented only by ActorEntity.
/// Colliders never cast to RykoPawn / SoldierPawn.
/// </summary>
public interface IDamageable
{
	void Hurt(DamagePacket packet);
	bool IsDead { get; }
}
