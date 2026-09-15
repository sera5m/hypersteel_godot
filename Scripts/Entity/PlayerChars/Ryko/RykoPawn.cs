using Godot;

namespace Hypersteel.Entity.PlayerChars;

/// <summary>Ryko-specific kit hooks. Movement stays on PlayerMovement until you reparent the scene.</summary>
public partial class RykoPawn : ActorEntity
{
	public override void _Ready()
	{
		base._Ready();
		entityId = "ryko";
	}
}
