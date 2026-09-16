using Godot;
using Hypersteel.Health;
using Hypersteel.Abilities.Kits;

namespace Hypersteel.Damage;

/// <summary>Closed flag list. KitComposer only instances these.</summary>
[GlobalClass]
public partial class KitSheet : Resource
{
	[Export] public bool hasHealth = true;
	[Export] public bool hasStatus = true;
	[Export] public bool hasFeelings;
	[Export] public bool hasMorale;
	[Export] public bool hasJumpKit;

	[Export] public HealthRuleConfs healthRules;
	[Export] public ArmorSheet armor;
	[Export] public StatusRules status;
	[Export] public FeelingsRules feelings;
	[Export] public JumpKitDef jumpKit;
}
