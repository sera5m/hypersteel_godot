using Godot;

namespace Hypersteel.Damage;

[GlobalClass]
public partial class FeelingsRules : Resource
{
	[Export] public float durability = 1f;
	[Export] public float resistance = 0f;
	[Export] public float flinchThreshold = 0.08f;
}
