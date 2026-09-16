using Godot;
using Hypersteel.Damage;
using Hypersteel.Health;

namespace Hypersteel.Entity.NpcAI;

/// <summary>
/// Scene actor. Mesh/skeleton/plates live in Dummy.tscn, not spawned here.
/// ArmorPiece children soak and forward leftover into Hurt — health is not armor.
/// </summary>
public partial class DummyPawn : ActorEntity
{
	[Export] public bool logHits = true;
	[Export] public bool buildStandIn;

	public override void _Ready()
	{
		entityId = "dummy";
		kit ??= new KitSheet { hasHealth = true, hasStatus = false, hasFeelings = false };
		base._Ready();

		if (buildStandIn)
			HumanoidStandIn.Attach(this);

		if (health != null)
		{
			var skel = FindChild("Skeleton3D", true, false) as Skeleton3D;
			if (skel != null && health.skeletonPath == null)
				health.skeletonPath = health.GetPathTo(skel);
			health.Damaged += OnHp;
		}

		foreach (Node child in FindChildren("*", owned: true))
		{
			if (child is ArmorPiece plate)
			{
				plate.ArmorDamaged += OnPlateDamaged;
				plate.ArmorBroken += OnPlateBroken;
			}
		}
	}

	public override void Hurt(DamagePacket packet)
	{
		if (logHits)
			GD.Print($"[dummy] through={packet.Kinetic:0.0} seg={packet.Segment} cover={packet.FromCover} src={packet.Name}");
		base.Hurt(packet);
	}

	void OnHp(float taken, int segment)
	{
		if (logHits)
			GD.Print($"[dummy] flesh -{taken:0.0} {(BodySegment)segment} torso={health?.State?.HpOf(BodySegment.Torso):0}");
	}

	void OnPlateDamaged(float taken, Node3D cause)
	{
		if (logHits)
			GD.Print($"[dummy] plate soaked {taken:0.0} from {cause?.Name ?? "world"}");
	}

	void OnPlateBroken(Node3D cause)
	{
		if (logHits)
			GD.Print($"[dummy] plate broke ({cause?.Name})");
	}
}
