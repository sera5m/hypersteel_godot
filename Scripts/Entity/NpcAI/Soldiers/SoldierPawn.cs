using Godot;
using Hypersteel.Entity.NpcAI.Brain;

namespace Hypersteel.Entity.NpcAI;

public partial class SoldierPawn : ActorEntity
{
	public bool Critical { get; private set; }
	public NpcBrain Brain { get; private set; }

	public override void _Ready()
	{
		stats ??= new NpcStats { Mass = 80f, Strength = 1f, Agility = 1f, Speed = 1f };
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
				},
				Stats = stats as NpcStats
			};
			AddChild(Brain);
		}
		else if (stats is NpcStats ns)
			Brain.Stats = ns;
	}

	protected override void OnHealthBand(HealthBand from, HealthBand to)
	{
		Critical = to == HealthBand.Low || to == HealthBand.Dead;
	}
}
