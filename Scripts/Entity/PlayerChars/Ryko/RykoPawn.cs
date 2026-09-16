using Godot;

namespace Hypersteel.Entity.PlayerChars;

public partial class RykoPawn : ActorEntity
{
	[Export] public bool buildStandIn;

	public override void _Ready()
	{
		base._Ready();
		entityId = "ryko";
		if (buildStandIn)
			HumanoidStandIn.Attach(this);
	}
}
