using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>Data. Scout/sniper/soldier are rows, not subclasses.</summary>
[GlobalClass]
public partial class NpcRoleDef : Resource
{
	[Export] public string RoleId = "soldier";
	[Export] public NpcVerb DefaultVerb = NpcVerb.MoveAndAttack;
	[Export] public float PreferredRange = 18f;
	[Export] public bool WantsHighGround;
	[Export] public bool SoftStep;
	[Export] public bool IssuesEnclose;
	[Export] public bool PingsOnSight;
}
