using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>
/// One brain, four clocks. Attach to a pawn. Do not fork Resolve.
/// Mesh consensus is off: RoleDef rules run alone.
/// </summary>
public partial class NpcBrain : Node
{
	[Export] public NpcRoleDef Role;
	[Export] public bool UseMeshConsensus;

	public NpcReflex Reflex { get; } = new();
	public NpcVerb ActiveVerb { get; private set; }
	public NpcSenseSnapshot Sense;

	ActorEntity _body;

	public override void _Ready()
	{
		_body = GetParent() as ActorEntity;
		Role ??= new NpcRoleDef();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_body == null || _body.IsDead) return;
		TickSense();
		var asked = PickMacro();
		asked = Reshape(asked);
		ActiveVerb = NpcMacroMicro.Rewrite(asked, Sense, _body);
		NpcExecution.Step(_body, Role, ActiveVerb, Sense, (float)delta);
	}

	void TickSense()
	{
		Sense.SuggestedMacro = UseMeshConsensus ? Sense.SuggestedMacro : NpcMacroIntent.None;
		if (_body.health?.State != null)
		{
			var hp = _body.health.State.HpOf(Hypersteel.Health.BodySegment.Torso);
			Sense.OccupantHp01 = Mathf.Clamp(hp / 100f, 0f, 1f);
		}
	}

	NpcVerb PickMacro()
	{
		if (UseMeshConsensus && Sense.SuggestedMacro == NpcMacroIntent.Enclose)
			return NpcVerb.Enclose;
		return Role != null ? Role.DefaultVerb : NpcVerb.MoveAndAttack;
	}

	NpcVerb Reshape(NpcVerb verb)
	{
		if (Role == null) return verb;
		if (Role.PingsOnSight && verb == NpcVerb.MoveAndAttack)
			return NpcVerb.GoLook;
		if (Role.IssuesEnclose && verb == NpcVerb.MoveAndAttack)
			return NpcVerb.Enclose;
		return verb;
	}

	/// <summary>Call from ordinance overlap. Reflex logs after it fires.</summary>
	public void IncomingNade(Vector3 from)
	{
		if (_body == null) return;
		Reflex.TryNadeLeap(_body, _body.GlobalPosition - from);
	}
}
