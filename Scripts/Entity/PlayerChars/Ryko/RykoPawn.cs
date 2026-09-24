using Godot;

namespace Hypersteel.Entity.PlayerChars;

public partial class RykoPawn : ActorEntity
{
	[Export] public bool buildStandIn;
	[Export] public bool armored = true;

	public override void _Ready()
	{
		stats ??= RykoSheet();
		Mass = armored ? 220f : 170f;
		stats.Mass = Mass;
		base._Ready();
		entityId = "ryko";
		if (buildStandIn)
			HumanoidStandIn.Attach(this);
	}

	public static EntityStats RykoSheet() => new()
	{
		Mass = 220f,
		MassBare = 170f,
		LiftKg = 600f,
		Strength = 10f,
		Agility = 1.4f,
		Speed = 1.2f,
		Stamina = 1.2f,
		UseTime = 0.55f,
		TurnRateDeg = 420f,
		Reaction = 0.12f,
	};
}
