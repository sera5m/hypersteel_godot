using Godot;
using Hypersteel.Entity.NpcAI.Brain;

namespace Hypersteel.Entity.NpcAI;

public partial class SoldierPawn : ActorEntity
{
	public bool Critical { get; private set; }
	public NpcBrain Brain { get; private set; }

	public override void _Ready()
	{
		base._Ready();
		entityId = "soldier";
		stats ??= new EntityStats { Mass = 80f, Strength = 1f, Agility = 1f, Speed = 1f };

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
				Stats = stats as NpcStats ?? new NpcStats()
			};
			AddChild(Brain);
		}
		else
			Brain.Stats = stats as NpcStats ?? Brain.Stats;
	}

	protected override void OnHealthBand(HealthBand from, HealthBand to)
	{
		Critical = to == HealthBand.Low || to == HealthBand.Dead;
	}
}
