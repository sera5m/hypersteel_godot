namespace Hypersteel.Entity.NpcAI;

/// <summary>Local hook only. Health does not know what "critical" means.</summary>
public partial class SoldierPawn : ActorEntity
{
	public bool Critical { get; private set; }

	public override void _Ready()
	{
		base._Ready();
		entityId = "soldier";
	}

	protected override void OnHealthBand(HealthBand from, HealthBand to)
	{
		Critical = to == HealthBand.Low || to == HealthBand.Dead;
	}
}
