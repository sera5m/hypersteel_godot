using Godot;

namespace Hypersteel.Damage;

public partial class StatusComponent : Node
{
	[Export] public StatusRules rules;

	public StatusTag Tags { get; private set; }
	public float Toxic;
	public float Heat;

	[Signal] public delegate void TagChangedEventHandler(int tags);

	public override void _Ready() => rules ??= new StatusRules();

	public void Add(StatusTag tag)
	{
		if (tag == StatusTag.Charged && Has(StatusTag.Wet)) return;
		if (tag == StatusTag.Stimmed) Tags &= ~StatusTag.Tranqued;
		if (tag == StatusTag.Tranqued) Tags &= ~StatusTag.Stimmed;
		if (tag == StatusTag.Hot) Tags &= ~StatusTag.Cold;
		if (tag == StatusTag.Cold) Tags &= ~StatusTag.Hot;
		if (tag == StatusTag.Liquefied && rules != null && !rules.flesh) return;
		StatusTag before = Tags;
		Tags |= tag;
		if (Tags != before) EmitSignal(SignalName.TagChanged, (int)Tags);
	}

	public void Remove(StatusTag tag)
	{
		StatusTag before = Tags;
		Tags &= ~tag;
		if (Tags != before) EmitSignal(SignalName.TagChanged, (int)Tags);
	}

	public bool Has(StatusTag tag) => (Tags & tag) != 0;

	public float HealMul()
	{
		float div = rules != null && rules.toxicHealDivisor > 0f ? rules.toxicHealDivisor : 10000f;
		return Math.Max(0f, 1f - Toxic / div);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Heat <= 0f && Toxic <= 0f) return;
		float dt = (float)delta;
		float tick = Heat > 100f ? dt * (Heat / 100f) : dt;
		Toxic = Math.Max(0f, Toxic - tick * 10f);
		if (rules != null && Heat >= rules.ignitionTemp)
			Add(StatusTag.Burning);
	}
}
