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
	[Export] public Node3D AssignedTarget;
	[Export] public float HazardRayMeters = 3.5f;

	public NpcReflex Reflex { get; } = new();
	public NpcVerb ActiveVerb { get; private set; }
	public NpcSenseSnapshot Sense;

	ActorEntity _body;

	public override void _Ready()
	{
		_body = GetParent() as ActorEntity;
		Role ??= new NpcRoleDef
		{
			RoleId = "soldier",
			DefaultVerb = NpcVerb.MoveAndAttack,
			PreferredRange = 18f
		};
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

		// Last-known from assigned Node3D export (overlap / vision later)
		if (AssignedTarget != null && GodotObject.IsInstanceValid(AssignedTarget))
		{
			Sense.LastKnownTarget = AssignedTarget.GlobalPosition;
			Sense.HasLastKnown = true;
		}

		// EQS lite: single ray behind the pawn. BackUp rewrites to GetOutOfView on hit.
		Sense.WorldHazardAhead = false;
		var space = _body.GetWorld3D()?.DirectSpaceState;
		if (space != null)
		{
			var origin = _body.GlobalPosition + Vector3.Up * 0.9f;
			// Godot look is -Z; behind is +Z
			var behind = _body.GlobalTransform.Basis.Z * HazardRayMeters;
			var q = PhysicsRayQueryParameters3D.Create(origin, origin + behind);
			q.CollideWithAreas = false;
			q.CollideWithBodies = true;
			q.Exclude = new Godot.Collections.Array<Rid> { _body.GetRid() };
			var hit = space.IntersectRay(q);
			if (hit.Count > 0)
			{
				Sense.WorldHazardAhead = true;
				Sense.WorldHazardPoint = (Vector3)hit["position"];
			}
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
