using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

[GlobalClass]
public partial class NpcActionDef : Resource
{
	[Export] public string Id = "fire";
	[Export] public bool CanCancel = true;
	[Export] public float InitCost = 0.05f;
	[Export] public float CancelCost = 0.08f;
	[Export] public float Warmup;
	[Export] public float Duration = 0.2f;
	[Export] public float CoolOff;
	[Export] public float Cooldown;
	[Export] public NpcVerb Verb = NpcVerb.MoveAndAttack;
}
