using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

public partial class FeelingsComponent : Node
{
	[Export] public FeelingsRules rules;
	[Export] public NodePath healthPath;

	[Signal] public delegate void FlinchEventHandler(float intensity);

	public override void _Ready()
	{
		rules ??= new FeelingsRules();
		HealthComponent health = null;
		if (healthPath != null && !healthPath.IsEmpty)
			health = GetNodeOrNull<HealthComponent>(healthPath);
		health ??= GetParent()?.GetNodeOrNull<HealthComponent>("Health");
		health ??= GetParent()?.FindChild("Health", true, false) as HealthComponent;
		if (health != null)
			health.Damaged += OnDamaged;
	}

	void OnDamaged(float taken, int segment)
	{
		if (rules == null || taken <= 0f) return;
		float rel = taken; // entity can pass better % later
		float intensity = rel / Math.Max(0.01f, rules.durability) * (1f - rules.resistance);
		if (intensity >= rules.flinchThreshold)
			EmitSignal(SignalName.Flinch, intensity);
	}
}
