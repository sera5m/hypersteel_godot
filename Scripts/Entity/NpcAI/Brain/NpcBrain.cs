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
	[Export] public float PortalSeekRadius = 12f;

	public NpcReflex Reflex { get; } = new();
	public NpcVision Vision { get; } = new();
	public NpcMorale Morale { get; } = new();
	public NpcActionRunner Actions { get; } = new();
	public NpcTeamComms Comms { get; } = new();
	public NpcLoadout Loadout { get; } = new();
	public NpcVerb ActiveVerb { get; private set; }
	public NpcSenseSnapshot Sense;

	ActorEntity _body;
	PawnActions _pawn;

	public override void _Ready()
	{
		_body = GetParent() as ActorEntity;
		Role ??= new NpcRoleDef { RoleId = "soldier", DefaultVerb = NpcVerb.MoveAndAttack, PreferredRange = 18f };
		Stats ??= (_body?.Stats as NpcStats) ?? new NpcStats();
		Loadout.Bind(_body);
		EnsurePawn();
		if (_body != null) _body.AddToGroup("npc_squad");
		if (_body?.health != null) _body.health.Damaged += OnDamaged;
		if (_body?.feelings != null) _body.feelings.Flinch += OnFlinch;
	}

	void EnsurePawn()
	{
		if (_body == null) return;
		_pawn = _body.GetNodeOrNull<PawnActions>("PawnActions");
		if (_pawn == null)
		{
			_pawn = new PawnActions { Name = "PawnActions" };
			_body.AddChild(_pawn);
		}
		_pawn.Bind(_body);
		if (Loadout.Gun != null) _pawn.SetGun(Loadout.Gun);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_body == null || _body.IsDead) return;
		var dt = (float)delta;
		Vision.Tick(dt);
		Morale.Tick(dt);
		Actions.Tick(dt);
		TickSense();
		NpcFanOut.Apply(_body, ref Sense);
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
		Sense.HasPortal = false;
		Sense.HasFanDest = false;

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

		var stim = NpcWorldUse.Nearest(_body, NpcWorldKind.Stim, StimSeekRadius);
		if (stim != null) Sense.HasStim = true;

		var portal = NpcTrapPortal.NearestPortal(_body, PortalSeekRadius);
		if (portal != null)
		{
			Sense.HasPortal = true;
			Sense.PortalPoint = portal.GlobalPosition;
		}

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
			var reachQ = PhysicsRayQueryParameters3D.Create(origin, candidate);
			reachQ.CollideWithAreas = false;
			if (space.IntersectRay(reachQ).Count > 0) continue;
			var coverQ = PhysicsRayQueryParameters3D.Create(candidate, threat);
			coverQ.CollideWithAreas = false;
			if (space.IntersectRay(coverQ).Count == 0) continue;
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

	NpcVerb HelpDying(NpcVerb verb)
	{
		if (Role == null || !Role.HasTeamComms || Morale.State == NpcMoraleState.Terrified) return verb;
		var ally = Comms.DyingAlly();
		if (ally == null) return verb;
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
		EnsurePawn();
		_pawn?.SetGun(gun);
		if (Loadout.NeedsReload) { _pawn?.Reload(); return; }
		if (!Sense.HasLastKnown) return;
		var dist = _body.GlobalPosition.DistanceTo(Sense.LastKnownTarget);
		if (gun.Use != null && gun.Use.WantsAdsAtFar && dist >= Role.FarRange)
			_pawn?.SetAds(true);
		var aim = Sense.LastKnownTarget;
		if (AimLead.Flat(_body.GlobalPosition, Sense.LastKnownTarget, Vision.LastVel, gun.Muzzle, out var lead, out _))
			aim = lead;
		if (!CanAttackAim(aim)) return;
		if (_pawn == null || !_pawn.Fire(aim))
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
		var str = Stats != null ? Stats.Strength : 1f;
		var slow = Loadout.Gun != null ? Loadout.Gun.TurnSlowdown(str) : 0f;
		var turn = Stats != null ? Stats.LiveTurnRate(slow) : 320f;
		var pre = Stats != null ? Stats.PreRotate : 0.2f;
		return NpcVision.CanFireWithoutTurn(look, to, slack)
		       || NpcVision.TurnTime(look, to, slack, turn) <= pre;
	}

	public bool TryPlaceTrap(NpcTrapKind kind) => NpcTrapPortal.PlaceAtPortal(_body, kind, out _);

	public void IncomingNade(Vector3 from)
	{
		if (_body == null) return;
		Reflex.TryNadeLeap(_body, _body.GlobalPosition - from);
	}

	public void NotifyAllyBrutalized() => Morale.OnAllyBrutalized(Role != null ? Role.FearResist : 0.2f);
	public void NotifyKill() => Morale.OnKill();
	public void NotifyOfficerDied() => Morale.OnOfficerDied();
}
