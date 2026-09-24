using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>
/// One brain, four clocks. Mesh off. RoleDef rules run alone.
/// </summary>
public partial class NpcBrain : Node
{
	[Export] public NpcRoleDef Role;
	[Export] public NpcStats Stats;
	[Export] public bool UseMeshConsensus;
	[Export] public Node3D AssignedTarget;
	[Export] public float HazardRayMeters = 3.5f;

	public NpcReflex Reflex { get; } = new();
	public NpcVision Vision { get; } = new();
	public NpcMorale Morale { get; } = new();
	public NpcActionRunner Actions { get; } = new();
	public NpcTeamComms Comms { get; } = new();
	public NpcVerb ActiveVerb { get; private set; }
	public NpcSenseSnapshot Sense;

	ActorEntity _body;

	public override void _Ready()
	{
		_body = GetParent() as ActorEntity;
		Role ??= new NpcRoleDef { RoleId = "soldier", DefaultVerb = NpcVerb.MoveAndAttack, PreferredRange = 18f };
		Stats ??= new NpcStats();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_body == null || _body.IsDead) return;
		var dt = (float)delta;
		Vision.Tick(dt);
		Morale.Tick(dt);
		Actions.Tick(dt);
		TickSense();
		if (Role != null && Role.HasTeamComms)
			Comms.Refresh(_body, Stats != null ? Stats.Rank : 0);

		var asked = PickMacro();
		asked = Reshape(asked);
		asked = ApplyMorale(asked);
		asked = HoldBand(asked);
		ActiveVerb = NpcMacroMicro.Rewrite(asked, Sense, _body);
		NpcExecution.Step(_body, Role, ActiveVerb, Sense, dt);
	}

	void TickSense()
	{
		Sense.SuggestedMacro = UseMeshConsensus ? Sense.SuggestedMacro : NpcMacroIntent.None;

		if (_body.health?.State != null)
		{
			var hp = _body.health.State.HpOf(Hypersteel.Health.BodySegment.Torso);
			Sense.OccupantHp01 = Mathf.Clamp(hp / 100f, 0f, 1f);
		}

		if (AssignedTarget != null && GodotObject.IsInstanceValid(AssignedTarget))
		{
			var p = AssignedTarget.GlobalPosition;
			if (Sense.HasLastKnown)
				Vision.LastVel = (p - Sense.LastKnownTarget) / Mathf.Max(0.016f, GetPhysicsProcessDeltaTime());
			Sense.LastKnownTarget = p;
			Sense.HasLastKnown = true;
			Vision.LastKnown = p;
			var look = -_body.GlobalTransform.Basis.Z;
			var to = p - _body.GlobalPosition;
			var cone = Stats != null ? Stats.VisionConeDeg : 110f;
			if (NpcVision.InCone(look, to, cone)) Vision.NotifySeen(Stats);
		}

		Sense.WorldHazardAhead = false;
		var space = _body.GetWorld3D()?.DirectSpaceState;
		if (space == null) return;
		var origin = _body.GlobalPosition + Vector3.Up * 0.9f;
		var behind = _body.GlobalTransform.Basis.Z * HazardRayMeters;
		var q = PhysicsRayQueryParameters3D.Create(origin, origin + behind);
		q.Exclude = new Godot.Collections.Array<Rid> { _body.GetRid() };
		var hit = space.IntersectRay(q);
		if (hit.Count > 0)
		{
			Sense.WorldHazardAhead = true;
			Sense.WorldHazardPoint = (Vector3)hit["position"];
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
		if (Role.PingsOnSight && verb == NpcVerb.MoveAndAttack) return NpcVerb.GoLook;
		if (Role.IssuesEnclose && verb == NpcVerb.MoveAndAttack) return NpcVerb.Enclose;
		return verb;
	}

	NpcVerb ApplyMorale(NpcVerb verb)
	{
		return Morale.State switch
		{
			NpcMoraleState.Terrified => NpcVerb.GetOutOfView,
			NpcMoraleState.Enraged => NpcVerb.MoveAndAttack,
			_ => verb
		};
	}

	NpcVerb HoldBand(NpcVerb verb)
	{
		if (!Sense.HasLastKnown || Role == null || _body == null) return verb;
		var d = _body.GlobalPosition.DistanceTo(Sense.LastKnownTarget);
		if (d < Role.CloseRange) return NpcVerb.BackUp;
		if (d > Role.FarRange) return NpcVerb.MoveAndAttack;
		return verb;
	}

	public bool CanAttackAim(Vector3 aimPoint)
	{
		if (_body == null || !Vision.ReactionReady) return false;
		var look = -_body.GlobalTransform.Basis.Z;
		var to = aimPoint - _body.GlobalPosition;
		var cone = Stats != null ? Stats.VisionConeDeg : 110f;
		var slack = Stats != null ? Stats.AimSlackDeg : 30f;
		if (!NpcVision.InCone(look, to, cone)) return false;
		return NpcVision.CanFireWithoutTurn(look, to, slack)
		       || NpcVision.TurnTime(look, to, slack, Stats != null ? Stats.TurnRateDeg : 320f) <= (Stats != null ? Stats.PreRotate : 0.2f);
	}

	public void IncomingNade(Vector3 from)
	{
		if (_body == null) return;
		Reflex.TryNadeLeap(_body, _body.GlobalPosition - from);
	}

	public void NotifyAllyBrutalized() => Morale.OnAllyBrutalized(Role != null ? Role.FearResist : 0.2f);
	public void NotifyKill() => Morale.OnKill();
	public void NotifyOfficerDied() => Morale.OnOfficerDied();
}
