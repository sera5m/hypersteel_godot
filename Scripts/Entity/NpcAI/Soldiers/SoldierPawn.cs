namespace Hypersteel.Entity.NpcAI;

public partial class SoldierPawn : ActorEntity
{
	public override void _Ready()
	{
		base._Ready();
		entityId = "soldier";
	}
}
