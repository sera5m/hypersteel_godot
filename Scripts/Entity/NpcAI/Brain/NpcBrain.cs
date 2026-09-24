using Godot;
using Hypersteel.Damage;

namespace Hypersteel.Entity.NpcAI.Brain;

public partial class NpcBrain : Node
{
	[Export] public NpcRoleDef Role;
	[Export] public NpcStats Stats;
	[Export] public bool UseMeshConsensus;
	[Export] public Node3D AssignedTarget;
	[Export] public float HazardRayMeters = 3.5f;
	[Export] public float CoverSampleMeters = 5f;
	[Export] public float RollMinTaken = 8f;
	[Export] public float StimSeekRadius = 14f;

	public NpcReflex Reflex { get; } = new();
	public NpcVision Vision { get; } = new();
	public NpcMorale Morale { get; } = new();
	public NpcActionRunner Actions { get; } = new();
	public NpcTeamComms Comms { get; } = new();
	public NpcLoadout Loadout { get; } = new();
	public NpcVerb ActiveVerb { get; private set; }
	public NpcSenseSnapshot Sense;

	ActorEntity _body;

	public override void _Ready()
	{
		_body = GetParent() as ActorEntity;
		Role ??= new NpcRoleDef { RoleId = "soldier", DefaultVerb = NpcVerb.MoveAndAttack, PreferredRange = 18f };
		Stats ??= new NpcStats();
		Loadout.Bind(_body);
		if (_body != null) _body.AddToGroup("npc_squad");
		if (_body?.health != null) _body.health.Damaged += OnDamaged;
		if (_body?.feelings != null) _body.feelings.Flinch += OnFlinch;
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
		asked = HelpDying(asked);
		asked = SeekWorld(asked);
		ActiveVerb = NpcMacroMicro.Rewrite(asked, Sense, _body);
		NpcExecution.Step(_body, Role, ActiveVerb, Sense, dt);
		TryShoot();
	}

	void OnDamaged(float taken, int segment)
	{
		if (taken < RollMinTaken) return;
		RollAway();
	}

	void OnFlinch(float intensity)
	{
		if (intensity < 0.35f) return;
		RollAway();
	}

	void RollAway()
	{
		if (_body == null) return;
		Actions.Cancel();
		var from = Sense.HasLastKnown ? Sense.LastKnownTarget : _body.GlobalPosition + _body.GlobalTransform.Basis.Z;
		Reflex.TryHitRoll(_body, from);
	}

	void TickSense()
	{
		Sense.SuggestedMacro = UseMeshConsensus ? Sense.SuggestedMacro : NpcMacroIntent.None;
		Sense.WorldHazardAhead = false;
		Sense.HasCover = false;
		Sense.HasStim = false;

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
			Loadout.Gun?.SelectTarget(AssignedTarget);
			var look = -_body.GlobalTransform.Basis.Z;
			if (NpcVision.InCone(look, p - _body.GlobalPosition, Stats != null ? Stats.VisionConeDeg : 110f))
				Vision.NotifySeen(Stats);
		}

		// Stim presence for HelpDying / SeekWorld (17-items)
		var stim = NpcWorldUse.Nearest(_body, NpcWorldKind.Stim, StimSeekRadius);
		if (stim != null)
			Sense.HasStim = true;

