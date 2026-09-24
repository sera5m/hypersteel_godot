using Godot;

namespace Hypersteel.Weapons;

/// <summary>Physical sight. Group world_pickup, meta kind=Sight, meta sight_id.</summary>
public partial class SightPickup : Node3D
{
	[Export] public string SightId = "1x";

	public override void _Ready()
	{
		AddToGroup("world_pickup");
		SetMeta("kind", "Sight");
		SetMeta("sight_id", SightId);
	}

	public SightDef Take() => SightDef.FromId(SightId);
}
