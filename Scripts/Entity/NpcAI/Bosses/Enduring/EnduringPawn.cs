using Godot;

namespace Hypersteel.Entity.NpcAI;

public partial class EnduringPawn : ActorEntity
{
	public override void _Ready()
	{
		stats ??= EnduringSheet();
		Mass = stats.Mass;
		base._Ready();
		entityId = "enduring";
	}

	public static EntityStats EnduringSheet() => new()
	{
		Mass = 420f,
		MassBare = 380f,
		LiftKg = 1000f,
		Strength = 12f,
		Agility = 0.85f,
		Speed = 0.9f,
		Stamina = 1.3f,
		UseTime = 1.1f,
		TurnRateDeg = 220f,
		Reaction = 0.22f,
	};
}