		SampleHazardAndCover();
	}

	void SampleHazardAndCover()
	{
		if (_body == null) return;
		var space = _body.GetWorld3D()?.DirectSpaceState;
		if (space == null) return;

		var origin = _body.GlobalPosition + Vector3.Up * 1.2f;
		var forward = -_body.GlobalTransform.Basis.Z;
		forward.Y = 0f;
		if (forward.LengthSquared() < 0.01f) forward = Vector3.Forward;
		forward = forward.Normalized();

		// Forward hazard (ledge / wall that forces BackUp rewrite)
		var hazTo = origin + forward * HazardRayMeters;
		var hazQ = PhysicsRayQueryParameters3D.Create(origin, hazTo);
		hazQ.CollideWithAreas = false;
		var hazHit = space.IntersectRay(hazQ);
		if (hazHit.Count > 0)
		{
			Sense.WorldHazardAhead = true;
			Sense.WorldHazardPoint = (Vector3)hazHit["position"];
		}

		if (!Sense.HasLastKnown) return;

		// Cover EQS-lite: 8 ring samples; keep if LOS from sample to threat is blocked and self can see sample
		var threat = Sense.LastKnownTarget + Vector3.Up * 1.2f;
		var bestD = CoverSampleMeters + 1f;
		Vector3 best = default;
		var found = false;
		const int rings = 8;
		for (var i = 0; i < rings; i++)
		{
			var ang = i * (Mathf.Tau / rings);
			var dir = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
			var candidate = origin + dir * CoverSampleMeters;

			// reachable-ish: clear ray self → candidate
			var reachQ = PhysicsRayQueryParameters3D.Create(origin, candidate);
			reachQ.CollideWithAreas = false;
			var reachHit = space.IntersectRay(reachQ);
			if (reachHit.Count > 0) continue;

			// cover: LOS candidate → threat blocked
			var coverQ = PhysicsRayQueryParameters3D.Create(candidate, threat);
			coverQ.CollideWithAreas = false;
			var coverHit = space.IntersectRay(coverQ);
			if (coverHit.Count == 0) continue;

			var d = origin.DistanceTo(candidate);
			if (d >= bestD) continue;
			bestD = d;
			best = candidate;
			found = true;
		}

		if (found)
		{
			Sense.HasCover = true;
			Sense.CoverPoint = best;
		}
	}

	NpcVerb PickMacro() =>
		UseMeshConsensus && Sense.SuggestedMacro == NpcMacroIntent.Enclose
			? NpcVerb.Enclose
			: Role != null ? Role.DefaultVerb : NpcVerb.MoveAndAttack;

	NpcVerb Reshape(NpcVerb verb)
	{
		if (Role == null) return verb;
		if (Role.PingsOnSight && verb == NpcVerb.MoveAndAttack) return NpcVerb.GoLook;
		if (Role.IssuesEnclose && verb == NpcVerb.MoveAndAttack) return NpcVerb.Enclose;
		return verb;
	}

	NpcVerb ApplyMorale(NpcVerb verb) => Morale.State switch
	{
		NpcMoraleState.Terrified => NpcVerb.GetOutOfView,
		NpcMoraleState.Enraged => NpcVerb.MoveAndAttack,
		_ => verb
	};

	NpcVerb HoldBand(NpcVerb verb)
	{
		if (!Sense.HasLastKnown || Role == null || _body == null) return verb;
		var d = _body.GlobalPosition.DistanceTo(Sense.LastKnownTarget);
		if (d < Role.CloseRange) return NpcVerb.BackUp;
		return verb;
	}

	/// <summary>TeamComms HelpDying: stim (17) if present, else go to dying ally. Cover already preferred by Execution when HasCover.</summary>
	NpcVerb HelpDying(NpcVerb verb)
	{
		if (Role == null || !Role.HasTeamComms || Morale.State == NpcMoraleState.Terrified) return verb;
		var ally = Comms.DyingAlly();
		if (ally == null) return verb;

		// Prefer stim when one is in range (self-stim then cover/ally). Same world_pickup path as 17.
		if (Sense.HasStim)
		{
			var stim = NpcWorldUse.Nearest(_body, NpcWorldKind.Stim, StimSeekRadius);
			if (stim != null)
			{
				Sense.LastKnownTarget = stim.GlobalPosition;
				Sense.HasLastKnown = true;
				return NpcVerb.MoveAndAttack;
			}
		}

		Sense.LastKnownTarget = ally.GlobalPosition;
		Sense.HasLastKnown = true;
		return NpcVerb.MoveAndAttack;
	}

	NpcVerb SeekWorld(NpcVerb verb)
	{
		if (_body == null) return verb;
		if (Sense.OccupantHp01 > 0f && Sense.OccupantHp01 < 0.4f)
		{
			// Stim first (fast crumb), then Health kit
			if (Sense.HasStim)
			{
				var stim = NpcWorldUse.Nearest(_body, NpcWorldKind.Stim, StimSeekRadius);
				if (stim != null)
				{
					Sense.LastKnownTarget = stim.GlobalPosition;
					Sense.HasLastKnown = true;
					return NpcVerb.MoveAndAttack;
				}
			}
			var kit = NpcWorldUse.Nearest(_body, NpcWorldKind.Health);
			if (kit != null) { Sense.LastKnownTarget = kit.GlobalPosition; Sense.HasLastKnown = true; return NpcVerb.MoveAndAttack; }
		}
		if (Loadout.Gun != null && !Loadout.Gun.CheckAmmo())
		{
			var ammo = NpcWorldUse.Nearest(_body, NpcWorldKind.Ammo);
			if (ammo != null) { Sense.LastKnownTarget = ammo.GlobalPosition; Sense.HasLastKnown = true; return NpcVerb.MoveAndAttack; }
		}
		return verb;
	}

	void TryShoot()
	{
		if (Sense.HasLastKnown && Role != null && _body != null)
			Loadout.PickSlot(_body.GlobalPosition.DistanceTo(Sense.LastKnownTarget), Role.CloseRange, Role.FarRange, false);
		var gun = Loadout.Gun;
		if (gun == null || ActiveVerb != NpcVerb.MoveAndAttack) return;
		if (Loadout.NeedsReload) { gun.Reload(); return; }
		if (!Sense.HasLastKnown) return;
		var aim = Sense.LastKnownTarget;
		if (AimLead.Flat(_body.GlobalPosition, Sense.LastKnownTarget, Vision.LastVel, gun.Muzzle, out var lead, out _))
			aim = lead;
		if (!CanAttackAim(aim)) return;
		gun.PrimaryFire(aim);
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
