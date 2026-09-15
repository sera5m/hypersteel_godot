using Godot;
using Hypersteel.Damage;

namespace Hypersteel.Entity.NpcAI;

/// <summary>
/// Test dummy. Inherits ActorEntity so DamageProbe + kit compose work.
/// Attach ArmorPiece children on chest/arm. Shoot the plate collider.
/// </summary>
public partial class DummyPawn : ActorEntity
{
	[Export] public bool logHits = true;

	public override void _Ready()
	{
		base._Ready();
		entityId = "dummy";
		kit ??= new KitSheet { hasHealth = true, hasStatus = true, hasFeelings = false };

		foreach (Node child in GetChildren())
		{
			if (child is ArmorPiece plate)
			{
				plate.ArmorDamaged += OnPlateDamaged;
				plate.ArmorBroken += OnPlateBroken;
			}
		}
	}

	void OnPlateDamaged(float taken, Node3D cause)
	{
		if (logHits)
			GD.Print($"[dummy] plate took {taken} from {cause?.Name ?? "world"}");
	}

	void OnPlateBroken(Node3D cause)
	{
		if (logHits)
			GD.Print($"[dummy] plate broken by {cause?.Name ?? "world"}");
	}

	protected override void OnHealthBand(HealthBand from, HealthBand to)
	{
		if (logHits)
			GD.Print($"[dummy] band {from} -> {to}");
	}
}
