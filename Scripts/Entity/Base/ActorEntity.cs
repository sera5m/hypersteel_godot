using Godot;
using Hypersteel.Damage;
using Hypersteel.Health;

namespace Hypersteel.Entity;

/// <summary>
/// Damage door for every pawn. Children (mesh, hurtbox, gun) walk up here.
/// Never cast the collider to RykoPawn / SoldierPawn.
/// </summary>
public partial class ActorEntity : CharacterBody3D, IDamageable
{
	[Export] public KitSheet kit;
	[Export] public HealthComponent health;
	[Export] public StatusComponent status;
	[Export] public FeelingsComponent feelings;
	[Export] public StringName entityId = "actor";

	public HealthBand Band { get; private set; } = HealthBand.High;

	public override void _Ready()
	{
		KitComposer.Compose(this, kit);
		health ??= GetNodeOrNull<HealthComponent>("Health");
		health ??= FindChild("Health", true, false) as HealthComponent;
		status ??= GetNodeOrNull<StatusComponent>("Status");
		feelings ??= GetNodeOrNull<FeelingsComponent>("Feelings");

		if (health != null)
			health.Damaged += OnHealthDamaged;
	}

	public void Hurt(Hit hit) => health?.Hurt(hit);

	public void Hurt(DamagePacket packet)
	{
		if (health == null) return;
		health.Hurt(packet);
	}

	public bool IsDead => health != null && health.State != null && health.State.Dead;

	/// <summary>Override on a pawn that cares (soldier critical). Drone leaves this empty.</summary>
	protected virtual void OnHealthDamaged(float taken, int segment)
	{
		HealthBand next = BandFromHealth();
		if (next == Band) return;
		HealthBand prev = Band;
		Band = next;
		OnHealthBand(prev, next);
	}

	protected virtual void OnHealthBand(HealthBand from, HealthBand to) { }

	HealthBand BandFromHealth()
	{
		if (health?.State == null) return HealthBand.High;
		float torso = health.State.HpOf(BodySegment.Torso);
		float max = health.rules != null ? Math.Max(1f, health.rules.torsoHp) : 100f;
		float p = torso / max;
		if (p <= 0f) return HealthBand.Dead;
		if (p < 0.25f) return HealthBand.Low;
		if (p < 0.6f) return HealthBand.Medium;
		return HealthBand.High;
	}
}

public enum HealthBand
{
	High = 0,
	Medium,
	Low,
	Dead,
}
