using Godot;

namespace Hypersteel.Health;

public partial class HealthComponent : Node
{
	[Export] public HealthRuleConfs rules;
	[Export] public NodePath skeletonPath;

	public HealthState State { get; private set; }

	[Signal] public delegate void DamagedEventHandler(float taken, int segment);
	[Signal] public delegate void DiedEventHandler();

	public override void _Ready()
	{
		rules ??= new HealthRuleConfs();
		State = new HealthState(rules);

		Skeleton3D skel = null;
		if (skeletonPath != null && !skeletonPath.IsEmpty)
			skel = GetNodeOrNull<Skeleton3D>(skeletonPath);
		skel ??= FindSkeleton(GetParent());
		State.MapSkeleton(skel);
	}

	public void Hurt(Hit hit)
	{
		if (State == null) return;
		var result = State.Apply(hit, (float)GetPhysicsProcessDeltaTime());
		if (result.IntoHp > 0f || result.IntoShield > 0f)
			EmitSignal(SignalName.Damaged, result.IntoHp + result.IntoShield, (int)hit.Segment);
		if (State.Dead)
			EmitSignal(SignalName.Died);
	}

	static Skeleton3D FindSkeleton(Node from)
	{
		if (from == null) return null;
		if (from is Skeleton3D s) return s;
		foreach (Node child in from.GetChildren())
		{
			var found = FindSkeleton(child);
			if (found != null) return found;
		}
		return null;
	}
}
