using Godot;
using Hypersteel.Health;

namespace Hypersteel.Entity;

/// <summary>
/// Shared pawn shell. Movement, AI, and kit stay on sibling nodes.
/// Health is composed, not inherited as logic.
/// </summary>
public partial class ActorEntity : CharacterBody3D
{
	[Export] public HealthComponent health;
	[Export] public StringName entityId = "actor";

	public override void _Ready()
	{
		health ??= GetNodeOrNull<HealthComponent>("Health");
		health ??= FindChild("Health", true, false) as HealthComponent;
	}

	public void Hurt(Hit hit) => health?.Hurt(hit);

	public bool IsDead => health != null && health.State != null && health.State.Dead;
}
