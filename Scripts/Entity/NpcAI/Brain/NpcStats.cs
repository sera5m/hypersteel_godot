using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>MP fields that AI also uses. Role crumbs sit on NpcRoleDef.</summary>
[GlobalClass]
public partial class NpcStats : Resource
{
	[Export] public float Reaction = 0.28f;
	[Export] public float LayerReaction = 0.08f;
	[Export] public float TurnRateDeg = 320f;
	[Export] public float VisionConeDeg = 110f;
	[Export] public float AimSlackDeg = 30f;
	[Export] public float PreRotate = 0.2f;
	[Export] public float Speed = 1f;
	[Export] public float Agility = 1f;
	[Export] public float Strength = 1f;
	[Export] public float UseTime = 1f;
	[Export] public float AbilityCdScale = 1f;
	[Export] public float FearResist = 0.2f;
	[Export] public int Rank;
}
