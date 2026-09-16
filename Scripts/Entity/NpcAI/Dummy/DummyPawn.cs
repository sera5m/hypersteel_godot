using Godot;
using Hypersteel.Damage;
using Hypersteel.Health;

namespace Hypersteel.Entity.NpcAI;

public partial class DummyPawn : ActorEntity
{
	[Export] public bool logHits = true;
	[Export] public bool buildStandIn = true;

	public override void _Ready()
	{
		entityId = "dummy";
		kit ??= new KitSheet { hasHealth = true, hasStatus = true, hasFeelings = false };
		base._Ready();
		if (buildStandIn)
			HumanoidStandIn.Attach(this);

		if (health != null)
			health.Damaged += OnHp;

		foreach (Node child in GetChildren())
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
		{
			GD.Print($"[dummy] {packet.Name} {packet.Kinetic:0.0} {packet.Source} seg={packet.Segment} bone={packet.Bone} cover={packet.FromCover}");
			if (health?.State != null)
				GD.Print($"[dummy] torso={health.State.HpOf(BodySegment.Torso):0} head={health.State.HpOf(BodySegment.Head):0} armL={health.State.HpOf(BodySegment.ArmL):0} dead={health.State.Dead}");
		}
		base.Hurt(packet);
	}

	void OnHp(float taken, int segment)
	{
		if (logHits)
			GD.Print($"[dummy] hp -{taken:0.0} segment={(BodySegment)segment}");
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
