using Godot;
using Hypersteel.Entity.NpcAI.Brain;

namespace Hypersteel.Entity.NpcAI;

/// <summary>Local hook only. Health does not know what "critical" means. Owns NpcBrain.</summary>
public partial class SoldierPawn : ActorEntity
{
	public bool Critical { get; private set; }
	public NpcBrain Brain { get; private set; }

	public override void _Ready()
	{
		base._Ready();
		entityId = "soldier";

		Brain = GetNodeOrNull<NpcBrain>("NpcBrain");
		if (Brain == null)
		{
			Brain = new NpcBrain
			{
				Name = "NpcBrain",
				Role = new NpcRoleDef
				{
					RoleId = "soldier",
					DefaultVerb = NpcVerb.MoveAndAttack,
					PreferredRange = 18f
				}
			};
			AddChild(Brain);
		}
	}

	protected override void OnHealthBand(HealthBand from, HealthBand to)
	{
		Critical = to == HealthBand.Low || to == HealthBand.Dead;
	}
}
